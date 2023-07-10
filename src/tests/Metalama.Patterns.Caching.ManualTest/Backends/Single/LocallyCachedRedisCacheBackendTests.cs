// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using System;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.TestHelpers.Shared;
using StackExchange.Redis;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.Backends
{
    public class LocallyCachedRedisCacheBackendTests : BaseCacheBackendTests
    {
        public LocallyCachedRedisCacheBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
        }

        public override TimeSpan GetExpirationTolerance( double multiplier )
        {
            return TimeSpan.FromSeconds( 0.1 * multiplier );
        }


        protected override CachingBackend CreateBackend()
        {
            return RedisFactory.CreateBackend( this.TestContext, supportsDependencies: true, locallyCached: true );
        }


        protected override async Task<CachingBackend> CreateBackendAsync()
        {
            return await RedisFactory.CreateBackendAsync( this.TestContext, supportsDependencies: true, locallyCached: true );
        }

        internal override ITestableCachingComponent CreateCollector( CachingBackend backend )
        {
            return RedisCacheDependencyGarbageCollector.Create( backend );
        }

        #region TestIssue15680

        private const string testIssue15680 = "Issue15680";

        private class Issue15680CachingClass : CachingClass
        {
            [Cache( ProfileName = testIssue15680 )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestIssue15680()
        {
            string redisKeyPrefix = testIssue15680 + Guid.NewGuid();

            Issue15680CachingClass testObject = new Issue15680CachingClass();

            TestProfileConfigurationFactory.InitializeTestWithoutBackend();

            CachedValueClass setValue;
            RedisTestInstance redisTestInstance = RedisPersistentInstance.GetOrLaunchRedisInstance();

            this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;

            using ( CachingServices.DefaultBackend = RedisFactory.CreateBackend( this.TestContext, prefix: redisKeyPrefix, locallyCached: false ) )
            {
                try
                {
                    setValue = testObject.GetValue();
                    Assert.True( testObject.Reset() );
                }
                finally
                {
                    TestProfileConfigurationFactory.DisposeTest();
                }
            }

            using ( CachingServices.DefaultBackend = RedisFactory.CreateBackend( this.TestContext, prefix: redisKeyPrefix, locallyCached: true ) )
            {
                try
                {
                    CachedValueClass retrievedValue = testObject.GetValue();
                    Assert.True( testObject.Reset() );
                    Assert.NotEqual( setValue, retrievedValue );
                }
                finally
                {
                    TestProfileConfigurationFactory.DisposeTest();
                }
            }
        }

        #endregion

        #region Issue 23499
        [Fact]
        public void TestIssue23499()
        {
            using ( DisposingRedisCachingBackend backend = (DisposingRedisCachingBackend) this.CreateBackend() )
            {
                backend.SetItem( "test", new CacheItem( "Hello, world." ) );
                ((TwoLayerCachingBackendEnhancer) backend.UnderlyingBackend).LocalCache.Clear();
                backend.GetItem( "test" );
            }

            
        }
        #endregion
    }
}