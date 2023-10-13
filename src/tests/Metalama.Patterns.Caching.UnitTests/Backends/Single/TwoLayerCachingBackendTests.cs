// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Building;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single;

public class TwoLayerCachingBackendTests : BaseCacheBackendTests
{
    // ReSharper disable once MemberCanBeProtected.Global
    public TwoLayerCachingBackendTests( CachingTestOptions cachingTestOptions, ITestOutputHelper testOutputHelper ) : base(
        cachingTestOptions,
        testOutputHelper ) { }

    protected override CheckAfterDisposeCachingBackend CreateBackend()
    {
#pragma warning disable CS0618 // Type or member is obsolete

        var backend = CachingBackend.Create(
            b => b.Memory( new MemoryCachingBackendConfiguration { DebugName = "Remote" } )
                .WithL1()
                .WithLocalCacheConfiguration( new MemoryCachingBackendConfiguration { DebugName = "Local" } ) );

        backend.DebugName = "TwoLayer";

        return new CheckAfterDisposeCachingBackend( backend );

#pragma warning restore CS0618 // Type or member is obsolete
    }
}