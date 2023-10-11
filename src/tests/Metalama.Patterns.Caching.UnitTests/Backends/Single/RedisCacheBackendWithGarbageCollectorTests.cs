// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

public class RedisCacheBackendWithGarbageCollectorTests : RedisCacheBackendTests
{
    public RedisCacheBackendWithGarbageCollectorTests(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        ITestOutputHelper testOutputHelper ) : base( cachingTestOptions, redisSetupFixture, testOutputHelper ) { }

    protected override bool GarbageCollectorEnabled => true;
}