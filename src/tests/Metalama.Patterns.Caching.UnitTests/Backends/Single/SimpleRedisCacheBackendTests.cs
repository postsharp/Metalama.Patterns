// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

#if !ENABLE_FLAKY_REDIS_TESTS
public class SimpleRedisCacheBackendTests
{
    [Fact( Skip = "#33990: Skipping all tests in class SimpleRedisCacheBackendTests" )]
    public void PlaceholderForInheritedTestsInCommonBaseClass()
    { }
}
#else
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using Xunit.Abstractions;
// ReSharper disable once UnusedType.Global
public class SimpleRedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public SimpleRedisCacheBackendTests(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        ITestOutputHelper testOutputHelper ) : base(
        cachingTestOptions,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
    }

    protected override bool TestDependencies => false;

    protected override CheckAfterDisposeCachingBackend CreateBackend() => Task.Run( this.CreateBackendAsync ).Result;

    protected override async Task<CheckAfterDisposeCachingBackend> CreateBackendAsync()
    {
        return await this.CreateBackendAsync( null );
    }

    private async Task<CheckAfterDisposeCachingBackend> CreateBackendAsync( string? keyPrefix )
    {
        return new CheckAfterDisposeCachingBackend(
            await RedisFactory.CreateBackendAsync( this.TestOptions, this._redisSetupFixture, keyPrefix, serviceProvider: this.ServiceProvider ) );
    }
}
#endif