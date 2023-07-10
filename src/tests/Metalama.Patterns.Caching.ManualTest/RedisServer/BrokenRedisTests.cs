// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using StackExchange.Redis;
using System;
using Xunit;

namespace Metalama.Patterns.Caching.ManualTest;

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