// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.RunTime;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Custom attribute that, when applied on a method, causes the return value of the method to be cached
/// for the specific list of arguments passed to this method call.
/// </summary>
/// <remarks>
/// <para>There are several ways to configure the behavior of the <see cref="CacheAttribute"/> aspect: you can set the properties of the
/// <see cref="CacheAttribute"/> class, such as <see cref="AbsoluteExpiration"/> or <see cref="SlidingExpiration"/>. You can
/// add the <see cref="CacheConfigurationAttribute"/> custom attribute to the declaring type, a base type, or the declaring assembly.
/// Finally, you can define a profile by setting the <see cref="ProfileName"/> property and configure the profile at run time
/// by accessing the <see cref="CachingServices.DefaultService.Profiles"/> collection of the <see cref="CachingServices"/> class.</para>
/// <para>Use the <see cref="NotCacheKeyAttribute"/> custom attribute to exclude a parameter from being a part of the cache key.</para>
/// <para>To invalidate a cached method, see <see cref="InvalidateCacheAttribute"/> and <see cref="CachingServices.DefaultService.Invalidation"/>.</para>
/// </remarks>
[PublicAPI]
public sealed class CacheAttribute : MethodAspect
{
    private bool? _autoReload;
    private TimeSpan? _absoluteExpiration;
    private TimeSpan? _slidingExpiration;
    private CacheItemPriority? _priority;
    private bool? _ignoreThisParameter;

    /// <summary>
    /// Gets or sets the name of the <see cref="CachingProfile"/>  that contains the configuration of the cached methods.
    /// </summary>
    public string? ProfileName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    public bool AutoReload
    {
        get => this._autoReload.GetValueOrDefault();
        set => this._autoReload = value;
    }

    /// <summary>
    /// Gets or sets the total duration, in minutes, during which the result of the current method is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    public double AbsoluteExpiration
    {
        get => this._absoluteExpiration.GetValueOrDefault( TimeSpan.Zero ).TotalMinutes;
        set => this._absoluteExpiration = TimeSpan.FromMinutes( value );
    }

    /// <summary>
    /// Gets or sets the duration, in minutes, during which the result of the current method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    public double SlidingExpiration
    {
        get => this._slidingExpiration.GetValueOrDefault( TimeSpan.Zero ).TotalMinutes;
        set => this._slidingExpiration = TimeSpan.FromMinutes( value );
    }

    /// <summary>
    /// Gets or sets the priority of the current method.
    /// </summary>
    public CacheItemPriority Priority
    {
        get => this._priority.GetValueOrDefault( CacheItemPriority.Default );
        set => this._priority = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
    /// which means that by default the <c>this</c> instance is a part of the cache key.
    /// </summary>
    public bool IgnoreThisParameter
    {
        get => this._ignoreThisParameter.GetValueOrDefault();
        set => this._ignoreThisParameter = value;
    }

    public override void BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        base.BuildEligibility( builder );

        builder.MustNotHaveRefOrOutParameter();
        builder.ReturnType().MustSatisfy( t => t.SpecialType != SpecialType.Void, _ => $"the return type must not be void" );

        builder.ReturnType()
            .MustSatisfy( t => !t.IsTaskOrValueTask( hasResult: false ), _ => $"the return type must not be a Task or ValueTask without a return value" );

        builder.ReturnType()
            .MustSatisfy(
                t => !t.GetAsyncInfo().IsAwaitable || t.IsTaskOrValueTask( hasResult: true ),
                _ => $"the return type must not be an awaitable type other than Task<TResult> or ValueTask<TResult>" );
    }

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        if ( builder.AspectInstance.SecondaryInstances.Length > 0 )
        {
            builder.Diagnostics.Report( CachingDiagnosticDescriptors.Cache.ErrorMultipleInstancesOfCachedAttribute );

            return;
        }

        var unboundReturnSpecialType = (builder.Target.ReturnType as INamedType)?.GetOriginalDefinition().SpecialType ?? SpecialType.None;

        var returnTypeIsTask = unboundReturnSpecialType == SpecialType.Task_T;

        // Introduce a field of type CachedMethodRegistration.
        var registrationFieldPrefix = $"_cacheRegistration_{builder.Target.Name}";
        string registrationFieldName;

        if ( !builder.Target.DeclaringType.Fields.OfName( registrationFieldPrefix ).Any() )
        {
            registrationFieldName = registrationFieldPrefix;
        }
        else
        {
            for ( var i = 2; true; i++ )
            {
                registrationFieldName = $"{registrationFieldPrefix}{i}";

                if ( !builder.Target.DeclaringType.Fields.OfName( registrationFieldName ).Any() )
                {
                    break;
                }
            }
        }

        var registrationField = builder.Advice.IntroduceField(
            builder.Target.DeclaringType,
            registrationFieldName,
            typeof(CachedMethodMetadata),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Accessibility.Private;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        // Override the original method.
        var overrideTemplates = new MethodTemplateSelector(
            defaultTemplate: nameof(OverrideMethod),
            asyncTemplate: returnTypeIsTask ? nameof(OverrideMethodAsyncTask) : nameof(OverrideMethodAsyncValueTask),
            enumerableTemplate: nameof(OverrideMethod),
            enumeratorTemplate: nameof(OverrideMethod),
            asyncEnumerableTemplate: "OverrideMethodAsyncEnumerable",
            asyncEnumeratorTemplate: "OverrideMethodAsyncEnumerator",
            useAsyncTemplateForAnyAwaitable: true,
            useEnumerableTemplateForAnyEnumerable: true );

        var genericValueType =
            unboundReturnSpecialType is SpecialType.Task_T or SpecialType.ValueTask_T or SpecialType.IAsyncEnumerable_T or SpecialType.IAsyncEnumerator_T
                ? ((INamedType) builder.Target.ReturnType).TypeArguments[0]
                : null;

        builder.Advice.Override(
            builder.Target,
            overrideTemplates,
            args: new { TValue = genericValueType, TReturnType = builder.Target.ReturnType, registrationField = registrationField.Declaration } );

        // Introduce a method that invokes the original method body but where the arguments are passed through an array.
        var getInvokerMethodName = builder.Target.ToSerializableId()
            .MakeAssociatedIdentifier( $"GetInvoker_{builder.Target.Name}" );

        // TODO: The generation scheme could be significantly simplified with sub-templates:
        // The invoker method should be just a delegate in the original method. There would be no need for separate methods, and the diff would look better.
        // This would solve the issue of ugly names of methods at least.
        // (not so sure that sub-templates are needed)

        // Initialize the CachedMethodRegistration field from the static constructor.
        var awaitableResultType = unboundReturnSpecialType switch
        {
            SpecialType.Task_T or SpecialType.ValueTask_T => ((INamedType) builder.Target.ReturnType).TypeArguments[0],
            SpecialType.IAsyncEnumerable_T or SpecialType.IAsyncEnumerator_T => (INamedType) builder.Target.ReturnType,
            _ => null
        };

        builder.Advice.AddInitializer(
            builder.Target.DeclaringType,
            nameof(this.CachedMethodRegistrationInitializer),
            InitializerKind.BeforeTypeConstructor,
            args: new { method = builder.Target, field = registrationField.Declaration, awaitableResultType } );

        var effectiveConfiguration = this.GetEffectiveConfiguration( builder.Target );

        // Here we replace (or add, if this aspect was applied eg by fabric) the [Cache]
        // attribute with a [Cache] attribute initialized with the effective configuration
        // taking into account [CacheConfiguration]. This "attaches" the effective
        // configuration to the method itself, which is more robust with respect to
        // linker trimming, obfuscation, assembly merging and so on. For fabric-applied aspects,
        // this is how aspect configuration is persisted.

        builder.Advice.IntroduceAttribute( builder.Target, effectiveConfiguration.ToAttributeConstruction(), OverrideStrategy.Override );
    }

    private static IEnumerable<IExpression> GetArgumentExpressions( IMethod method, IExpression arrayExpression, IExpression? cancellationTokenArgument )
    {
        // If the method accepts a CancellationToken parameter, we have to replace it with the value we received.

        var cancellationTokenParameter = cancellationTokenArgument != null ? GetCancellationTokenParameter() : null;

        IExpression GetItem( IParameter p )
        {
            if ( cancellationTokenParameter == p )
            {
                return cancellationTokenArgument!;
            }
            else
            {
                var eb = new ExpressionBuilder();
                eb.AppendExpression( arrayExpression );
                eb.AppendVerbatim( "[" );
                eb.AppendLiteral( p.Index );
                eb.AppendVerbatim( "]" );

                return eb.ToExpression();
            }
        }

        return method.Parameters.Select( GetItem );
    }

    // ReSharper disable once MergeConditionalExpression
#pragma warning disable IDE0031
    [Template]
    public void CachedMethodRegistrationInitializer( IMethod method, IField field, IType? awaitableResultType )
    {
        var effectiveConfiguration = this.GetEffectiveConfiguration( method );

        field.Value = CachedMethodMetadata.Register(
            method.ToMethodInfo().ThrowIfMissing( method.ToDisplayString() ),
            awaitableResultType == null ? null : awaitableResultType.ToTypeOfExpression().Value,
            new CacheAttributeProperties()
            {
                AbsoluteExpiration = effectiveConfiguration.AbsoluteExpiration,
                AutoReload = effectiveConfiguration.AutoReload,
                IgnoreThisParameter = effectiveConfiguration.IgnoreThisParameter,
                Priority = effectiveConfiguration.Priority,
                ProfileName = effectiveConfiguration.ProfileName,
                SlidingExpiration = effectiveConfiguration.SlidingExpiration
            },
            method.ReturnType.IsReferenceType == true || method.ReturnType.IsNullable == true );
    }
#pragma warning restore IDE0031

    private static IParameter? GetCancellationTokenParameter()
    {
        return meta.Target.Method.Parameters.OfParameterType( typeof(CancellationToken) ).LastOrDefault();
    }

    private static IExpression GetCancellationTokenExpression()
    {
        // TODO: CancellationToken is currently incorrectly handled and must be checked:
        // 1. The parameter should not be considered cacheable.
        // 2. With auto-reload, the value should be replaced by a default token.
        // 3. It is not tested.

        return GetCancellationTokenParameter() ?? ExpressionFactory.Parse( "default" );
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313

    [Template]
    public static TReturnType? OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType TValue /* not used */ )
    {
        object? Invoke( object? instance, object?[] args )
        {
            return meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), null ) );
        }

        return CachingServices.DefaultService.Lookup.GetFromCacheOrExecute<TReturnType>(
            (CachedMethodMetadata) registrationField.Value!,
            Invoke,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object[]) meta.Target.Method.Parameters.ToValueArray()! );
    }

    [Template]
    public static Task<TValue?> OverrideMethodAsyncTask<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async Task<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            return await meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) )!;
        }

        return CachingServices.DefaultService.Lookup.GetFromCacheOrExecuteTaskAsync<TValue>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );
    }

    [Template]
    public static ValueTask<TValue?> OverrideMethodAsyncValueTask<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async ValueTask<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            return await meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) )!;
        }

        return CachingServices.DefaultService.Lookup.GetFromCacheOrExecuteValueTaskAsync<TValue>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );
    }

    // ReSharper disable once RedundantBlankLines
#if NETCOREAPP3_0_OR_GREATER
    // ReSharper disable once UnusedMember.Global
    [Template]
    public static IAsyncEnumerable<TValue>? OverrideMethodAsyncEnumerable<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async ValueTask<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            var enumerable = (IAsyncEnumerable<TValue>?)
                meta.Target.Method.With( instance, InvokerOptions.Base )
                    .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) );

            if ( enumerable != null )
            {
                return await enumerable.BufferAsync( cancellationToken );
            }
            else
            {
                return null;
            }
        }

        var task = CachingServices.DefaultService.Lookup.GetFromCacheOrExecuteValueTaskAsync<IAsyncEnumerable<TValue>>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );

        // Avoid extension method form due to current Metalama framework issue.
        return AsyncEnumerableHelper.AsAsyncEnumerable( task );
    }

    // ReSharper disable once UnusedMember.Global
    [Template]
    public static IAsyncEnumerator<TValue> OverrideMethodAsyncEnumerator<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async ValueTask<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            var enumerator = (IAsyncEnumerator<TValue>?)
                meta.Target.Method.With( instance, InvokerOptions.Base )
                    .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) );

            if ( enumerator == null )
            {
                return null;
            }

            var buffer = await enumerator.BufferAsync( cancellationToken );

            return buffer;
        }

        var task = CachingServices.DefaultService.Lookup.GetFromCacheOrExecuteValueTaskAsync<IAsyncEnumerator<TValue>>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );

        // Avoid extension method form due to current Metalama framework issue.
        return AsyncEnumerableHelper.AsAsyncEnumerator( task );
    }

#endif

    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedParameter.Global
#pragma warning restore SA1313

    /// <summary>
    /// Gets the effective configuration of the method by applying fallback configuration from <see cref="CacheConfigurationAttribute"/>
    /// attributes on ancestor types and the declaring assembly.
    /// </summary>
    [CompileTime]
    private CompileTimeCacheItemConfiguration GetEffectiveConfiguration( IMethod method )
    {
        var mergedConfiguration = this.ToCompileTimeCacheItemConfiguration();
        mergedConfiguration.ApplyEffectiveConfiguration( method );

        return mergedConfiguration;
    }

    [CompileTime]
    internal CompileTimeCacheItemConfiguration ToCompileTimeCacheItemConfiguration()
        => new()
        {
            AbsoluteExpiration = this._absoluteExpiration,
            AutoReload = this._autoReload,
            IgnoreThisParameter = this._ignoreThisParameter,
            Priority = this._priority,
            ProfileName = this.ProfileName,
            SlidingExpiration = this._slidingExpiration
        };
}