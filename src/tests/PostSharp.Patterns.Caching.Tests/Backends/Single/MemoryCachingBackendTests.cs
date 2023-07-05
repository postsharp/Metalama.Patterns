// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if RUNTIME_CACHING

using System.Runtime.Caching;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.Implementation;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public sealed class MemoryCachingBackendTests : BaseCacheBackendTests
    {
        public MemoryCachingBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override CachingBackend CreateBackend()
        {
            MemoryCacheHack.MakeExpirationChecksMoreFrequently();
            return new MemoryCachingBackend(new MemoryCache("1"));
        }
    }
}

#endif
