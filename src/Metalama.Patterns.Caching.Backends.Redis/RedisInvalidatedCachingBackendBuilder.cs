// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Building;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

public sealed class RedisInvalidatedCachingBackendBuilder : BuiltCachingBackendBuilder
{
    private readonly MemoryCachingBackendBuilder _underlying;
    private readonly IConnectionMultiplexer _connection;
    private RedisCacheInvalidatorConfiguration? _configuration;

    internal RedisInvalidatedCachingBackendBuilder( MemoryCachingBackendBuilder underlying, IConnectionMultiplexer connection )
    {
        this._underlying = underlying;
        this._connection = connection;
    }

    public RedisInvalidatedCachingBackendBuilder WithConfiguration( RedisCacheInvalidatorConfiguration configuration )
    {
        this._configuration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        var underlying = this._underlying.CreateBackend( args );

        return new RedisCacheInvalidator( underlying, this._connection, this._configuration ?? new RedisCacheInvalidatorConfiguration() );
    }
}