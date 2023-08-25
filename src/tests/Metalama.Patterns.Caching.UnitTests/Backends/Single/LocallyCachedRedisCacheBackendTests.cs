// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Single;

#pragma warning disable SA1124
#pragma warning disable LAMA0048

public sealed class LocallyCachedRedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public LocallyCachedRedisCacheBackendTests(
        TestContext testContext,
        RedisSetupFixture redisSetupFixture,
        ITestOutputHelper testOutputHelper ) : base(
        testContext,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
    }

    protected override TimeSpan GetExpirationQuantum( double multiplier = 1 )
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

#pragma warning disable SA1203
    private const string _testIssue15680 = "Issue15680";
#pragma warning restore SA1203

    private sealed class Issue15680CachingClass : CachingClass
    {
        [Cache( ProfileName = _testIssue15680 )]
        public override CachedValueClass GetValue()
        {
            return base.GetValue();
        }
    }

    [Fact]
    public void TestIssue15680()
    {
        var redisKeyPrefix = _testIssue15680 + Guid.NewGuid();

        var testObject = new Issue15680CachingClass();

        TestProfileConfigurationFactory.InitializeTestWithoutBackend();

        CachedValueClass setValue;
        var redisTestInstance = this._redisSetupFixture.TestInstance;

        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;

        using ( CachingServices.Default.DefaultBackend = RedisFactory.CreateBackend(
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

        using ( CachingServices.Default.DefaultBackend = RedisFactory.CreateBackend(
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