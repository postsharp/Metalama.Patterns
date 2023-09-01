// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;
using System.Reflection;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Encapsulates information about a method being cached. This cached is used by the implementation of <see cref="ICachingService"/>
/// and you can use it if you override the <see cref="CacheKeyBuilder"/> class.
/// </summary>
[PublicAPI]
public sealed partial class CachedMethodMetadata
{
    private static int _nextId;

    internal int Id { get; } = Interlocked.Increment( ref _nextId );

    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the method.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// Gets an array of <see cref="Parameter"/>.
    /// </summary>
    private ImmutableArray<Parameter> Parameters { get; }

    /// <summary>
    /// Gets a value indicating whether the return type of the method can be <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="ReturnValueCanBeNull"/> is only concerned with whether an instance of the type can be represented
    /// by <see langword="null"/>. For example, primitives like <see cref="int"/> and other non-nullable structs cannot
    /// be represented by <see langword="null"/>.
    /// </remarks>
    internal bool ReturnValueCanBeNull { get; }

    /// <summary>
    /// Gets the build time configuration that applies to the method.
    /// </summary>
    /// <remarks>
    /// This already takes account of the any configuration custom attribute applied to parent classes and the assembly.
    /// </remarks>
    internal CacheItemConfiguration BuildTimeConfiguration { get; }

    /// <summary>
    /// Gets the awaitable result type for awaitable (eg, async) methods, or <see langword="null"/> where not applicable.
    /// </summary>
    /// <remarks>
    /// For example, for a method returning <see cref="Task{TResult}"/>, this would be the generic argument corresponding to <c>TResult</c>.
    /// </remarks>
    internal Type? AwaitableResultType { get; }

    public bool IgnoreThisParameter => this.BuildTimeConfiguration.IgnoreThisParameter.GetValueOrDefault( false );

    public bool IsParameterIgnored( int index ) => this.Parameters[index].IsIgnored;

    private CachedMethodMetadata(
        MethodInfo method,
        ImmutableArray<Parameter> parameters,
        Type? awaitableResultType,
        CacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        this.Method = method;
        this.Parameters = parameters;
        this.BuildTimeConfiguration = buildTimeConfiguration.AsCacheItemConfiguration();
        this.ReturnValueCanBeNull = returnValueCanBeNull;
        this.AwaitableResultType = awaitableResultType;
    }

    public static CachedMethodMetadata Register(
        MethodInfo method,
        CacheItemConfiguration buildTimeConfiguration,
        Type? awaitableResultType,
        bool returnValueCanBeNull )
    {
        var metadata = new CachedMethodMetadata(
            method,
            GetCachedParameterInfos( method ),
            awaitableResultType,
            buildTimeConfiguration,
            returnValueCanBeNull );

        CachedMethodMetadataRegistry.Instance.Register( metadata );

        return metadata;
    }

    private static ImmutableArray<Parameter> GetCachedParameterInfos( MethodInfo method )
    {
        var parameterInfos = method.GetParameters();
        var cachedParameterInfos = new Parameter[parameterInfos.Length];

        for ( var i = 0; i < parameterInfos.Length; i++ )
        {
            var isIgnored = parameterInfos[i].IsDefined( typeof(NotCacheKeyAttribute) );

            cachedParameterInfos[i] = new Parameter( isIgnored );
        }

        return cachedParameterInfos.ToImmutableArray();
    }
}