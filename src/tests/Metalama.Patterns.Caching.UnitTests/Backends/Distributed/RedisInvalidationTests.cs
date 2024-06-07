// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Building;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

// ReSharper disable once UnusedType.Global
public class RedisInvalidationTests : BaseInvalidationBrokerTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public RedisInvalidationTests( CachingTestOptions cachingTestOptions, RedisSetupFixture redisSetupFixture, ITestOutputHelper testOutputHelper ) : base(
        cachingTestOptions,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override void ConnectToRedisIfRequired()
    {
        RedisFactory.CreateTestInstance( this.TestOptions, this._redisSetupFixture );
    }

    protected override ConcreteCachingBackendBuilder AddInvalidationBroker( MemoryCachingBackendBuilder builder, string prefix )
        => builder.WithRedisSynchronization( new RedisCacheSynchronizerConfiguration( RedisFactory.CreateConnection( this.TestOptions ), prefix ) );
}