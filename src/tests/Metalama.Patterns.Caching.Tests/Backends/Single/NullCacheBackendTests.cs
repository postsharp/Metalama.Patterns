// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.Backends
{
    public sealed class NullCacheBackendTests
    {
        [Fact]
        public void TestMiss()
        {
            using ( var cache = new NullCachingBackend() )
            {
                const string key = "0";

                object retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The cache does not return null on miss." );
            }
        }

        [Fact]
        public void TestSet()
        {
            using ( var cache = new NullCachingBackend() )
            {
                CachedValueClass storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                var cacheItem0 = new CacheItem( storedValue0 );

                cache.SetItem( key, cacheItem0 );

                object retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The item has been stored in the cache." );
            }
        }
    }
}