// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Building;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.RedisServer;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

internal static class RedisFactory
{
    public static RedisTestInstance? CreateTestInstance( CachingTestOptions cachingTestOptions, RedisSetupFixture redisSetupFixture )
    {
        RedisTestInstance? redisTestInstance = null;

        if ( cachingTestOptions.Endpoint == null )
        {
            redisTestInstance = redisSetupFixture.TestInstance;
            cachingTestOptions.Endpoint = redisTestInstance.Endpoint;
        }

        return redisTestInstance;
    }

    public static IConnectionMultiplexer CreateConnection( CachingTestOptions cachingTestOptions )
    {
        var endPoint = cachingTestOptions.Endpoint
                       ?? throw new ArgumentOutOfRangeException( nameof(cachingTestOptions), "The Endpoint property is null." );

        var socketManager = new SocketManager();

        var redisConfigurationOptions = new ConfigurationOptions();

        redisConfigurationOptions.EndPoints.Add( endPoint );
        redisConfigurationOptions.AbortOnConnectFail = false;
        redisConfigurationOptions.SocketManager = socketManager;

        return ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );
    }

    public static async Task<CheckAfterDisposeCachingBackend> CreateBackendAsync(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool collector = false,
        IServiceProvider? serviceProvider = null,
        bool locallyCached = false )
    {
        _ = CreateTestInstance( cachingTestOptions, redisSetupFixture );

        var connection = CreateConnection( cachingTestOptions );

        var configuration =
            new RedisCachingBackendConfiguration
            {
                KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
                Connection = connection,
                SupportsDependencies = supportsDependencies,
                RunGarbageCollector = collector
            };

        var backend = CachingBackend.Create(
            b =>
            {
                var redis = (OutOfProcessCachingBackendBuilder) b.Redis( configuration );

                if ( locallyCached )
                {
                    return redis.WithL1();
                }
                else
                {
                    return redis;
                }
            },
            serviceProvider );

        await backend.InitializeAsync();

        return new CheckAfterDisposeCachingBackend( backend );
    }
}