// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Building;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Builds a <see cref="CachingBackend"/> that relies on a Redis server.
/// </summary>
[PublicAPI]
public sealed class RedisCachingBackendBuilder : OutOfProcessCachingBackendBuilder
{
    private readonly IConnectionMultiplexer _connection;
    private RedisCachingBackendConfiguration? _configuration;

    internal RedisCachingBackendBuilder( IConnectionMultiplexer connection, RedisCachingBackendConfiguration? configuration )
    {
        this._connection = connection;
        this._configuration = configuration;
    }

    /// <summary>
    /// Specifies the configuration of the Redis <see cref="CachingBackend"/>.
    /// </summary>
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
                KeyPrefix = this._configuration.KeyPrefix != null ? this._configuration.KeyPrefix + "." + prefixSuffix : prefixSuffix
            };
        }

        if ( this._configuration.SupportsDependencies )
        {
            var backend = new DependenciesRedisCachingBackend( this._connection, this._configuration, args.ServiceProvider );

            if ( this._configuration.RunGarbageCollector )
            {
                // This conveniently binds the lifetime of the collector with the one of the back-end.
                backend.Collector = new RedisCacheDependencyGarbageCollector( backend );
            }

            return backend;
        }
        else
        {
            return new RedisCachingBackend( this._connection, this._configuration, args.ServiceProvider );
        }
    }
}