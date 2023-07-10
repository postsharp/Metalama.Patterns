// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

public class SimpleRedisDistributedTest : BaseDistributedCacheTests
{
    public SimpleRedisDistributedTest( TestContext testContext ) : base( testContext ) { }

    protected override bool TestDependencies { get; } = false;

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            await RedisFactory.CreateBackendAsync( this.TestContext, prefix ), await RedisFactory.CreateBackendAsync( this.TestContext, prefix )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[] { RedisFactory.CreateBackend( this.TestContext, prefix ), RedisFactory.CreateBackend( this.TestContext, prefix ) };
    }

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = RedisPersistentInstance.GetOrLaunchRedisInstance();
        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
    }
}