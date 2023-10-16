// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

#if !ENABLE_FLAKY_REDIS_TESTS
public class LocallyCachedRedisCacheBackendWithGarbageCollectorTests
{
    [Fact( Skip = "#33990: Skipping all tests in class LocallyCachedRedisCacheBackendWithGarbageCollectorTests" )]
    public void PlaceholderForInheritedTestsInCommonBaseClass()
    { }
}
#else
using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;
public class LocallyCachedRedisCacheBackendWithGarbageCollectorTests : LocallyCachedRedisCacheBackendTests
{
    public LocallyCachedRedisCacheBackendWithGarbageCollectorTests(
        CachingTestOptions cachingTestOptions,
        RedisSetupFixture redisSetupFixture,
        ITestOutputHelper testOutputHelper ) : base( cachingTestOptions, redisSetupFixture, testOutputHelper ) { }

    protected override bool EnableGarbageCollector => true;
}
#endif