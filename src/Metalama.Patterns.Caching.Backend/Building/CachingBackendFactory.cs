// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

[PublicAPI]
public static class CachingBackendFactory
{
    public static UninitializedCachingBackendBuilder Uninitialized( this CachingBackendBuilder builder ) => new();

    public static MemoryCachingBackendBuilder Memory( this CachingBackendBuilder builder, MemoryCachingBackendConfiguration? configuration = null )
        => new( configuration );

    public static NonBlockingCachingBackendBuilder NonBlocking( this DistributedCachingBackendBuilder builder ) => new( builder );

    public static LayeredCachingBackendBuilder WithLocalLayer( this DistributedCachingBackendBuilder builder ) => new( builder );

    [Obsolete( "Adding a memory cache on the top of another memory cache should only be used in tests." )]
    public static LayeredCachingBackendBuilder WithLocalLayer( this MemoryCachingBackendBuilder builder ) => new( builder );

    public static BuiltCachingBackendBuilder Specific( this CachingBackendBuilder builder, CachingBackend backend )
        => new SpecificCachingBackendBuilder( _ => backend );
}