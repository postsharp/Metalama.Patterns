﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Extensions.DependencyInjection;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Caching.Implementation;
using System.Diagnostics;

#if NET6_0_OR_GREATER
using Metalama.Framework.RunTime;
#endif

namespace Metalama.Patterns.Caching;

// TODO: #33663 make local functions in templates static where possible
// ReSharper disable LocalFunctionCanBeMadeStatic 
#pragma warning disable IDE0062

/// <summary>
/// Custom attribute that, when applied on a method, causes the return value of the method to be cached
/// for the specific list of arguments passed to this method call.
/// </summary>
/// <remarks>
/// <para>There are several ways to configure the behavior of the <see cref="CacheAttribute"/> aspect: you can set the properties of the
/// <see cref="CacheAttribute"/> class, such as <see cref="AbsoluteExpiration"/> or <see cref="SlidingExpiration"/>. You can
/// add the <see cref="CachingConfigurationAttribute"/> custom attribute to the declaring type, a base type, or the declaring assembly.
/// Finally, you can define a profile by setting the <see cref="ProfileName"/> property and configure the profile at run time
/// by accessing the <see cref="CachingService.Profiles"/> collection of the <see cref="CachingService"/> class.</para>
/// <para>Use the <see cref="NotCacheKeyAttribute"/> custom attribute to exclude a parameter from being a part of the cache key.</para>
/// <para>To invalidate a cached method, see <see cref="InvalidateCacheAttribute"/> and the <see cref="CachingService.Invalidate(object)"/> method.</para>
/// </remarks>
[PublicAPI]
public sealed class CacheAttribute : BaseCachingAttribute, IAspect<IMethod>
{
    void IEligible<IMethod>.BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        builder.AddRule( EligibilityRuleFactory.GetAdviceEligibilityRule( AdviceKind.OverrideMethod ) );

        builder.MustNotHaveRefOrOutParameter();
        builder.ReturnType().MustSatisfy( t => t.SpecialType != SpecialType.Void, _ => $"the return type must not be void" );

        builder.ReturnType()
            .MustSatisfy( t => !t.IsTaskOrValueTask( false ), _ => $"the return type must not be a Task or ValueTask without a return value" );

        builder.ReturnType()
            .MustSatisfy(
                t => !t.GetAsyncInfo().IsAwaitable || t.IsTaskOrValueTask( true ),
                _ => $"the return type must not be an awaitable type other than Task<TResult> or ValueTask<TResult>" );
    }

    void IAspect<IMethod>.BuildAspect( IAspectBuilder<IMethod> builder )
    {
        Debugger.Break();
        
        if ( builder.AspectInstance.SecondaryInstances.Length > 0 )
        {
            builder.Diagnostics.Report( CachingDiagnosticDescriptors.Cache.ErrorMultipleInstancesOfCachedAttribute );

            return;
        }

        var unboundReturnSpecialType = (builder.Target.ReturnType as INamedType)?.GetOriginalDefinition().SpecialType ?? SpecialType.None;

        var returnTypeIsTask = unboundReturnSpecialType == SpecialType.Task_T;

        var options = builder.GetOptions<CachingOptions>();

        // Introduce a field of type CachedMethodRegistration.
        var registrationFieldPrefix = $"_cacheRegistration_{builder.Target.Name}";
        string registrationFieldName;

        if ( !builder.Target.DeclaringType.Fields.OfName( registrationFieldPrefix ).Any() )
        {
            registrationFieldName = registrationFieldPrefix;
        }
        else
        {
            for ( var i = 2; /* Intentionally empty */; i++ )
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

        // Introduce the dependency
        IFieldOrProperty? cachingServiceField;

        if ( options.UseDependencyInjection.GetValueOrDefault( true ) )
        {
            if ( builder.Target.IsStatic )
            {
                builder.Diagnostics.Report( CachingDiagnosticDescriptors.MethodCannotBeStaticBecauseItUsesDependencyInjection.WithArguments( builder.Target ) );
                builder.SkipAspect();

                return;
            }

            if ( !builder.TryIntroduceDependency(
                    new DependencyProperties(
                        builder.Target.DeclaringType,
                        typeof(CachingService),
                        "_cachingService" ),
                    out cachingServiceField ) )
            {
                builder.SkipAspect();

                return;
            }
        }
        else
        {
            cachingServiceField = null;
        }

        // Override the original method.
        var overrideTemplates = new MethodTemplateSelector(
            nameof(OverrideMethod),
            returnTypeIsTask ? nameof(OverrideMethodAsyncTask) : nameof(OverrideMethodAsyncValueTask),
            nameof(OverrideMethod),
            nameof(OverrideMethod),
            "OverrideMethodAsyncEnumerable",
            "OverrideMethodAsyncEnumerator",
            true,
            true );

        var genericValueType =
            unboundReturnSpecialType is SpecialType.Task_T or SpecialType.ValueTask_T or SpecialType.IAsyncEnumerable_T or SpecialType.IAsyncEnumerator_T
                ? ((INamedType) builder.Target.ReturnType).TypeArguments[0]
                : null;

        builder.Advice.Override(
            builder.Target,
            overrideTemplates,
            new
            {
                TValue = genericValueType, TReturnType = builder.Target.ReturnType, registrationField = registrationField.Declaration, cachingServiceField
            } );

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
            args: new { method = builder.Target, field = registrationField.Declaration, awaitableResultType, options } );

        builder.Advice.AddAnnotation(
            builder.Target,
            new CachedMethodAnnotation(
                new CachingOptions
                {
                    ProfileName = options.ProfileName,
                    IgnoreThisParameter = options.IgnoreThisParameter,
                    UseDependencyInjection = options.UseDependencyInjection
                } ),
            true );
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
    public void CachedMethodRegistrationInitializer( IMethod method, IField field, IType? awaitableResultType, [CompileTime] CachingOptions options )
        => field.Value = CachedMethodMetadata.Register(
            method.ToMethodInfo().ThrowIfMissing( method.ToDisplayString() ),
            awaitableResultType == null ? null : awaitableResultType.ToTypeOfExpression().Value,
            new CacheAttributeProperties()
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AutoReload = options.AutoReload,
                IgnoreThisParameter = options.IgnoreThisParameter,
                Priority = options.Priority,
                ProfileName = options.ProfileName,
                SlidingExpiration = options.SlidingExpiration
            },
            method.ReturnType.IsReferenceType == true || method.ReturnType.IsNullable == true );
#pragma warning restore IDE0031

    private static IParameter? GetCancellationTokenParameter() => meta.Target.Method.Parameters.OfParameterType( typeof(CancellationToken) ).LastOrDefault();

    private static IExpression GetCancellationTokenExpression()
        =>

            // TODO: CancellationToken is currently incorrectly handled and must be checked:
            // 1. The parameter should not be considered cacheable.
            // 2. With auto-reload, the value should be replaced by a default token.
            // 3. It is not tested.
            GetCancellationTokenParameter() ?? ExpressionFactory.Parse( "default" );

    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedParameter.Global
#pragma warning disable SA1313

    [Template]
    public static TReturnType? OverrideMethod<[CompileTime] TReturnType>( IField registrationField, IType TValue /* not used */, IField? cachingServiceField )
    {
        object? Invoke( object? instance, object?[] args )
        {
            return meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), null ) );
        }

        var cachingServiceExpression = cachingServiceField ?? ExpressionFactory.Capture( CachingServices.Default );

        return ((CachingService) cachingServiceExpression.Value!).GetFromCacheOrExecute<TReturnType>(
            (CachedMethodMetadata) registrationField.Value!,
            Invoke,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object[]) meta.Target.Method.Parameters.ToValueArray() );
    }

    [Template]
    public static Task<TValue?> OverrideMethodAsyncTask<[CompileTime] TValue>(
        IField registrationField,
        IType TReturnType /* not used */,
        IField? cachingServiceField )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async Task<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            return await meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) )!;
        }

        var cachingServiceExpression = cachingServiceField ?? ExpressionFactory.Capture( CachingServices.Default );

        return ((CachingService) cachingServiceExpression.Value!).GetFromCacheOrExecuteTaskAsync<TValue>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );
    }

    [Template]
    public static ValueTask<TValue?> OverrideMethodAsyncValueTask<[CompileTime] TValue>(
        IField registrationField,
        IType TReturnType /* not used */,
        IField? cachingServiceField )
    {
        var cancellationTokenExpression = GetCancellationTokenExpression();

        async ValueTask<object?> InvokeAsync( object? instance, object?[] args, CancellationToken cancellationToken )
        {
            return await meta.Target.Method.With( instance, InvokerOptions.Base )
                .Invoke( GetArgumentExpressions( meta.Target.Method, ExpressionFactory.Capture( args ), ExpressionFactory.Capture( cancellationToken ) ) )!;
        }

        var cachingServiceExpression = cachingServiceField ?? ExpressionFactory.Capture( CachingServices.Default );

        return ((CachingService) cachingServiceExpression.Value!).GetFromCacheOrExecuteValueTaskAsync<TValue>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );
    }

#if NETCOREAPP3_0_OR_GREATER
    [Template]
    public static IAsyncEnumerable<TValue>? OverrideMethodAsyncEnumerable<[CompileTime] TValue>(
        IField registrationField,
        IType TReturnType /* not used */,
        IField? cachingServiceField )
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

        var cachingServiceExpression = cachingServiceField ?? ExpressionFactory.Capture( CachingServices.Default );

        var task = ((CachingService) cachingServiceExpression.Value!).GetFromCacheOrExecuteValueTaskAsync<IAsyncEnumerable<TValue>>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );

        // Avoid extension method form due to current Metalama framework issue.
        // ReSharper disable once InvokeAsExtensionMethod
        return AsyncEnumerableHelper.AsAsyncEnumerable( task );
    }

    // ReSharper disable once UnusedMember.Global
    [Template]
    public static IAsyncEnumerator<TValue>? OverrideMethodAsyncEnumerator<[CompileTime] TValue>(
        IField registrationField,
        IType TReturnType /* not used */,
        IField? cachingServiceField )
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

        var cachingServiceExpression = cachingServiceField ?? ExpressionFactory.Capture( CachingServices.Default );

        var task = ((CachingService) cachingServiceExpression.Value!).GetFromCacheOrExecuteValueTaskAsync<IAsyncEnumerator<TValue>>(
            (CachedMethodMetadata) registrationField.Value!,
            InvokeAsync,
            meta.Target.Method.IsStatic ? null : (object) meta.This,
            (object?[]) meta.Target.Method.Parameters.ToValueArray(),
            (CancellationToken) cancellationTokenExpression.Value! );

        // Avoid extension method form due to current Metalama framework issue.
        // ReSharper disable once InvokeAsExtensionMethod
        return AsyncEnumerableHelper.AsAsyncEnumerator( task );
    }

#endif

    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedParameter.Global
#pragma warning restore SA1313
}

internal class CachedMethodAnnotation : IAnnotation<IMethod>
{
    public CachingOptions Options { get; }

    public CachedMethodAnnotation( CachingOptions options )
    {
        this.Options = options;
    }
}