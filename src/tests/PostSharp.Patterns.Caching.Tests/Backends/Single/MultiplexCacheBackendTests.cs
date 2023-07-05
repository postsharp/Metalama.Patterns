// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if RUNTIME_CACHING

using System;
using System.Runtime.Caching;
using Xunit;
using PostSharp.Patterns.Caching.TestHelpers;
using System.Threading;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public class TwoLayerCachingBackendTests : BaseCacheBackendTests
    {
        public TwoLayerCachingBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override CachingBackend CreateBackend()
        {
            MemoryCacheHack.MakeExpirationChecksMoreFrequently();
            return new TwoLayerCachingBackendEnhancer( new MemoryCachingBackend(new MemoryCache("1")), new MemoryCachingBackend(new MemoryCache("2")) );
        }

    }

    public sealed class TwoLayerCachingBackendSimulatedLocalEvictionTests : TwoLayerCachingBackendTests
    {
        public TwoLayerCachingBackendSimulatedLocalEvictionTests( TestContext testContext ) : base( testContext )
        {
        }

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