// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial source-available license. Please see the LICENSE.md file in the repository root for details.

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
        public MicrosoftIMemoryCacheTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override CachingBackend CreateBackend()
        {
            return new MemoryCacheBackend(new MemoryCache( new MemoryCacheOptions()
                                                           {
                                                               ExpirationScanFrequency = TimeSpan.FromMilliseconds( 10 )
                                                           } ));
        }
    }
}

#endif