// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Tests.Backends;

public class RedisDistributedCachingBackendTests : BaseDistributedCacheTests
{
    public RedisDistributedCachingBackendTests( TestContext testContext ) : base( testContext ) { }

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            await RedisFactory.CreateBackendAsync( this.TestContext, prefix, supportsDependencies: true ),
            await RedisFactory.CreateBackendAsync( this.TestContext, prefix, supportsDependencies: true )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new[]
        {
            RedisFactory.CreateBackend( this.TestContext, prefix, supportsDependencies: true ),
            RedisFactory.CreateBackend( this.TestContext, prefix, supportsDependencies: true )
        };
    }

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = RedisPersistentInstance.GetOrLaunchRedisInstance();
        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
    }
}