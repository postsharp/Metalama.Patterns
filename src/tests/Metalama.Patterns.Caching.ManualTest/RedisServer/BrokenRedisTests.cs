// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using StackExchange.Redis;
using Xunit;

namespace Metalama.Patterns.Caching.ManualTest.RedisServer;

public sealed class BrokenRedisTests
{
    [Fact( Timeout = 20000 )]
    public void TestWeAbortConnection()
    {
        AssertEx.Throws<TimeoutException>(
            () =>
            {
                var configuration =
                    new RedisCachingBackendConfiguration
                    {
                        KeyPrefix = Guid.NewGuid().ToString(),
                        OwnsConnection = true,
                        SupportsDependencies = false,
                        IsLocallyCached = false,
                        ConnectionTimeout = TimeSpan.FromMilliseconds( 10 )
                    };

                var connection = CreateConnection( false );
                RedisCachingBackend.Create( connection, configuration );
            } );
    }

    [Fact( Timeout = 20000 )]
    public void TestRedisAbortsConnection()
    {
        AssertEx.Throws<RedisConnectionException>(
            () => _ = CreateConnection( true ) );
    }

    private static IConnectionMultiplexer CreateConnection( bool redisAborts )
    {
        var socketManager = new SocketManager( "BrokenTest" );

        var redisConfigurationOptions = new ConfigurationOptions();
        redisConfigurationOptions.EndPoints.Add( "192.168.45.127:12345" );
        redisConfigurationOptions.AbortOnConnectFail = redisAborts;
        redisConfigurationOptions.ConnectTimeout = 10;
        redisConfigurationOptions.SocketManager = socketManager;

        var connection = ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );

        return new DisposingConnectionMultiplexer( connection, socketManager );
    }
}