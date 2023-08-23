// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using CacheItemPriority = Metalama.Patterns.Caching.Implementation.CacheItemPriority;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class SizeCalculatorTests
    {
        [Fact( Timeout = 5000 )]
        public void TestSizeCalculator()
        {
            var backend = new MemoryCachingBackend(
                new MemoryCache(
                    new MemoryCacheOptions()
                    {
                        SizeLimit = 5, CompactionPercentage = 1 // compact full cache
                    } ),
#pragma warning disable CS8605
                ( cItem ) => (int) cItem.Value );
#pragma warning restore CS8605

            backend.SetItem( "A", new CacheItem( 2 ) );
            backend.SetItem( "B", new CacheItem( 2 ) );
            Assert.True( backend.ContainsItem( "A" ) );
            Assert.True( backend.ContainsItem( "B" ) );
            backend.SetItem( "C", new CacheItem( 2 ) );

            Assert.False( backend.ContainsItem( "C" ) );
            EvictEventually( backend, "A" );
        }

        [Fact( Timeout = 5000 )]
        public void TestPriorities()
        {
            var backend = new MemoryCachingBackend(
                new MemoryCache(
                    new MemoryCacheOptions()
                    {
                        SizeLimit = 10, CompactionPercentage = 0.5 // compact half
                    } ),
                ( cItem ) => 1 );

            backend.SetItem( "A", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );
            backend.SetItem( "B", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );
            backend.SetItem( "C", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.Low } ) );
            backend.SetItem( "D", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );
            backend.SetItem( "E", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );

            backend.SetItem( "F", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.Low } ) );
            backend.SetItem( "G", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.Low } ) );
            backend.SetItem( "H", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );
            backend.SetItem( "I", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );
            backend.SetItem( "J", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );

            // trigger compaction:
            backend.SetItem( "K", new CacheItem( 2, null, new CacheItemConfiguration { Priority = CacheItemPriority.High } ) );

            Assert.False( backend.ContainsItem( "K" ) );
            EvictEventually( backend, "G" );
            EvictEventually( backend, "F" );
            EvictEventually( backend, "C" );

            // Some items are still in the cache:
            var count = 0;

            for ( var c = 'A'; c <= 'J'; c++ )
            {
                if ( backend.ContainsItem( c.ToString() ) )
                {
                    count++;
                }
            }

            Assert.True( count > 4 );
        }

        private static void EvictEventually( MemoryCachingBackend backend, string key )
        {
            while ( backend.ContainsItem( key ) )
            {
                // wait for eviction
                Thread.Yield();
            }
        }
    }
}