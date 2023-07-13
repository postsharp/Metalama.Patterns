// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Microsoft.Extensions.Caching.Memory;

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