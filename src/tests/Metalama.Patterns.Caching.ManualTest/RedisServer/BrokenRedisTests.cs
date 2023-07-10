// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using System;
using System.Net;
using Xunit;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Common.Tests.Helpers;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Tests;

public class BrokenRedisTests
{
    [Fact( Timeout = 20000 )]
    public void TestWeAbortConnection()
    {
        AssertEx.Throws<TimeoutException>(
            () =>
            {
                RedisCachingBackendConfiguration configuration =
                    new RedisCachingBackendConfiguration
                    {
                        KeyPrefix = Guid.NewGuid().ToString(),
                        OwnsConnection = true,
                        SupportsDependencies = false,
                        IsLocallyCached = false,
                        ConnectionTimeout = TimeSpan.FromMilliseconds( 10 )
                    };

                IConnectionMultiplexer connection = this.CreateConnection( false );
                RedisCachingBackend.Create( connection, configuration );
            } );
    }

    [Fact( Timeout = 20000 )]
    public void TestRedisAbortsConnection()
    {
        AssertEx.Throws<RedisConnectionException>(
            () =>
            {
                IConnectionMultiplexer connection = this.CreateConnection( true );
            } );
    }

    private IConnectionMultiplexer CreateConnection( bool redisAborts )
    {
        SocketManager socketManager = new SocketManager( "BrokenTest" );

        ConfigurationOptions redisConfigurationOptions = new ConfigurationOptions();
        redisConfigurationOptions.EndPoints.Add( "192.168.45.127:12345" );
        redisConfigurationOptions.AbortOnConnectFail = redisAborts;
        redisConfigurationOptions.ConnectTimeout = 10;
        redisConfigurationOptions.SocketManager = socketManager;

        ConnectionMultiplexer connection = ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );

        return new DisposingConnectionMultiplexer( connection, socketManager );
    }
}