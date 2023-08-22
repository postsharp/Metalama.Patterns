// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.ManualTest.RedisServer;
using Metalama.Patterns.Caching.TestHelpers;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

internal static class RedisFactory
{
    public static RedisTestInstance? CreateTestInstance( TestContext testContext, RedisSetupFixture redisSetupFixture )
    {
        RedisTestInstance? redisTestInstance = null;

        if ( !testContext.Properties.Contains( "RedisEndpoint" ) )
        {
            redisTestInstance = redisSetupFixture.TestInstance;
            testContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        return redisTestInstance;
    }

    public static DisposingRedisCachingBackend CreateBackend(
        TestContext testContext,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false )
    {
        _ = CreateTestInstance( testContext, redisSetupFixture );

        var configuration =
            new RedisCachingBackendConfiguration
            {
                KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
                OwnsConnection = true,
                SupportsDependencies = supportsDependencies,
                IsLocallyCached = locallyCached
            };

        IConnectionMultiplexer connection = CreateConnection( testContext );

        return new DisposingRedisCachingBackend( RedisCachingBackend.Create( connection, configuration ) );
    }

    public static DisposingConnectionMultiplexer CreateConnection( TestContext testContext )
    {
        var socketManager = new SocketManager();

        var redisConfigurationOptions = new ConfigurationOptions();
        redisConfigurationOptions.EndPoints.Add( (EndPoint?) testContext.Properties["RedisEndpoint"] );
        redisConfigurationOptions.AbortOnConnectFail = false;
        redisConfigurationOptions.SocketManager = socketManager;

        var connection = ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );

        return new DisposingConnectionMultiplexer( connection, socketManager );
    }

    public static async Task<DisposingRedisCachingBackend> CreateBackendAsync(
        TestContext testContext,
        RedisSetupFixture redisSetupFixture,
        string? prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false )
    {
        if ( !testContext.Properties.Contains( "RedisEndpoint" ) )
        {
            var redisTestInstance = redisSetupFixture.TestInstance;
            testContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        var configuration = new RedisCachingBackendConfiguration
        {
            KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
            OwnsConnection = true,
            SupportsDependencies = supportsDependencies,
            IsLocallyCached = locallyCached
        };

        return new DisposingRedisCachingBackend( await RedisCachingBackend.CreateAsync( CreateConnection( testContext ), configuration ) );
    }
}