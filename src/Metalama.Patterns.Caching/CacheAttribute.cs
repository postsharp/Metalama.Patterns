// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
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
        builder.ReturnType().MustSatisfy( t => !t.Is( typeof( Task ) ) || ((INamedType) t).IsGeneric, t => $"must not be non-generic Task" );
    }

    /* Consider alternative implementation strategy:
     * - Move runtime logic to helper method taking ValueTuple of args.
     * - Use ( delegate, TArgs ) pattern (where TArgs is tuple) when calling frontend to avoid delegate allocation and premature boxing.
     * - Aims to avoid allocation/boxing on cache hit.
     */

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        // TODO: [Porting] !!! Required, decide: Apply fallback configuration or deprecate [CacheConfiguration] and use Metalama options. See also BuildTimeCacheConfigurationManager.

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
         * 
         * TODO: Review - do we still need the ValueAdaptor layer with Metalama?
         */
        var templates = new MethodTemplateSelector(
            nameof( this.OverrideMethod ) ); //,
            //nameof( this.OverrideMethodAsync ),
            //nameof( this.OverrideMethod ) );

        var taskResultType = builder.Target.MethodDefinition.IsAsync
            ? builder.Target.MethodDefinition.GetAsyncInfo().ResultType
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
        
        var getOriginalMethodInvokerResult = builder.Advice.IntroduceMethod(            
            builder.Target.DeclaringType,
            nameof( this.GetOriginalMethodInvoker ),
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
                //return method.Invoke( args );
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
                //return method.With( instance ).Invoke( args );
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
        return CacheAttributeRuntime.OverrideMethod<TReturnType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
#if false
        var logSource = registration.Logger;

        object? result;

        // TODO: [Porting] Discuss: We could do this string interpolation at build time, but obfuscation/IL-rewriting could change the method signature before runtime. Best practice?
        using ( var activity = logSource.Default.OpenActivity( Formatted( "Processing invocation of method {Method}", registration.Method ) ) )
        {
            try
            {
                var mergedConfiguration = registration.MergedConfiguration;

                if ( !mergedConfiguration.IsEnabled.GetValueOrDefault() )
                {
                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Ignoring the caching aspect because caching is disabled for this profile." ) );

                    result = meta.Proceed();
                }
                else
                {
                    var methodKey = CachingServices.DefaultKeyBuilder.BuildMethodKey(
                        registration,
                        meta.Target.Method.Parameters.ToValueArray(),
                        meta.Target.Method.IsStatic || this.IgnoreThisParameter ? null : meta.This );

                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", methodKey ) );

                    // TODO: [Porting] Use ( delegate, TArgs ) pattern to avoid delegate creation on each call.

                    result = CachingFrontend.GetOrAdd(
                        registration.Method,
                        methodKey,
                        registration.Method.ReturnType,
                        mergedConfiguration,
                        (Func<TReturnType>) OriginalMethod,
                        logSource );
                }

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }

        if ( meta.Target.Method.ReturnType.IsReferenceType == true || meta.Target.Method.ReturnType.IsNullable == true )
        {
            return (TReturnType) result;
        }
        else
        {
            return result == null ? default : (TReturnType) result;
        }

        // TODO: [Porting] Make this method static if meta.Target.Method.IsStatic. How?

        TReturnType OriginalMethod()
        {
            return meta.Proceed();
        }
#endif
    }

#if false
    [Template]
    public async Task<TTaskResultType> OverrideMethodAsync<[CompileTime] TTaskResultType>( IField registrationField, IType taskResultType, IType TReturnType /* not used */ )
    {
        // TODO: What about ConfigureAwait( false )?

        var registration = registrationField.Value;

        var logSource = registration.Logger;

        object? result;
        
        // TODO: PostSharp passes an otherwise uninitialzed CallerInfo with the CallerAttributes.IsAsync flag set.

        // TODO: [Porting] Discuss: We could do this string interpolation at build time, but obfuscation/IL-rewriting could change the method signature before runtime. Best practice?
        using ( var activity = logSource.Default.OpenActivity( Formatted( "Processing invocation of async method {Method}", registration.Method ) ) )
        {
            try
            {
                var mergedConfiguration = registration.MergedConfiguration;

                if ( !mergedConfiguration.IsEnabled.GetValueOrDefault() )
                {
                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Ignoring the caching aspect because caching is disabled for this profile." ) );

                    var task = meta.Proceed();

                    if ( !task.IsCompleted )
                    {
                        // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                        // and the await instrumentation policy is not applied.
                        activity.Suspend();
                        try
                        {
                            result = await task;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }
                    else
                    {
                        // Don't wrap any exception.
                        result = task.GetAwaiter().GetResult();
                    }
                }
                else
                {
                    var methodKey = CachingServices.DefaultKeyBuilder.BuildMethodKey(
                        registration,
                        meta.Target.Method.Parameters.ToValueArray(),
                        meta.Target.Method.IsStatic || this.IgnoreThisParameter ? null : meta.This );

                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", methodKey ) );

                    // TODO: Pass CancellationToken (note from original code)

                    // TODO: [Porting] Use ( delegate, TArgs ) pattern to avoid delegate creation on each call.

                    var task = CachingFrontend.GetOrAddAsync(
                        registration.Method,
                        methodKey,
                        registration.Method.ReturnType,
                        mergedConfiguration,
                        (Func<Task<TTaskResultType>>?) OriginalMethod,
                        logSource,
                        CancellationToken.None );

                    if ( !task.IsCompleted )
                    {
                        // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                        // and the await instrumentation policy is not applied.
                        activity.Suspend();
                        try
                        {
                            result = await task;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }
                    else
                    {
                        // Don't wrap any exception.
                        result = task.GetAwaiter().GetResult();
                    }
                }

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }
        
        if ( taskResultType.IsReferenceType == true || taskResultType.IsNullable == true )
        {
            return (TTaskResultType?) result;
        }
        else
        {
            return result == null ? default : (TTaskResultType) result;
        }

        // TODO: [Porting] Make this method static if meta.Target.Method.IsStatic. How?

        async Task<TTaskResultType> OriginalMethod()
        {
            return await meta.Proceed();
        }
    }
#endif
}