// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Building;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.RedisServer;
using StackExchange.Redis;
using System.Net;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

internal static class RedisFactory
{
    public static RedisTestInstance? CreateTestInstance( CachingTestOptions cachingTestOptions, RedisSetupFixture redisSetupFixture )
    {
        RedisTestInstance? redisTestInstance = null;

        if ( !cachingTestOptions.Properties.Contains( "RedisEndpoint" ) )
        {
            redisTestInstance = redisSetupFixture.TestInstance;
            cachingTestOptions.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        return redisTestInstance;
    }

    public static DisposingConnectionMultiplexer CreateConnection( CachingTestOptions cachingTestOptions )
    {
        var socketManager = new SocketManager();

        var redisConfigurationOptions = new ConfigurationOptions();
        redisConfigurationOptions.EndPoints.Add( (EndPoint?) cachingTestOptions.Properties["RedisEndpoint"] );
        redisConfigurationOptions.AbortOnConnectFail = false;
        redisConfigurationOptions.SocketManager = socketManager;

        var connection = ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );

        return new DisposingConnectionMultiplexer( connection, socketManager );
    }

    public static async Task<CheckAfterDisposeCachingBackend> CreateBackendAsync(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool collector = false,
        bool nonBlocking = false,
        IServiceProvider? serviceProvider = null,
        bool locallyCached = false )
    {
        _ = CreateTestInstance( cachingTestOptions, redisSetupFixture );

        var configuration =
            new RedisCachingBackendConfiguration
            {
                KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
                OwnsConnection = true,
                SupportsDependencies = supportsDependencies,
                RunGarbageCollector = collector
            };

        IConnectionMultiplexer connection = CreateConnection( cachingTestOptions );

        var backend = CachingBackend.Create(
            b =>
            {
                var redis = (OutOfProcessCachingBackendBuilder) b.Redis( connection ).WithConfiguration( configuration );

                if ( nonBlocking )
                {
                    redis = redis.NonBlocking();
                }

                if ( locallyCached )
                {
                    return redis.WithLocalLayer();
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