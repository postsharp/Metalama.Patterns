// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Xunit;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.TestHelpers.Shared;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.TestHelpersTests
{
    // TODO: as we have prepared the generalization of testing of the CacheBackend ancestors, we should remove the respective test from this class.
    // (See MemoryCacheBackendTests.)

    public sealed class TestingCacheBackendTests
    {
        private const string namePrefix = "Caching.Tests.Backends.TestingCacheBackendTests_";

        [Fact]
        public void TestContainsKey()
        {
            using ( TestingCacheBackend cache = new TestingCacheBackend( namePrefix + "TestContainsKey" ) )
            {
                const string key = "0";

                cache.ExpectedContainsKeyCount = 1;
                bool keyContained = cache.ContainsItem( key );
                cache.ResetTest( "When checkin existence of a non-existing key." );
                
                Assert.False( keyContained, "The cache does not return false on miss." );

                CachedValueClass storedValue0 = new CachedValueClass(0);
                CacheItem cacheItem0 = new CacheItem(storedValue0);
                
                cache.ExpectedSetCount = 1;
                cache.SetItem( key, cacheItem0 );
                cache.ResetTest("When setting the cache item");

                cache.ExpectedContainsKeyCount = 1;
                keyContained = cache.ContainsItem(key);
                cache.ResetTest("When checkin existence of an existing key.");

                Assert.True(keyContained, "The cache does not return true on hit.");

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact]
        public void TestGet()
        {
            using ( TestingCacheBackend cache = new TestingCacheBackend( namePrefix + "TestGet" ) )
            {
                const string key = "0";

                cache.ExpectedGetCount = 1;
                object retrievedValue = cache.GetItem( key );
                cache.ResetTest( "When getting the cached value" );

                AssertEx.Null( retrievedValue, "The cache does not return null on miss." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact]
        public void TestSet()
        {
            using ( TestingCacheBackend cache = new TestingCacheBackend( namePrefix + "TestSet" ) )
            {
                CachedValueClass storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem0 = new CacheItem(storedValue0);

                cache.ExpectedSetCount = 1;
                cache.SetItem( key, cacheItem0 );
                cache.ResetTest( "When setting the cache item" );

                cache.ExpectedGetCount = 1;
                CacheValue retrievedItem = cache.GetItem( key );
                cache.ResetTest( "When getting the cache item" );
                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.Equal( cacheItem0.Value, retrievedItem.Value, "The retrieved item is not equal to the stored item." );

                CachedValueClass storedValue1 = new CachedValueClass( 1 );
                CacheItem cacheItem1 = new CacheItem(storedValue1);

                cache.ExpectedSetCount = 1;
                cache.SetItem( key, cacheItem1 );
                cache.ResetTest( "When setting the second item" );

                retrievedItem = cache.GetItem( key );
                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.NotEqual( cacheItem0, retrievedItem, "The item has not been changed." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }
        
        [Fact]
        public void TestInvalidateObjectDependency()
        {
            using ( TestingCacheBackend cache = new TestingCacheBackend( namePrefix + "TestInvalidateObjectDependency" ) )
            {
                const string dependencyKey = "0";

                cache.ExpectedInvalidateCount = 1;
                cache.InvalidateDependency( dependencyKey);
                cache.ResetTest( "When invalidating dependency" );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }
    }
}