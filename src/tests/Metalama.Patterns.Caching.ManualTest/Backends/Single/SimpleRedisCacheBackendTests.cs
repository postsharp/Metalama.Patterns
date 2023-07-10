// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.TestHelpers.Backends;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Metalama.Patterns.Caching.ManualTest.Backends;

public class SimpleRedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public SimpleRedisCacheBackendTests( TestContext testContext, RedisSetupFixture redisSetupFixture ) : base( testContext )
    {
        this._redisSetupFixture = redisSetupFixture;
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
        return RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, keyPrefix );
    }

    private async Task<CachingBackend> CreateBackendAsync( string keyPrefix )
    {
        return await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, keyPrefix );
    }

    private string GeneratePrefix()
    {
        var keyPrefix = Guid.NewGuid().ToString();

        //Assert.False( this.connection.GetEndPoints().Select( endpoint => this.connection.GetServer( endpoint ) ).Any(
        //                    server => server.Keys( pattern: keyPrefix + ":*" ).Any() ) );

        return keyPrefix;
    }
}