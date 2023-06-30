// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Caching.Implementation;
using static Flashtrace.FormattedMessageBuilder;
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

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        // TODO: [Porting] !!! Required, decide: Apply fallback configuration or deprecate [CacheConfiguration] and use Metalama options. See also BuildTimeCacheConfigurationManager.

        var fieldName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{DC8C6993-4BD2-49BB-AACB-B628E69954CC}" );

        var registrationField = builder.Advice.IntroduceField(
            builder.Target.DeclaringType,
            fieldName,
            typeof( CachedMethodRegistration ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Accessibility.Private;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        builder.Advice.AddInitializer(
            builder.Target.DeclaringType,
            nameof( this.CachedMethodRegistrationInitializer ),
            InitializerKind.BeforeTypeConstructor,
            args: new { method = builder.Target, field = registrationField.Declaration } );

        /* Note: We use `OverrideMethod` explicitly as enumerableTemplate so we don't get .Buffer() added for enumerable methods. We don't
         * want buffering here because caching infrastructure uses ValueAdaptors for this.
         * 
         * TODO: Review - do we still need the ValueAdaptor layer with Metalama?
         */
        var templates = new MethodTemplateSelector( 
            nameof( this.OverrideMethod ),
            nameof( this.OverrideMethodAsync ),
            nameof( this.OverrideMethod ) );

        var taskResultType = builder.Target.MethodDefinition.IsAsync
            ? builder.Target.MethodDefinition.GetAsyncInfo().ResultType
            : null;
           
        builder.Advice.Override(
            builder.Target,
            templates,
            args: new { 
                TTaskResultType = taskResultType,
                TReturnType = builder.Target.ReturnType,
                taskResultType = taskResultType,
                registrationField = registrationField.Declaration } );
    }

    [Template]
    public TReturnType OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType taskResultType /* not used */, IType TTaskResultType /* not used */ )
    {
        var registration = registrationField.Value;

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
    }

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

    [Template]
    public void CachedMethodRegistrationInitializer( IMethod method, IField field )
    {
        field.Value = CachingServices.DefaultMethodRegistrationCache.Register(
            method.ToMethodInfo(),
            new CacheItemConfiguration()
            {
                AbsoluteExpiration = this._absoluteExpiration,
                AutoReload = this._autoReload,
                IgnoreThisParameter = this._ignoreThisParameter,
                Priority = this._priority,
                ProfileName = this.ProfileName,
                SlidingExpiration = this._slidingExpiration
            } );
    }
}