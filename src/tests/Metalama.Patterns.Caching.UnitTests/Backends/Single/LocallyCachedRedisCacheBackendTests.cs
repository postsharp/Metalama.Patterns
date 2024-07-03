// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Aspects;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

public class LocallyCachedRedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public LocallyCachedRedisCacheBackendTests(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        ITestOutputHelper testOutputHelper ) : base(
        cachingTestOptions,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected virtual bool EnableGarbageCollector => false;

    protected override void Cleanup()
    {
        base.Cleanup();
        AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
    }

    protected override TimeSpan GetExpirationQuantum( double multiplier = 1 )
    {
        return TimeSpan.FromSeconds( 0.1 * multiplier );
    }

    protected override CheckAfterDisposeCachingBackend CreateBackend()
    {
        return Task.Run( this.CreateBackendAsync ).Result;
    }

    protected override async Task<CheckAfterDisposeCachingBackend> CreateBackendAsync()
    {
        return new CheckAfterDisposeCachingBackend(
            await RedisFactory.CreateBackendAsync(
                this.TestOptions,
                this._redisSetupFixture,
                supportsDependencies: true,
                collector: this.EnableGarbageCollector,
                serviceProvider: this.ServiceProvider,
                locallyCached: true ) );
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
    public async Task TestIssue15680()
    {
        var redisKeyPrefix = _testIssue15680 + Guid.NewGuid();

        var testObject = new Issue15680CachingClass();

        CachedValueClass setValue;
        var redisTestInstance = this._redisSetupFixture.TestInstance;

        this.TestOptions.Endpoint = redisTestInstance.Endpoint;

        await using ( this.InitializeTest(
                         "TestIssue15680",
                         await RedisFactory.CreateBackendAsync(
                             this.TestOptions,
                             this._redisSetupFixture,
                             prefix: redisKeyPrefix,
                             locallyCached: false ),
                         b => b.WithProfile( "Issue15680" ) ) )
        {
            setValue = testObject.GetValue();
            Assert.True( testObject.Reset() );
        }

        await using ( this.InitializeTest(
                         "TestIssue15680",
                         await RedisFactory.CreateBackendAsync(
                             this.TestOptions,
                             this._redisSetupFixture,
                             prefix: redisKeyPrefix,
                             locallyCached: true ),
                         b => b.WithProfile( "Issue15680" ) ) )
        {
            var retrievedValue = testObject.GetValue();
            Assert.True( testObject.Reset() );
            Assert.NotEqual( setValue, retrievedValue );
        }
    }

    #endregion

    #region Issue 23499

    [Fact]
    public void TestIssue23499()
    {
        using ( var backend = this.CreateBackend() )
        {
            backend.SetItem( "test", new CacheItem( "Hello, world." ) );
            backend.Clear( ClearCacheOptions.Local );
            backend.GetItem( "test" );
        }
    }

    #endregion

    [Fact( Skip = "https://postsharp.tpondemand.com/entity/33937-test-locallycachedrediscachebackendteststestremovaleventbydependencyasync-is-flaky" )]
    public override Task TestRemovalEventByDependencyAsync() => base.TestRemovalEventByDependencyAsync();
}