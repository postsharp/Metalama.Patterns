// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

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
        builder.ReturnType().MustSatisfy( t => !t.IsTaskOrValueTask( hasResult: false ), t => $"must not be a Task or ValueTask without a return value" );
        builder.ReturnType().MustSatisfy( t => !t.GetAsyncInfo().IsAwaitable || t.IsTaskOrValueTask( hasResult: true ), t => $"must not be an awaitable type other than Task<TResult> or ValueTask<TResult>" );
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
        var returnTypeIsTask = builder.Target.ReturnType.IsTask();

        // looseReturnType is object, or if eligible awaitable, Task<object> or ValueTask<object>
        //var looseReturnType = asyncInfo.IsAwaitable
        //    ? TypeFactory.GetType( ((INamedType) builder.Target.ReturnType).GetOriginalDefinition().SpecialType ).WithTypeArguments( TypeFactory.GetType( SpecialType.Object ) ).ToNullableType()
        //    : TypeFactory.GetType( SpecialType.Object ).ToNullableType();

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
            returnTypeIsTask ? nameof( this.OverrideMethodAsyncTask ) : nameof( this.OverrideMethodAsyncValueTask ),
            nameof( this.OverrideMethod ),
            nameof( this.OverrideMethod ),
            nameof( this.OverrideMethod ),
            nameof( this.OverrideMethod ),            
            useAsyncTemplateForAnyAwaitable: true ,
            useEnumerableTemplateForAnyEnumerable: true );

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
                registrationField = registrationField.Declaration
            } );

        var getInvokerMethodName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{FC85178F-3F0F-47E3-B5ED-25050804CF44}", $"GetInvoker_{builder.Target.Name}" );

        var getInvokerTemplate = asyncInfo.IsAwaitable 
            ? returnTypeIsTask ? nameof( this.GetOriginalMethodInvokerForTask ) : nameof( this.GetOriginalMethodInvokerForValueTask )
            : nameof( this.GetOriginalMethodInvoker );

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
                method = builder.Target,
                TResult = TypeFactory.GetType( SpecialType.Object ) // TODO: For now. Might change for enumerables etc. If not, simplify.
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
    public Func<object?, object?[], TResult?> GetOriginalMethodInvoker<[CompileTime] TResult>( IMethod method )
    {
        return Invoke;

        TResult? Invoke( object? instance, object?[] args )
        {
            return method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
        }
    }
        
    [Template]
    public Func<object?, object?[], Task<object?>> GetOriginalMethodInvokerForTask( IMethod method, IType TResult /* not used */ )
    {
        return Invoke;
        
        async Task<object?> Invoke( object? instance, object?[] args )
        {
            return await method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
        }
    }

    [Template]
    public Func<object?, object?[], ValueTask<object?>> GetOriginalMethodInvokerForValueTask( IMethod method, IType TResult /* not used */ )
    {
        return Invoke;

        async ValueTask<object?> Invoke( object? instance, object?[] args )
        {
            return await method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
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
    public TReturnType OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType TTaskResultType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethod<TReturnType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public Task<TTaskResultType> OverrideMethodAsyncTask<[CompileTime] TTaskResultType>( IField registrationField, IType TReturnType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethodAsyncTask<TTaskResultType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public ValueTask<TTaskResultType> OverrideMethodAsyncValueTask<[CompileTime] TTaskResultType>( IField registrationField, IType TReturnType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethodAsyncValueTask<TTaskResultType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }
}