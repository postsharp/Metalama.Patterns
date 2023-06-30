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
        builder.ReturnType().MustSatisfy( t => !t.Is( typeof( Task ) ), t => $"must not be non-generic Task" );
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

        var templates = new MethodTemplateSelector( nameof( this.OverrideMethod ) );

        builder.Advice.Override( builder.Target, templates, args: new { registrationField = registrationField.Declaration } );
    }

    [Template]
    public dynamic OverrideMethod( IField registrationField )
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

                    result = OriginalMethod();
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
                        (Func<object?>) OriginalMethod,
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
            return meta.Cast( meta.Target.Method.ReturnType, result );
        }
        else
        {
            return result == null ? default : meta.Cast( meta.Target.Method.ReturnType, result );
        }

        object? OriginalMethod()
        {
            return meta.Proceed();
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