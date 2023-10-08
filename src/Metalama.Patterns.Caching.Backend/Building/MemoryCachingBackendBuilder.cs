// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.Building;

public sealed class MemoryCachingBackendBuilder : BuiltCachingBackendBuilder
{
    private IMemoryCache? _memoryCache;
    private MemoryCachingBackendConfiguration? _configuration;

    internal MemoryCachingBackendBuilder( MemoryCachingBackendConfiguration? configuration )
    {
        this._configuration = configuration;
    }

    public MemoryCachingBackendBuilder WithMemoryCache( IMemoryCache memoryCache )
    {
        this._memoryCache = this._memoryCache;

        return this;
    }

    public MemoryCachingBackendBuilder WithMemoryCacheOptions( MemoryCacheOptions memoryCacheOptions )
    {
        this._memoryCache = new MemoryCache( memoryCacheOptions );

        return this;
    }

    public MemoryCachingBackendBuilder WithConfiguration( MemoryCachingBackendConfiguration configuration )
    {
        this._configuration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
        => new MemoryCachingBackend( this._memoryCache ?? new MemoryCache( new MemoryCacheOptions() ), this._configuration, args.ServiceProvider );
}