// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single
{
    // ReSharper disable once UnusedType.Global
    public sealed class TwoLayerCachingBackendSimulatedLocalEvictionTests : TwoLayerCachingBackendTests
    {
        public TwoLayerCachingBackendSimulatedLocalEvictionTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base(
            testContext,
            testOutputHelper ) { }

        protected override CachingBackend CreateBackend()
        {
            return new TwoLayerCachingBackendEnhancer(
                MemoryCacheFactory.CreateBackend( this.ServiceProvider, "Remote" ),
                MemoryCacheFactory.CreateBackend( this.ServiceProvider, "Local" ) ) { DebugName = "TwoLayer" };
        }

        protected override void GiveChanceToResetLocalCache( CachingBackend backend )
        {
            ((TwoLayerCachingBackendEnhancer) backend).LocalCache.Clear();
        }
    }
}