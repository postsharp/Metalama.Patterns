// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using Xunit;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Backends.Redis;
using StackExchange.Redis;
using System.Threading;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Tests;
using PostSharp.Patterns.Caching.Tests.Backends.Distributed;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public class SimpleRedisCacheBackendTests : BaseCacheBackendTests
    {
        public SimpleRedisCacheBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
        }

        protected override bool TestDependencies { get; } = false;
        
        protected override CachingBackend CreateBackend()
        {
            return this.CreateBackend( null );
        }

        protected override async Task<CachingBackend> CreateBackendAsync()
        {
            return await this.CreateBackendAsync( null );
        }

        private DisposingConnectionMultiplexer CreateConnection()
        {
            return RedisFactory.CreateConnection( this.TestContext );
        }

        private CachingBackend CreateBackend( string keyPrefix )
        {
            return RedisFactory.CreateBackend( this.TestContext, keyPrefix );
        }

        private async Task<CachingBackend> CreateBackendAsync( string keyPrefix )
        {
            return await RedisFactory.CreateBackendAsync( this.TestContext, keyPrefix );
        }

        private string GeneratePrefix()
        {
            string keyPrefix = Guid.NewGuid().ToString();

            //Assert.False( this.connection.GetEndPoints().Select( endpoint => this.connection.GetServer( endpoint ) ).Any(
            //                    server => server.Keys( pattern: keyPrefix + ":*" ).Any() ) );

            return keyPrefix;
        }

    }
}