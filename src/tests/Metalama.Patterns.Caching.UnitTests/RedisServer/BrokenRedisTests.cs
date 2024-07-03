// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using StackExchange.Redis;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.RedisServer;

public sealed class BrokenRedisTests
{
    [Fact( Timeout = 20000 )]
    public async Task TestWeAbortConnection()
    {
        try
        {
            var connection = CreateConnection( false );

            var configuration =
                new RedisCachingBackendConfiguration
                {
                    Connection = connection,
                    KeyPrefix = Guid.NewGuid().ToString(),
                    OwnsConnection = true,
                    SupportsDependencies = false,
                    ConnectionTimeout = TimeSpan.FromMilliseconds( 10 )
                };

            await using var backend = CachingBackend.Create( b => b.Redis( configuration ) );

            using var cancellation = new CancellationTokenSource( 20 );

            await backend.InitializeAsync( cancellation.Token );

            Assert.Fail( "A OperationCanceledException was expected but we got no exception." );
        }
        catch ( OperationCanceledException ) { }
        catch ( Exception e )
        {
            Assert.Fail( $"A OperationCanceledException was expected but we got {e.GetType()}." );
        }

        // Make sure there are no deadlocks in finalizers.
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [Fact( Timeout = 20000 )]
    public void TestRedisAbortsConnection()
    {
        AssertEx.Throws<RedisConnectionException>( () => _ = CreateConnection( true ) );
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