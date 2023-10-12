// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Building;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// A <see cref="CachingBackendBuilder"/> that synchronizes the underlying in-memory cache thanks to Redis Pub/Sub.
/// </summary>
[PublicAPI]
public sealed class RedisCacheSynchronizerBuilder : ConcreteCachingBackendBuilder
{
    private readonly MemoryCachingBackendBuilder _underlying;
    private readonly IConnectionMultiplexer _connection;
    private RedisCacheSynchronizerConfiguration? _configuration;

    internal RedisCacheSynchronizerBuilder( MemoryCachingBackendBuilder underlying, IConnectionMultiplexer connection, RedisCacheSynchronizerConfiguration? configuration )
    {
        this._underlying = underlying;
        this._connection = connection;
        this._configuration = configuration;
    }

    /// <summary>
    /// Specifies the configuration of the synchronization component.
    /// </summary>
    public RedisCacheSynchronizerBuilder WithConfiguration( RedisCacheSynchronizerConfiguration configuration )
    {
        this._configuration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        var underlying = this._underlying.CreateBackend( args );

        return new RedisCacheSynchronizer( underlying, this._connection, this._configuration ?? new RedisCacheSynchronizerConfiguration() );
    }
}