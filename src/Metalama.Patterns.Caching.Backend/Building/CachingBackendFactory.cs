// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.Building;

[PublicAPI]
public static class CachingBackendFactory
{
    /// <summary>
    /// Creates a <see cref="CachingBackend"/> that throws an exception whenever it is used.
    /// </summary>
    public static ConcreteCachingBackendBuilder Uninitialized( this CachingBackendBuilder builder ) => new UninitializedCachingBackendBuilder();

    /// <summary>
    /// Creates an in-memory caching backend based on a <see cref="MemoryCache"/>.
    /// </summary>
    public static MemoryCachingBackendBuilder Memory( this CachingBackendBuilder builder, MemoryCachingBackendConfiguration? configuration = null )
        => new( configuration );

    /// <summary>
    /// Modifies a caching back-end to make its write operations non-blocking.
    /// </summary>
    public static NonBlockingCachingBackendBuilder NonBlocking( this OutOfProcessCachingBackendBuilder builder ) => new( builder );

    /// <summary>
    /// Adds an in-memory, in-process L1 cache in front of an out-of-process cache.
    /// </summary>
    public static LayeredCachingBackendBuilder WithLocalLayer( this OutOfProcessCachingBackendBuilder builder ) => new( builder );

    /// <summary>
    /// Adds an in-memory, in-process L1 cache in front of another in-memory back-end. This method is used for tests.
    /// </summary>
    [Obsolete( "Adding a memory cache on the top of another memory cache should only be used in tests." )]
    public static LayeredCachingBackendBuilder WithLocalLayer( this MemoryCachingBackendBuilder builder ) => new( builder );

    /// <summary>
    /// Creates a <see cref="CachingBackendBuilder"/> that returns a specific instance of the <see cref="CachingBackend"/> class.
    /// This method is used in tests.
    /// </summary>
    public static ConcreteCachingBackendBuilder Specific( this CachingBackendBuilder builder, CachingBackend backend )
        => new SpecificCachingBackendBuilder( _ => backend );

    /// <summary>
    /// Creates a <see cref="CachingBackendBuilder"/> that returns an instance of the <see cref="CachingBackend"/> class that does not do anything.
    /// </summary>
    public static ConcreteCachingBackendBuilder Null( this CachingBackendBuilder builder ) => builder.Specific( new NullCachingBackend() );
}