// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Building;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

public class RedisCachingBackendBuilder : DistributedCachingBackendBuilder
{
    private readonly IConnectionMultiplexer _connection;
    private RedisCachingBackendConfiguration? _configuration;

    public RedisCachingBackendBuilder( IConnectionMultiplexer connection, RedisCachingBackendConfiguration? configuration )
    {
        this._connection = connection;
        this._configuration = configuration;
    }

    public RedisCachingBackendBuilder WithConfiguration( RedisCachingBackendConfiguration configuration )
    {
        this._configuration = configuration;

        return this;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        this._configuration ??= new RedisCachingBackendConfiguration();

        if ( args.Layer != 1 )
        {
            // #20775 Caching: two-layered cache should modify the key to avoid conflicts when toggling the option
            var prefixSuffix = $"L{args.Layer}";

            this._configuration = this._configuration with
            {
                KeyPrefix = this._configuration.KeyPrefix != null ? this._configuration.KeyPrefix + prefixSuffix : prefixSuffix
            };
        }

        return this._configuration.SupportsDependencies
            ? new DependenciesRedisCachingBackend( this._connection, this._configuration, args.ServiceProvider )
            : new RedisCachingBackend( this._connection, this._configuration, args.ServiceProvider );
    }
}