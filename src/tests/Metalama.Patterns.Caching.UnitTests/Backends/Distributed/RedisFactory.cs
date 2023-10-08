// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
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

    public static DisposingRedisCachingBackend CreateBackend(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false,
        IServiceProvider? serviceProvider = null )
    {
        _ = CreateTestInstance( cachingTestOptions, redisSetupFixture );

        var configuration =
            new RedisCachingBackendConfiguration
            {
                KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
                OwnsConnection = true,
                SupportsDependencies = supportsDependencies,
                IsLocallyCached = locallyCached
            };

        IConnectionMultiplexer connection = CreateConnection( cachingTestOptions );

        return new DisposingRedisCachingBackend( RedisCachingBackend.Create( connection, configuration, serviceProvider ) );
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

    public static async Task<DisposingRedisCachingBackend> CreateBackendAsync(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false )
    {
        if ( !cachingTestOptions.Properties.Contains( "RedisEndpoint" ) )
        {
            var redisTestInstance = redisSetupFixture.TestInstance;
            cachingTestOptions.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        var configuration = new RedisCachingBackendConfiguration
        {
            KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
            OwnsConnection = true,
            SupportsDependencies = supportsDependencies,
            IsLocallyCached = locallyCached
        };

        return new DisposingRedisCachingBackend(
            await RedisCachingBackend.CreateAsync( CreateConnection( cachingTestOptions ), configuration: configuration ) );
    }
}