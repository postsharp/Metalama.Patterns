// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Building;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single
{
    // ReSharper disable once UnusedType.Global
    public sealed class TwoLayerCachingBackendSimulatedLocalEvictionTests : TwoLayerCachingBackendTests
    {
        public TwoLayerCachingBackendSimulatedLocalEvictionTests( CachingTestOptions cachingTestOptions, ITestOutputHelper testOutputHelper ) : base(
            cachingTestOptions,
            testOutputHelper ) { }

        protected override CheckAfterDisposeCachingBackend CreateBackend()
        {
#pragma warning disable CS0618 // Type or member is obsolete

            var backend = CachingBackend.Create(
                b => b.Memory( new MemoryCachingBackendConfiguration { DebugName = "Remote" } )
                    .WithLocalLayer()
                    .WithLocalCacheConfiguration( new MemoryCachingBackendConfiguration { DebugName = "Local" } ) );

            backend.DebugName = "TwoLayer";

#pragma warning restore CS0618 // Type or member is obsolete

            return new CheckAfterDisposeCachingBackend( backend );
        }

        protected override void GiveChanceToResetLocalCache( CachingBackend backend )
        {
            backend.Clear( ClearCacheOptions.Local );
        }
    }
}