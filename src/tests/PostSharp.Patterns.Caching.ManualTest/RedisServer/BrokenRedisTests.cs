// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial source-available license. Please see the LICENSE.md file in the repository root for details.
#if POSTSHARP_CACHING_REDIS

using System;
using System.Net;
using Xunit;
using PostSharp.Patterns.Caching.Backends.Redis;
using PostSharp.Patterns.Caching.Tests.Backends.Distributed;
using PostSharp.Patterns.Common.Tests.Helpers;
using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Tests
{
    public class BrokenRedisTests
    {
        [Xunit.Fact(Timeout = 20000)]
        public void TestWeAbortConnection()
        {
            AssertEx.Throws<TimeoutException>( () =>
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

                 IConnectionMultiplexer connection = CreateConnection( false );
                 RedisCachingBackend.Create( connection, configuration );
             } );
        }

        [Fact(Timeout = 20000 )]
        public void TestRedisAbortsConnection()
        {
            AssertEx.Throws<RedisConnectionException>( () =>
            {
                IConnectionMultiplexer connection = CreateConnection( true );
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
}
#endif