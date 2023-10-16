// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Building;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Builds a <see cref="CachingBackend"/> that relies on a Redis server.
/// </summary>
[PublicAPI]
public sealed class RedisCachingBackendBuilder : OutOfProcessCachingBackendBuilder
{
    private RedisCachingBackendConfiguration _configuration;

    internal RedisCachingBackendBuilder(
        RedisCachingBackendConfiguration configuration,
        IServiceProvider? serviceProvider ) : base( serviceProvider )
    {
        this._configuration = configuration;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
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
            var backend = new DependenciesRedisCachingBackend( this._configuration, this.ServiceProvider );

            if ( this._configuration.RunGarbageCollector )
            {
                // This conveniently binds the lifetime of the collector with the one of the back-end.
                backend.Collector = new RedisCacheDependencyGarbageCollector( backend );
            }

            return backend;
        }
        else
        {
            return new RedisCachingBackend( this._configuration, this.ServiceProvider );
        }
    }
}