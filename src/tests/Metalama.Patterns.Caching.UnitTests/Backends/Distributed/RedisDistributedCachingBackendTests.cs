﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

// ReSharper disable once UnusedType.Global
public class RedisDistributedCachingBackendTests : BaseDistributedCacheTests, IAssemblyFixture<RedisAssemblyFixture>
{
    private readonly RedisAssemblyFixture _redisAssemblyFixture;

    public RedisDistributedCachingBackendTests(
        CachingClassFixture cachingClassFixture,
        RedisAssemblyFixture redisAssemblyFixture,
        ITestOutputHelper testOutputHelper ) : base(
        cachingClassFixture,
        testOutputHelper )
    {
        this._redisAssemblyFixture = redisAssemblyFixture;
    }

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var prefix = Guid.NewGuid().ToString();

        return
        [
            await RedisFactory.CreateBackendAsync( this.ClassFixture, this._redisAssemblyFixture, this.ServiceProvider, prefix, supportsDependencies: true ),
            await RedisFactory.CreateBackendAsync( this.ClassFixture, this._redisAssemblyFixture, this.ServiceProvider,  prefix, supportsDependencies: true )
        ];
    }

    protected override CachingBackend[] CreateBackends() => Task.Run( this.CreateBackendsAsync ).Result;

    protected override void ConnectToRedisIfRequired()
    {
        var redisTestInstance = this._redisAssemblyFixture.TestInstance;
        this.ClassFixture.Endpoint = redisTestInstance.Endpoint;
    }
}