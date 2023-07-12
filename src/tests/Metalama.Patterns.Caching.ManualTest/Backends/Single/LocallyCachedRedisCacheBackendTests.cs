// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.TestHelpers.Backends;
using Xunit;

namespace Metalama.Patterns.Caching.ManualTest.Backends;

public class LocallyCachedRedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public LocallyCachedRedisCacheBackendTests( TestContext testContext, RedisSetupFixture redisSetupFixture ) : base( testContext )
    {
        this._redisSetupFixture = redisSetupFixture;
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
        return RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, supportsDependencies: true, locallyCached: true );
    }

    protected override async Task<CachingBackend> CreateBackendAsync()
    {
        return await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, supportsDependencies: true, locallyCached: true );
    }

    protected override ITestableCachingComponent CreateCollector( CachingBackend backend )
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
        var redisKeyPrefix = testIssue15680 + Guid.NewGuid();

        var testObject = new Issue15680CachingClass();

        TestProfileConfigurationFactory.InitializeTestWithoutBackend();

        CachedValueClass setValue;
        var redisTestInstance = this._redisSetupFixture.TestInstance;

        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;

        using ( CachingServices.DefaultBackend = RedisFactory.CreateBackend(
                   this.TestContext,
                   this._redisSetupFixture,
                   prefix: redisKeyPrefix,
                   locallyCached: false ) )
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

        using ( CachingServices.DefaultBackend = RedisFactory.CreateBackend(
                   this.TestContext,
                   this._redisSetupFixture,
                   prefix: redisKeyPrefix,
                   locallyCached: true ) )
        {
            try
            {
                var retrievedValue = testObject.GetValue();
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
        using ( var backend = (DisposingRedisCachingBackend) this.CreateBackend() )
        {
            backend.SetItem( "test", new CacheItem( "Hello, world." ) );
            ((TwoLayerCachingBackendEnhancer) backend.UnderlyingBackend).LocalCache.Clear();
            backend.GetItem( "test" );
        }
    }

    #endregion
}