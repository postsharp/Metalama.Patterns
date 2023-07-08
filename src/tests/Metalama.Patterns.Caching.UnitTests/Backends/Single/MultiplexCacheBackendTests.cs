// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Runtime.Caching;

namespace Metalama.Patterns.Caching.Tests.Backends
{
    public class TwoLayerCachingBackendTests : BaseCacheBackendTests
    {
        public TwoLayerCachingBackendTests( TestContext testContext ) : base( testContext ) { }

        protected override CachingBackend CreateBackend()
        {
            MemoryCacheHack.MakeExpirationChecksMoreFrequently();

            return new TwoLayerCachingBackendEnhancer( new MemoryCachingBackend( new MemoryCache( "1" ) ), new MemoryCachingBackend( new MemoryCache( "2" ) ) );
        }
    }

    public sealed class TwoLayerCachingBackendSimulatedLocalEvictionTests : TwoLayerCachingBackendTests
    {
        public TwoLayerCachingBackendSimulatedLocalEvictionTests( TestContext testContext ) : base( testContext ) { }

        protected override CachingBackend CreateBackend()
        {
            MemoryCacheHack.MakeExpirationChecksMoreFrequently();

            return new TwoLayerCachingBackendEnhancer( new MemoryCachingBackend( new MemoryCache( "1" ) ), new MemoryCachingBackend( new MemoryCache( "2" ) ) );
        }

        protected override void GiveChanceToResetLocalCache( CachingBackend backend )
        {
            ((TwoLayerCachingBackendEnhancer) backend).LocalCache.Clear();
        }
    }
}