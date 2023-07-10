// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.Implementation;
using System.Collections;
using System.Reflection;
using static Metalama.Patterns.Caching.CachingDiagnosticDescriptors.InvalidateCache;

[assembly:AspectOrder(typeof(InvalidateCacheAttribute), typeof(CacheAttribute))]

namespace Metalama.Patterns.Caching;

/// <summary>
/// Custom attribute that, when applied on a method, causes an invocation of this method to remove from
/// the cache the result of invocations of other given methods with the same parameter values. 
/// </summary>
[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public sealed class InvalidateCacheAttribute : MethodAspect
{

    private readonly Type? _invalidatedMethodsDeclaringType;

    private readonly string[] _invalidatedMethodNames;

    /// <summary>
    /// Gets or sets a value indicating whether the current <see cref="InvalidateCacheAttribute"/> can match several overloads of the methods.
    /// The default value is <c>false</c>, which means that an diagnostic will be emitted if the current <see cref="InvalidateCacheAttribute"/> matches
    /// several methods of the same name.
    /// </summary>
    public bool AllowMultipleOverloads { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidateCacheAttribute"/> class that invalidates method of the same type as the
    /// type to which the current <see cref="InvalidateCacheAttribute"/> aspect is being applied.
    /// </summary>
    /// <param name="methodNames">A list of names of methods to invalidate. All parameters of these methods (except those marked 
    /// with <see cref="NotCacheKeyAttribute"/>) must have a parameter of the same name and compatible type in the target
    /// method of the current <see cref="InvalidateCacheAttribute"/> aspect.</param>
    public InvalidateCacheAttribute( params string[] methodNames ) : this( null!, methodNames ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidateCacheAttribute"/> class  that invalidates method of a different type than the
    /// type to which the current <see cref="InvalidateCacheAttribute"/> aspect is being applied.
    /// </summary>
    /// <param name="declaringType">The type containing the methods to invalidate.</param>
    /// <param name="methodNames">A list of names of methods to invalidate. All parameters of these methods (except those marked 
    /// with <see cref="NotCacheKeyAttribute"/>) must have a parameter of the same name and compatible type in the target
    /// method of the current <see cref="InvalidateCacheAttribute"/> aspect.</param>
    public InvalidateCacheAttribute( Type declaringType, params string[] methodNames )
    {
        this._invalidatedMethodsDeclaringType = declaringType;
        this._invalidatedMethodNames = methodNames;
    }

    public override void BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy( m => !m.ReturnType.GetAsyncInfo().IsAwaitable || m.ReturnType.IsTask(), m => $"awaitable types other than Task and Task<T> are not supported." );
    }

    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        // For each attribute:
        // - Validate attribute properties.
        // - Determine and validate

        var invalidatedMethods = new Dictionary<IMethod, InvalidatedMethodInfo>();

        var isValid = true;

        isValid &= Validate( builder, builder.AspectInstance, this, invalidatedMethods );

        foreach ( var secondaryInstance in builder.AspectInstance.SecondaryInstances )
        {
            isValid &= Validate( builder, secondaryInstance, (InvalidateCacheAttribute) secondaryInstance.Aspect, invalidatedMethods );
        }

        if ( !isValid )
        {
            return;
        }

        if ( invalidatedMethods.Count == 0 )
        {
            throw new MetalamaPatternsCachingAssertionFailedException( "invalidatedMethods.Count == 0 not expected." );
        }

        // TODO: [Porting] Discuss idea of common purposes, exposing etc.
        var logSourceFieldName = builder.Target.DeclaringType.ToSerializableId().MakeAssociatedIdentifier( "Metalama.Patterns/StaticLogSourceForType", "_logSource" );

        var logSourceFieldAdviceResult = builder.Advice.IntroduceField(
            builder.Target.DeclaringType,
            nameof( _logSource ),
            IntroductionScope.Static,
            OverrideStrategy.Ignore,
            b => b.Name = logSourceFieldName,
            tags: new 
            { 
                type = builder.Target.DeclaringType 
            } );

        // TODO: !!! ML - should set Declaration when already exists.
        // Remove this when fixed:
        var logSourceField = logSourceFieldAdviceResult.Outcome == AdviceOutcome.Ignore
            ? builder.Target.DeclaringType.Fields.Single( f => f.Name == logSourceFieldName )
            : logSourceFieldAdviceResult.Declaration;

        var arrayBuilder = new ArrayBuilder( typeof( MethodInfo ) );
        foreach ( var method in invalidatedMethods.Keys )
        {
            // TODO: Use ThrowIfMissing.
            arrayBuilder.Add( method.ToMethodInfo() );
        }

        var methodsInvalidatedByFieldName = builder.Target.ToSerializableId().MakeAssociatedIdentifier( "{3AB07EE4-9AB7-423C-810A-994D9BC620CA}", $"_methodsInvalidatedBy_{builder.Target.Name}" );

        var methodsInvalidatedByField = builder.Advice.IntroduceField(
            builder.Target.DeclaringType,
            methodsInvalidatedByFieldName,
            typeof( MethodInfo[] ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Name = methodsInvalidatedByFieldName;
                b.InitializerExpression = arrayBuilder.ToExpression();
            } );

        var asyncInfo = builder.Target.GetAsyncInfo();

        var templates = new MethodTemplateSelector(
            nameof( this.OverrideMethod ),
            builder.Target.ReturnType.IsTask( withResult: false ) ? nameof( this.OverrideMethodAsyncTask ) : nameof( this.OverrideMethodAsyncTaskOfT ),
            useAsyncTemplateForAnyAwaitable: true );

        builder.Advice.Override(
            builder.Target,
            templates,
            args: new
            {
                TReturn = asyncInfo.IsAwaitable ? asyncInfo.ResultType : builder.Target.ReturnType,
                logSourceField = logSourceField,
                methodsInvalidatedByField = methodsInvalidatedByField.Declaration,
                invalidatedMethods = invalidatedMethods.Values
            } );
    }

    [Template]
    private static readonly LogSource _logSource = LogSource.Get( ((IType) meta.Tags["type"]!).ToTypeOfExpression().Value );
    
    [Template]
    public dynamic OverrideMethod( 
        IField logSourceField, 
        IEnumerable<InvalidatedMethodInfo> invalidatedMethods, 
        IField methodsInvalidatedByField,
        IType TReturn /* not used */ )
    {
        using ( var activity = logSourceField.Value.Default.OpenActivity( 
            FormattedMessageBuilder.Formatted( $"Processing invalidation by method {meta.Target.Method.ToDisplayString()}" ) ) )
        {
            try
            {                
                dynamic result = meta.Proceed();

                var index = meta.CompileTime( 0 );

                foreach ( var invalidatedMethod in invalidatedMethods )
                {
                    CachingServices.Invalidation.Invalidate(
                        methodsInvalidatedByField.Value[index],
                        invalidatedMethod.Method.IsStatic ? null : meta.This,
                        MapArguments( invalidatedMethod ).Value );

                    ++index;
                }

                activity.SetSuccess();

                return result;
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }        
    }

    [Template]
    public async Task<dynamic> OverrideMethodAsyncTaskOfT(
        IField logSourceField,
        IEnumerable<InvalidatedMethodInfo> invalidatedMethods,
        IField methodsInvalidatedByField,
        IType TReturn )
    {
        // TODO: Abstract to RunTime helper where possible.

        // TODO: Automagically accept CancellationToken parameter?

        using ( var activity = logSourceField.Value.Default.OpenActivity(
            FormattedMessageBuilder.Formatted( $"Processing invalidation by method {meta.Target.Method.ToDisplayString()}" ) ) )
        {
            dynamic? result = TReturn.DefaultValue();

            try
            {
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

                var index = meta.CompileTime( 0 );

                Task task2;

                foreach ( var invalidatedMethod in invalidatedMethods )
                {
                    task2 = CachingServices.Invalidation.InvalidateAsync(
                        methodsInvalidatedByField.Value[index],
                        invalidatedMethod.Method.IsStatic ? null : meta.This,
                        MapArguments( invalidatedMethod ).Value );

                    if ( !task2.IsCompleted )
                    {
                        // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                        // and the await instrumentation policy is not applied.
                        activity.Suspend();
                        try
                        {
                            await task;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }

                    ++index;
                }

                activity.SetSuccess();

                return result;
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }
    }

    [Template]
    public async Task OverrideMethodAsyncTask( 
        IField logSourceField, 
        IEnumerable<InvalidatedMethodInfo> invalidatedMethods, 
        IField methodsInvalidatedByField, 
        IType TReturn /* not used */ )
    {
        // TODO: Abstract to RunTime helper where possible.

        // TODO: Automagically accept CancellationToken parameter?

        using ( var activity = logSourceField.Value.Default.OpenActivity(
            FormattedMessageBuilder.Formatted( $"Processing invalidation by method {meta.Target.Method.ToDisplayString()}" ) ) )
        {
            try
            {
                var task = meta.Proceed();

                if ( !task.IsCompleted )
                {
                    // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                    // and the await instrumentation policy is not applied.
                    activity.Suspend();
                    try
                    {
                        await task;
                    }
                    finally
                    {
                        activity.Resume();
                    }
                }
                else
                {
                    // Don't wrap any exception.
                    task.GetAwaiter().GetResult();
                }

                var index = meta.CompileTime( 0 );

                Task task2;

                foreach ( var invalidatedMethod in invalidatedMethods )
                {
                    task2 = CachingServices.Invalidation.InvalidateAsync(
                        methodsInvalidatedByField.Value[index],
                        invalidatedMethod.Method.IsStatic ? null : meta.This,
                        MapArguments( invalidatedMethod ).Value );

                    if ( !task2.IsCompleted )
                    {
                        // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                        // and the await instrumentation policy is not applied.
                        activity.Suspend();
                        try
                        {
                            await task;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }

                    ++index;
                }

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }
    }

    private static IExpression MapArguments( InvalidatedMethodInfo invalidatedMethod )
    {
        var arrayBuilder = new ArrayBuilder();

        for ( var i = 0; i < invalidatedMethod.ParameterMap.Length; i++ )
        {
            var mappedArgumentPosition = invalidatedMethod.ParameterMap[i];
            
            arrayBuilder.Add(
                mappedArgumentPosition >= 0
                    ? invalidatedMethod.Method.Parameters[mappedArgumentPosition]
                    : 0 );
        }

        return arrayBuilder.ToExpression();
    }

    /// <summary>
    /// Validates the given aspect attribute. If valid, adds details of the invalidated methods to <paramref name="invalidatedMethods"/>.
    /// </summary>
    /// <returns><see langword="false"/> if any <see cref="Severity.Error"/> severity diagnostics are reported; otherwise, <see langword="false"/>.</returns>
    private static bool Validate(
        IAspectBuilder<IMethod> builder,
        IAspectInstance aspect,
        InvalidateCacheAttribute attribute,
        Dictionary<IMethod, InvalidatedMethodInfo> invalidatedMethods )
    {
        if ( attribute._invalidatedMethodNames == null || attribute._invalidatedMethodNames.Length == 0 )
        {
            builder.Diagnostics.Report( ErrorInvalidAspectConstructorNoMethodName.WithArguments( builder.Target ) );
            return false;
        }
        
        if ( attribute._invalidatedMethodNames.Any( s => string.IsNullOrWhiteSpace( s ) ) )
        {
            builder.Diagnostics.Report( ErrorInvalidAspectConstructorNullOrWhitespaceString.WithArguments( builder.Target ) );
            return false;
        }

        var invalidatingMethod = builder.Target;
        var invalidatedMethodsDeclaringType = attribute._invalidatedMethodsDeclaringType == null
            ? invalidatingMethod.DeclaringType
            : (INamedType) TypeFactory.GetType( attribute._invalidatedMethodsDeclaringType );

        var invalidatingMethodParameters = invalidatingMethod.Parameters;

        var candidateInvalidatedMethods = invalidatedMethodsDeclaringType.AllMethods;

        DictionaryOfLists<string, IDiagnostic> matchingErrorsDictionary = new();

        DictionaryOfLists<string, InvalidatedMethodInfo?> invalidatedMethodsDictionary = new();

        ; // TODO: Do we see ctors/property methods?

        var isValid = true;

        foreach ( var invalidatedMethod in candidateInvalidatedMethods )
        {
            if ( Array.IndexOf( attribute._invalidatedMethodNames, invalidatedMethod.Name ) == -1 )
            {
                continue;
            }

            if ( invalidatedMethods.ContainsKey( invalidatedMethod ))
            {
                // Already processed: add null so that the later checks based on list count are handled correctly.
                invalidatedMethodsDictionary.Add( invalidatedMethod.Name, null );
                continue;
            }

            // Ensure the method is actually cached.
            var cacheAspect = invalidatedMethod.Enhancements().GetAspects<CacheAttribute>().SingleOrDefault();

            if ( cacheAspect == null )
            {
                matchingErrorsDictionary.Add(
                    invalidatedMethod.Name,
                    ErrorMethodIsNotCached.WithArguments( (builder.Target, invalidatedMethod) ) );

                continue;
            }

            var cacheAspectConfiguration = cacheAspect.GetEffectiveConfiguration( invalidatedMethod );

            // Check that the 'this' parameter is compatible.
            if ( !invalidatedMethod.IsStatic && !cacheAspectConfiguration.IgnoreThisParameter.GetValueOrDefault() &&
                 (invalidatingMethod.IsStatic || !(invalidatingMethod.DeclaringType == invalidatedMethod.DeclaringType || invalidatingMethod.DeclaringType.DerivesFrom( invalidatedMethod.DeclaringType ))) )
            {
                matchingErrorsDictionary.Add(
                    invalidatedMethod.Name,
                    ErrorThisParameterCannotBeMapped.WithArguments( (invalidatingMethod, invalidatedMethod, invalidatingMethod.DeclaringType, invalidatedMethod.DeclaringType) ) );

                continue;
            }

            // Match parameters.
            var invalidatedMethodParameters = invalidatedMethod.Parameters;

            var invalidatedMethodInfo = new InvalidatedMethodInfo( invalidatedMethod );

            var allParametersMatching = true;

            for ( var i = 0; i < invalidatedMethodParameters.Count; i++ )
            {
                var invalidatedMethodParameter = invalidatedMethodParameters[i];
                
                if ( invalidatedMethodParameter.Attributes.Any( typeof( NotCacheKeyAttribute ) ) )
                {
                    continue;
                }

                // Match parameter by name.
                var invalidatingMethodParameter =
                    invalidatingMethodParameters.FirstOrDefault( p => p.Name == invalidatedMethodParameter.Name );

                if ( invalidatingMethodParameter == null )
                {
                    matchingErrorsDictionary.Add(
                        invalidatedMethod.Name,
                        ErrorMissingParameterInInvalidatingMethod.WithArguments( (invalidatedMethod, invalidatedMethod, invalidatedMethodParameter.Name) ) );

                    allParametersMatching = false;
                    continue;
                }

                // Check that the type is compatible.
                if ( !invalidatingMethodParameter.Type.Is( invalidatedMethodParameter.Type ) )
                {
                    matchingErrorsDictionary.Add(
                        invalidatedMethod.Name,
                        ErrorParameterTypeIsNotCompatible.WithArguments( (invalidatingMethod, invalidatedMethod, invalidatedMethodParameter.Name) ) );

                    allParametersMatching = false;
                    continue;
                }

                invalidatedMethodInfo.ParameterMap[i] = invalidatingMethodParameter.Index;
            }

            if ( !allParametersMatching )
            {
                continue;
            }

            invalidatedMethodsDictionary.Add( invalidatedMethod.Name, invalidatedMethodInfo );
        }

        foreach ( var invalidatedMethodName in attribute._invalidatedMethodNames )
        {
            if ( !invalidatedMethodsDictionary.TryGetList( invalidatedMethodName, out var invalidatedOverloads ) || invalidatedOverloads.Count == 0 )
            {
                List<IDiagnostic> diagnostics;

                if ( matchingErrorsDictionary.TryGetList( invalidatedMethodName, out diagnostics ) )
                {
                    // There were diagnostics, but the method of the given name exists
                    foreach ( var diagnostic in diagnostics )
                    {
                        builder.Diagnostics.Report( diagnostic );
                    }                    
                }
                else
                {
                    // The method of the given name does not exist
                    builder.Diagnostics.Report( ErrorCachedMethodNotFound.WithArguments( (invalidatingMethod, invalidatedMethodName, invalidatedMethodsDeclaringType) ) );
                }

                isValid = false;
                continue;
            }

            if ( !attribute.AllowMultipleOverloads && invalidatedOverloads.Count > 1 )
            {
                builder.Diagnostics.Report( ErrorMultipleOverloadsFound.WithArguments( (invalidatingMethod, invalidatedMethodName) ) );

                isValid = false;
                continue;
            }

            foreach ( var invalidatedMethodInfo in invalidatedOverloads )
            {
                // null indicates already processed (eg, via another aspect instance on the current target method)
                if ( invalidatedMethodInfo != null )
                {
                    builder.Diagnostics.Report( InfoMethodIsInvalidatedBy.WithArguments( invalidatingMethod ), invalidatedMethodInfo.Method );
                    builder.Diagnostics.Report( InfoMethodInvalidates.WithArguments( invalidatedMethodInfo.Method ), invalidatingMethod );
                    
                    invalidatedMethods.Add( invalidatedMethodInfo.Method, invalidatedMethodInfo );
                }
            }
        }

        return isValid;
    }

    [CompileTime]
    private class DictionaryOfLists<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, List<TValue>> _collectionsDictionary = new();

        public void Add( TKey methodName, TValue value )
        {
            List<TValue> list;

            if ( !this._collectionsDictionary.TryGetValue( methodName, out list ) )
            {
                list = new List<TValue>();
                this._collectionsDictionary.Add( methodName, list );
            }

            list.Add( value );
        }

        public bool TryGetList( TKey key, out List<TValue> list )
        {
            return this._collectionsDictionary.TryGetValue( key, out list );
        }

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return this._collectionsDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }

    [CompileTime]
    public class InvalidatedMethodInfo
    {
        public InvalidatedMethodInfo( IMethod method )
        {
            this.Method = method;
            this.ParameterMap = new int[method.Parameters.Count];
            for ( var i = 0; i < this.ParameterMap.Length; i++ )
            {
                this.ParameterMap[i] = -1;
            }
        }

        public readonly IMethod Method;
        public readonly int[] ParameterMap;
    }
}