// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.Building;

#pragma warning disable CA1001

/// <summary>
/// A <see cref="CachingBackendBuilder"/> that returns an in-memory, in-process caching back-end, backed
/// by an <see cref="IMemoryCache"/>.
/// </summary>
public sealed class MemoryCachingBackendBuilder : ConcreteCachingBackendBuilder
{
    private IMemoryCache? _memoryCache;
    private MemoryCachingBackendConfiguration? _configuration;

    internal MemoryCachingBackendBuilder( MemoryCachingBackendConfiguration? configuration )
    {
        this._configuration = configuration;
    }

    /// <summary>
    /// Specifies the <see cref="IMemoryCache"/> to use.
    /// </summary>
    public MemoryCachingBackendBuilder WithMemoryCache( IMemoryCache memoryCache )
    {
        this._memoryCache = memoryCache;

        return this;
    }

    /// <summary>
    /// Specifies the options of the <see cref="MemoryCache"/>. This method is ignored if the <see cref="WithMemoryCache"/>
    /// method is called.
    /// </summary>
    public MemoryCachingBackendBuilder WithMemoryCacheOptions( MemoryCacheOptions memoryCacheOptions )
    {
        this._memoryCache = new MemoryCache( memoryCacheOptions );

        return this;
    }

    /// <summary>
    /// Specifies the configuration of the in-memory caching back-end.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public MemoryCachingBackendBuilder WithConfiguration( MemoryCachingBackendConfiguration configuration )
    {
        this._configuration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
        => new MemoryCachingBackend( this._memoryCache ?? new MemoryCache( new MemoryCacheOptions() ), this._configuration, args.ServiceProvider );
}