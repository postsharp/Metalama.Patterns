// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

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