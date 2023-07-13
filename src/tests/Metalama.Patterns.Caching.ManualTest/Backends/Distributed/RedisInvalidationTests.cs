// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

public class RedisInvalidationTests : BaseInvalidationBrokerTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public RedisInvalidationTests( TestContext testContext, RedisSetupFixture redisSetupFixture, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
    {
        return await RedisCacheInvalidator.CreateAsync(
            backend,
            RedisFactory.CreateConnection( this.TestContext ),
            new RedisCacheInvalidatorOptions { Prefix = prefix, OwnsConnection = true } );
    }

    protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
    {
        return RedisCacheInvalidator.Create(
            backend,
            RedisFactory.CreateConnection( this.TestContext ),
            new RedisCacheInvalidatorOptions { Prefix = prefix, OwnsConnection = true } );
    }

    protected override void ConnectToRedisIfRequired()
    {
        RedisFactory.CreateTestInstance( this.TestContext, this._redisSetupFixture );
    }
}