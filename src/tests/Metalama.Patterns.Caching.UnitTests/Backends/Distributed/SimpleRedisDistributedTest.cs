// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

// ReSharper disable once UnusedType.Global
public class SimpleRedisDistributedTest : BaseDistributedCacheTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public SimpleRedisDistributedTest( CachingTestOptions cachingTestOptions, RedisSetupFixture redisSetupFixture, ITestOutputHelper testOutputHelper ) : base(
        cachingTestOptions,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override bool TestDependencies => false;

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            await RedisFactory.CreateBackendAsync( this.TestOptions, this._redisSetupFixture, prefix ),
            await RedisFactory.CreateBackendAsync( this.TestOptions, this._redisSetupFixture, prefix )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var prefix = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            RedisFactory.CreateBackend( this.TestOptions, this._redisSetupFixture, prefix ),
            RedisFactory.CreateBackend( this.TestOptions, this._redisSetupFixture, prefix )
        };
    }

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = this._redisSetupFixture.TestInstance;
        this.TestOptions.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
    }
}