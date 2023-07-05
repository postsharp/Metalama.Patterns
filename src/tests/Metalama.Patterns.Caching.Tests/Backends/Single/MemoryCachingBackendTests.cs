// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Runtime.Caching;

namespace Metalama.Patterns.Caching.Tests.Backends
{
    public sealed class MemoryCachingBackendTests : BaseCacheBackendTests
    {
        public MemoryCachingBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override CachingBackend CreateBackend()
        {
            MemoryCacheHack.MakeExpirationChecksMoreFrequently();
            return new MemoryCachingBackend( new MemoryCache( "1" ) );
        }
    }
}