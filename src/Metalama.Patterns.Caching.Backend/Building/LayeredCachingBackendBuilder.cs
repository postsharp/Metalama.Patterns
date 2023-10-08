// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.Building;

public sealed class LayeredCachingBackendBuilder : BuiltCachingBackendBuilder
{
    private readonly BuiltCachingBackendBuilder _underlying;

    private IMemoryCache? _memoryCache;
    private MemoryCachingBackendConfiguration? _memoryCacheConfiguration;

    internal LayeredCachingBackendBuilder( BuiltCachingBackendBuilder underlying )
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
        var underlying = this._underlying.CreateBackend( args with { Layer = args.Layer + 1 } );
        var memoryCache = new MemoryCachingBackend( this._memoryCache, this._memoryCacheConfiguration, args.ServiceProvider );

        return new TwoLayerCachingBackendEnhancer( underlying, memoryCache );
    }
}