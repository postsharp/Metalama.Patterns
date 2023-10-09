// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

public static class RedisCachingBackendUtilities
{
    /// <summary>
    /// Performs a full garbage collection on all Redis servers. This operation enumerates and validates all keys in the database, and can possibly last several
    /// minutes and affect performance in production.
    /// </summary>
    public static Task PerformFullCollectionAsync(
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration? configuration = null,
        IServiceProvider? serviceProvider = null,
        CancellationToken cancellationToken = default )
    {
        configuration ??= new RedisCachingBackendConfiguration() { SupportsDependencies = true };
        var backend = new DependenciesRedisCachingBackend( connection, configuration, serviceProvider );

        return backend.CleanUpAsync( cancellationToken );
    }

    /// <summary>
    /// Performs a full garbage collection on a given Redis server. This operation enumerates and validates all keys in the database, and can possibly last several
    /// minutes and affect performance in production.
    /// </summary>
    public static Task PerformFullCollectionAsync(
        IServer server,
        RedisCachingBackendConfiguration? configuration = null,
        IServiceProvider? serviceProvider = null,
        CancellationToken cancellationToken = default )
    {
        configuration ??= new RedisCachingBackendConfiguration() { SupportsDependencies = true };
        var backend = new DependenciesRedisCachingBackend( server.Multiplexer, configuration, serviceProvider );

        return backend.CleanUpAsync( server, cancellationToken );
    }
}