// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if EXTENSIONS_CACHING

using System;
using Microsoft.Extensions.Caching.Memory;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Tests.Backends;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class MicrosoftIMemoryCacheTests : BaseCacheBackendTests
    {
        public MicrosoftIMemoryCacheTests( TestContext testContext ) : base( testContext ) { }

        protected override CachingBackend CreateBackend()
        {
            return new MemoryCacheBackend( new MemoryCache( new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMilliseconds( 10 ) } ) );
        }
    }
}

#endif