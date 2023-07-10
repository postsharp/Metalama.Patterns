﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;

namespace Metalama.Patterns.Caching.ManualTest.Backends;

public class RedisDistributedCachingBackendTests : BaseDistributedCacheTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public RedisDistributedCachingBackendTests( TestContext testContext, RedisSetupFixture redisSetupFixture ) : base( testContext ) 
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true ),
            await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true ),
            RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true )
        };
    }

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = this._redisSetupFixture.TestInstance;
        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
    }
}