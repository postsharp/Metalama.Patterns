// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

// ReSharper disable once UnusedType.Global
public class RedisDistributedCachingBackendTests : BaseDistributedCacheTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public RedisDistributedCachingBackendTests( TestContext testContext, RedisSetupFixture redisSetupFixture, ITestOutputHelper testOutputHelper ) : base(
        testContext,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true ),
            await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix, supportsDependencies: true )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new CachingBackend[]
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