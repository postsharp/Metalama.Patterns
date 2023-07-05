// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Xunit;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public sealed class NullCacheBackendTests
    {
        [Fact]
        public void TestMiss()
        {
            using ( NullCachingBackend cache = new NullCachingBackend())
            {
                const string key = "0";

                object retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The cache does not return null on miss." );
            }
        }

        [Fact]
        public void TestSet()
        {
            using ( NullCachingBackend cache = new NullCachingBackend())
            {
                CachedValueClass storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem0 = new CacheItem(storedValue0);

                cache.SetItem( key, cacheItem0 ); 

                object retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The item has been stored in the cache." );
            }
        }
    }
}