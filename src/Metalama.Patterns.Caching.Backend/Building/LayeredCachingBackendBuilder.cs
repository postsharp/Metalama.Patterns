// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.Building;

#pragma warning disable CA1001

/// <summary>
/// A <see cref="CachingBackendBuilder"/> that adds an in-memory L1 cache in front a another, typically out-of-process,
/// cache.
/// </summary>
public sealed class LayeredCachingBackendBuilder : ConcreteCachingBackendBuilder
{
    private readonly ConcreteCachingBackendBuilder _underlying;

    private IMemoryCache? _memoryCache;
    private MemoryCachingBackendConfiguration? _memoryCacheConfiguration;

    internal LayeredCachingBackendBuilder( ConcreteCachingBackendBuilder underlying, IServiceProvider? serviceProvider ) : base( serviceProvider )
    {
        this._underlying = underlying;
    }

    public LayeredCachingBackendBuilder WithMemoryCache( IMemoryCache memoryCache )
    {
        this._memoryCache = memoryCache;

        return this;
    }

    public LayeredCachingBackendBuilder WithMemoryCacheOptions( MemoryCacheOptions memoryCacheOptions )
    {
        this._memoryCache = new MemoryCache( memoryCacheOptions );

        return this;
    }

    public LayeredCachingBackendBuilder WithLocalCacheConfiguration( MemoryCachingBackendConfiguration configuration )
    {
        this._memoryCacheConfiguration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        // ReSharper disable once WithExpressionModifiesAllMembers
        var underlying = this._underlying.CreateBackend( args with { Layer = args.Layer + 1 } );

        // We always add a non-blocking modifier to a layered caching because it improves latency without worsening
        // the cross-node cache consistency which, anyway, is not guaranteed.
        // There is no other way for the user to enable non-blocking operations because using it without a local layer
        // almost guarantees to cause cache consistency issues within the same application node, which is a nightmare 
        // to avoid.

        var nonBlocking = new NonBlockingCachingBackendEnhancer( underlying );

        var memoryCache = new MemoryCachingBackend( this._memoryCache, this._memoryCacheConfiguration, this.ServiceProvider );

        return new TwoLayerCachingBackendEnhancer( nonBlocking, memoryCache );
    }
}