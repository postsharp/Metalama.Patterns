// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if RUNTIME_CACHING

using System;
using System.Runtime.Caching;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using System.Threading;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Backends;

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

#endif