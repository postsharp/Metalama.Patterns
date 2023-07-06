﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Eligibility;
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
/// by accessing the <see cref="CachingServices.Profiles"/> collection of the <see cref="CachingServices"/> class.</para>
/// <para>Use the <see cref="NotCacheKeyAttribute"/> custom attribute to exclude a parameter from being a part of the cache key.</para>
/// <para>To invalidate a cached method, see <see cref="InvalidateCacheAttribute"/> and <see cref="CachingServices.Invalidation"/>.</para>
/// </remarks>
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
        builder.ReturnType().MustSatisfy( t => t.SpecialType != SpecialType.Void, t => $"must not be void" );
        builder.ReturnType().MustSatisfy( t => !t.IsTaskOrValueTask( hasResult: false ), t => $"must not be a Task or ValueTask without a return value" );
        builder.ReturnType().MustSatisfy( t => !t.GetAsyncInfo().IsAwaitable || t.IsTaskOrValueTask( hasResult: true ), t => $"must not be an awaitable type other than Task<TResult> or ValueTask<TResult>" );
    }

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        // TODO: [Porting] !!! Required, decide: Apply fallback configuration or deprecate [CacheConfiguration] and use Metalama options. See also BuildTimeCacheConfigurationManager.

        var asyncInfo = builder.Target.MethodDefinition.GetAsyncInfo();
        var unboundReturnSpecialType = (builder.Target.ReturnType as INamedType)?.GetOriginalDefinition().SpecialType ?? SpecialType.None;

        var returnTypeIsTask = unboundReturnSpecialType == SpecialType.Task_T;

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

        var templates = new MethodTemplateSelector(
            defaultTemplate: nameof( this.OverrideMethod ),
            asyncTemplate: returnTypeIsTask ? nameof( this.OverrideMethodAsyncTask ) : nameof( this.OverrideMethodAsyncValueTask ),
            enumerableTemplate: nameof( this.OverrideMethod ),
            enumeratorTemplate: nameof( this.OverrideMethod ),
            asyncEnumerableTemplate: "OverrideMethodAsyncEnumerable",
            asyncEnumeratorTemplate: "OverrideMethodAsyncEnumerator",
            useAsyncTemplateForAnyAwaitable: true,
            useEnumerableTemplateForAnyEnumerable: true );

        var genericValueType = unboundReturnSpecialType is SpecialType.Task_T or SpecialType.ValueTask_T or SpecialType.IAsyncEnumerable_T or SpecialType.IAsyncEnumerator_T
            ? ((INamedType) builder.Target.ReturnType).TypeArguments[0]
            : null;

        var overrideMethodResult = builder.Advice.Override(
            builder.Target,
            templates,
            args: new
            {
                TValue = genericValueType,
                TReturnType = builder.Target.ReturnType,
                registrationField = registrationField.Declaration
            } );

        var getInvokerMethodName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{FC85178F-3F0F-47E3-B5ED-25050804CF44}", $"GetInvoker_{builder.Target.Name}" );

        var getInvokerTemplate = unboundReturnSpecialType switch
        {
            SpecialType.Task_T => nameof( this.GetOriginalMethodInvokerForTask ),
            SpecialType.ValueTask_T => nameof( this.GetOriginalMethodInvokerForValueTask ),
            SpecialType.IAsyncEnumerable_T or SpecialType.IAsyncEnumerator_T => "GetOriginalMethodInvokerForAsyncEnumerableOrEnumerator",
            _ => nameof( this.GetOriginalMethodInvoker )
        };

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
                method = builder.Target
            } );

        // TODO: When subtemplates are supported, consider folding getInvokerTemplate into CachedMethodRegistrationInitializer.

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
            return method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
        }
    }
        
    [Template]
    public Func<object?, object?[], Task<object?>> GetOriginalMethodInvokerForTask( IMethod method )
    {
        return Invoke;
        
        async Task<object?> Invoke( object? instance, object?[] args )
        {
            return await method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
        }
    }

    [Template]
    public Func<object?, object?[], ValueTask<object?>> GetOriginalMethodInvokerForValueTask( IMethod method )
    {
        return Invoke;

        async ValueTask<object?> Invoke( object? instance, object?[] args )
        {
            return await method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args );
        }
    }

#if NETCOREAPP3_0_OR_GREATER

    [Template]
    public Func<object?, object?[], ValueTask<object?>> GetOriginalMethodInvokerForAsyncEnumerableOrEnumerator( IMethod method )
    {
        return Invoke;

        ValueTask<object?> Invoke( object? instance, object?[] args )
        {
            return new ValueTask<object?>( method.With( instance, InvokerOptions.Base ).InvokeWithArgumentsObject( args ) );
        }
    }

#endif

    [Template]
    public void CachedMethodRegistrationInitializer( IMethod method, IField field, IMethod getOriginalMethodInvoker )
    {
        var effectiveConfiguration = this.GetEffectiveConfiguration( method );

        field.Value = CachingServices.DefaultMethodRegistrationCache.Register(
            method.ToMethodInfo(),
            getOriginalMethodInvoker.Invoke(),
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

    [Template]
    public TReturnType OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType TValue /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethod<TReturnType>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public Task<TValue> OverrideMethodAsyncTask<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethodAsyncTask<TValue>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public ValueTask<TValue> OverrideMethodAsyncValueTask<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        return CacheAttributeRunTime.OverrideMethodAsyncValueTask<TValue>(
            registrationField.Value,
            meta.Target.Method.IsStatic ? null : meta.This,
            meta.Target.Method.Parameters.ToValueArray() );
    }

#if NETCOREAPP3_0_OR_GREATER
    
    [Template]
    public IAsyncEnumerable<TValue> OverrideMethodAsyncEnumerable<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var task = CacheAttributeRunTime.OverrideMethodAsyncValueTask<IAsyncEnumerable<TValue>>(
                registrationField.Value,
                meta.Target.Method.IsStatic ? null : meta.This,
                meta.Target.Method.Parameters.ToValueArray() );

        return task.AsAsyncEnumerable();
    }

    [Template]
    public IAsyncEnumerator<TValue> OverrideMethodAsyncEnumerator<[CompileTime] TValue>( IField registrationField, IType TReturnType /* not used */ )
    {
        var task = CacheAttributeRunTime.OverrideMethodAsyncValueTask<IAsyncEnumerator<TValue>>(
                registrationField.Value,
                meta.Target.Method.IsStatic ? null : meta.This,
                meta.Target.Method.Parameters.ToValueArray() );

        return task.AsAsyncEnumerator();
    }

#endif

    /// <summary>
    /// Gets the effective configuration of the method by applying fallback configuration from <see cref="CacheConfigurationAttribute"/>
    /// attributes on ancestor types and the declaring assembly.
    /// </summary>
    [CompileTime]
    private CompileTimeCacheItemConfiguration GetEffectiveConfiguration( IMethod method )
    {
        // TODO: Consider reinstating cache of [CacheConfiguration] lookups.
        // PostSharp built a cache of search results so that subsequent lookups would avoid repeating the same work.
        // This requires the feature "user design-time caching" which is not yet implemented in Metalama. This
        // kind of caching might be better implemented at framework level.
        // See BuildTimeCacheConfigurationManager in the PostSharp codebase.
        var mergedConfiguration = new CompileTimeCacheItemConfiguration() 
        {  
            AbsoluteExpiration = this._absoluteExpiration, 
            AutoReload = this._autoReload,
            IgnoreThisParameter = this._ignoreThisParameter,
            Priority = this._priority,
            ProfileName = this.ProfileName,
            SlidingExpiration = this._slidingExpiration
        };

        var configurationFromAttributes = CompileTimeCacheConfigurationHelper.GetConfigurationFromAttributes( method );
        mergedConfiguration.ApplyFallback( configurationFromAttributes );

        return mergedConfiguration;
    }
}
