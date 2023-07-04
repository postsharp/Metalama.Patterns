// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

/* Task/ValueTask strategy:
 * 
 * - don't convert to Task for cache hit.
 * - Allow conversion to Task for cache miss and auto reload.
 * 
 */

public sealed class CacheAttribute : MethodAspect
{
    private bool? _autoReload;
    private TimeSpan? _absoluteExpiration;
    private TimeSpan? _slidingExpiration;
    private CacheItemPriority? _priority;
    private bool? _ignoreThisParameter;

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public string? ProfileName { get; set; }

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public bool AutoReload
    {
        get => this._autoReload.GetValueOrDefault();
        set => this._autoReload = value;
    }

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public TimeSpan AbsoluteExpiration
    {
        get => this._absoluteExpiration.GetValueOrDefault();
        set => this._absoluteExpiration = value;
    }

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public TimeSpan SlidingExpiration
    {
        get => this._slidingExpiration.GetValueOrDefault();
        set => this._slidingExpiration = value;
    }

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public CacheItemPriority Priority
    {
        get => this._priority.GetValueOrDefault();
        set => this._priority = value;
    }

    /// <inheritdoc cref="IBuildTimeCacheItemConfiguration" />
    public bool IgnoreThisParameter
    {
        get => this._ignoreThisParameter.GetValueOrDefault();
        set => this._ignoreThisParameter = value;
    }

    public override void BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        base.BuildEligibility( builder );

        builder.MustNotHaveRefOrOutParameter();
        builder.ReturnType().MustSatisfy( t => t.SpecialType != SpecialType.Void, t => $"must not be void" );
        builder.ReturnType().MustSatisfy( t => !t.Is( typeof( Task ) ) || ((INamedType) t).IsGeneric, t => $"must not be non-generic Task" );
        builder.ReturnType().MustSatisfy( t => !t.GetAsyncInfo().IsAwaitable || t.Is( typeof( Task ) ), t => $"must not be an awaitable type other than Task<TResult>" );
    }

    /* Consider alternative implementation strategy:
     * - Move runtime logic to helper method taking ValueTuple of args.
     * - Use ( delegate, TArgs ) pattern (where TArgs is tuple) when calling frontend to avoid delegate allocation and premature boxing.
     * - Aims to avoid allocation/boxing on cache hit.
     */

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        // TODO: [Porting] !!! Required, decide: Apply fallback configuration or deprecate [CacheConfiguration] and use Metalama options. See also BuildTimeCacheConfigurationManager.

        var asyncInfo = builder.Target.MethodDefinition.GetAsyncInfo();

        var registrationFieldName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{DC8C6993-4BD2-49BB-AACB-B628E69954CC}", $"_cacheRegistration_{builder.Target.Name}" );

        var registrationField = builder.Advice.IntroduceField(
            builder.Target.DeclaringType,
            registrationFieldName,
            typeof( CachedMethodRegistration ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Accessibility.Private;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        /* Note: We use `OverrideMethod` explicitly as enumerableTemplate so we don't get .Buffer() added for enumerable methods. We don't
         * want buffering here because caching infrastructure uses ValueAdaptors for this.
         */
        var templates = new MethodTemplateSelector(
            nameof( this.OverrideMethod ),
            nameof( this.OverrideMethodAsync ),
            nameof( this.OverrideMethod ),
            useAsyncTemplateForAnyAwaitable: true );

        var taskResultType = asyncInfo.IsAwaitable
            ? asyncInfo.ResultType
            : null;

        var overrideMethodResult = builder.Advice.Override(
            builder.Target,
            templates,
            args: new
            {
                TTaskResultType = taskResultType,
                TReturnType = builder.Target.ReturnType,
                taskResultType = taskResultType,
                registrationField = registrationField.Declaration
            } );

        var getInvokerMethodName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{FC85178F-3F0F-47E3-B5ED-25050804CF44}", $"GetInvoker_{builder.Target.Name}" );

        var getInvokerTemplate = asyncInfo.IsAwaitable ? nameof( this.GetOriginalMethodInvokerAsync ) : nameof( this.GetOriginalMethodInvoker );

        var getOriginalMethodInvokerResult = builder.Advice.IntroduceMethod(            
            builder.Target.DeclaringType,
            getInvokerTemplate,
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Accessibility.Private;
                b.Name = getInvokerMethodName;
            },
            args: new
            {
                // Invoking `method` needs to invoke the body of the original method, not the overridden one - otherwise we just get a circular call.
                method = builder.Target
            } );

        builder.Advice.AddInitializer(
            builder.Target.DeclaringType,
            nameof( this.CachedMethodRegistrationInitializer ),
            InitializerKind.BeforeTypeConstructor,
            args: new
            {
                method = builder.Target,
                field = registrationField.Declaration,
                getOriginalMethodInvoker = getOriginalMethodInvokerResult.Declaration
            } );
    }

    [Template]
    public Func<object?, object?[], object?> GetOriginalMethodInvoker( IMethod method )
    {
        return Invoke;

        object? Invoke( object? instance, object?[] args )
        {
            // In both cases, invoking `method` needs to invoke the body of the original method, not the overridden one - otherwise we just get a circular call.
            if ( method.IsStatic )
            {
                /* I want to write:
                 * 
                 *  method.Invoke( args );
                 * 
                 * Which should emit, for the actual number of parameters in `method`:
                 * 
                 *  ActualNameOfMethod( (ActualParameter0Type)args[0], (ActualParameter1Type)args[1] )
                 *  
                 */
                return method.With( InvokerOptions.Base ).Invoke( meta.Cast( method.Parameters[0].Type, args[0] ) );
            }
            else
            {
                /* I want to write:
                 * 
                 *  method.With( instance ).Invoke( args );
                 * 
                 * Which should emit, for the actual number of parameters in `method`:
                 * 
                 *  ((ActualDeclaringTypeOfMethod)instance).ActualNameOfMethod( (ActualParameter0Type)args[0], (ActualParameter1Type)args[1] )
                 *  
                 */
                return method.With( InvokerOptions.Base ).With( instance ).Invoke( meta.Cast( method.Parameters[0].Type, args[0] ) );
            }
        }
    }

    [Template]
    public Func<object?, object?[], Task<object?>> GetOriginalMethodInvokerAsync( IMethod method )
    {
        return Invoke;

        async Task<object?> Invoke( object? instance, object?[] args )
        {
            // In both cases, invoking `method` needs to invoke the body of the original method, not the overridden one - otherwise we just get a circular call.
            if ( method.IsStatic )
            {
                /* I want to write:
                 * 
                 *  method.Invoke( args );
                 * 
                 * Which should emit, for the actual number of parameters in `method`:
                 * 
                 *  ActualNameOfMethod( (ActualParameter0Type)args[0], (ActualParameter1Type)args[1] )
                 *  
                 */
                //return await method.Invoke( args );
                meta.InsertComment( $"Would have called {method.Name} here." );
                return null;
            }
            else
            {
                /* I want to write:
                 * 
                 *  method.With( instance ).Invoke( args );
                 * 
                 * Which should emit, for the actual number of parameters in `method`:
                 * 
                 *  ((ActualDeclaringTypeOfMethod)instance).ActualNameOfMethod( (ActualParameter0Type)args[0], (ActualParameter1Type)args[1] )
                 *  
                 */
                //return await method.With( instance ).Invoke( args );
                meta.InsertComment( $"Would have called {method.Name} here." );
                return null;
            }
        }
    }

    [Template]
    public void CachedMethodRegistrationInitializer( IMethod method, IField field, IMethod getOriginalMethodInvoker )
    {
        field.Value = CachingServices.DefaultMethodRegistrationCache.Register(
            method.ToMethodInfo(),
            getOriginalMethodInvoker.Invoke(),
            new CacheItemConfiguration()
            {
                AbsoluteExpiration = this._absoluteExpiration,
                AutoReload = this._autoReload,
                IgnoreThisParameter = this._ignoreThisParameter,
                Priority = this._priority,
                ProfileName = this.ProfileName,
                SlidingExpiration = this._slidingExpiration,
            },
            method.ReturnType.IsReferenceType == true || method.ReturnType.IsNullable == true );
    }

    [Template]
    public TReturnType OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType taskResultType /* not used */, IType TTaskResultType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethod<TReturnType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public Task<TTaskResultType> OverrideMethodAsync<[CompileTime] TTaskResultType>( IField registrationField, IType taskResultType, IType TReturnType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethodAsync<TTaskResultType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }
}