// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Building;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

[PublicAPI]
public static class RedisCachingBackendFactory
{
    public static RedisCachingBackendBuilder Redis(
        this CachingBackendBuilder builder,
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration? configuration = null )
        => new( connection, configuration );

    public static RedisInvalidatedCachingBackendBuilder WithRedisInvalidator(
        this MemoryCachingBackendBuilder builder,
        IConnectionMultiplexer connection )
        => new( builder, connection );

    /// <summary>
    /// Adds a service (implementing <see cref="IHostedService"/>) that removes dependencies added when a <see cref="RedisCachingBackend"/> when items
    /// are expired or evicted from the cache. At least one instance (ideally a single instance) of the <see cref="RedisCacheDependencyGarbageCollector"/> must
    /// be running whenever a Redis caching back-end instance that supports dependencies is running, otherwise the cache will use storage to store dependencies
    /// that are no longer relevant, and will not be removed otherwise. If no instance of this service is running during some time,
    /// you must initiate full garbage collection by calling the <see cref="RedisCachingBackendUtilities.PerformFullCollectionAsync(StackExchange.Redis.IConnectionMultiplexer,Metalama.Patterns.Caching.Backends.Redis.RedisCachingBackendConfiguration?,System.IServiceProvider?,System.Threading.CancellationToken)"/> method.
    /// </summary>
    public static IServiceCollection AddRedisCacheDependencyGarbageCollector(
        this IServiceCollection serviceCollection,
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration configuration )
    {
        serviceCollection.AddHostedService<RedisCacheDependencyGarbageCollector>(
            serviceProvider => new RedisCacheDependencyGarbageCollector( connection, configuration, serviceProvider ) );

        return serviceCollection;
    }

    /// <summary>
    /// Creates an instance of the garbage collector service described in <see cref="AddRedisCacheDependencyGarbageCollector"/>.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="configuration"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static IAsyncDisposable CreateRedisCacheDependencyGarbageCollector(
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration? configuration = null,
        IServiceProvider? serviceProvider = null )
        => new RedisCacheDependencyGarbageCollector( connection, configuration ?? new RedisCachingBackendConfiguration(), serviceProvider );
}