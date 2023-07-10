// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

public class SimpleRedisDistributedTest : BaseDistributedCacheTests
{
    private RedisSetupFixture _redisSetupFixture;

    public SimpleRedisDistributedTest( TestContext testContext, RedisSetupFixture redisSetupFixture ) : base( testContext )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override bool TestDependencies { get; } = false;

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix ), await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[] { RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, prefix ), RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, prefix ) };
    }

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = this._redisSetupFixture.TestInstance;
        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
    }
}