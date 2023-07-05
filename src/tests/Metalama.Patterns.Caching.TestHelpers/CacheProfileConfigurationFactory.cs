// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Xunit;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public static class TestProfileConfigurationFactory
    {
        public static void InitializeTestWithoutBackend()
        {
            Assert.True(
                CachingServices.DefaultBackend is UninitializedCachingBackend
                || CachingServices.DefaultBackend is NullCachingBackend,
                "Each test has to use the TestProfileConfigurationFactory." );

            CachingServices.DefaultKeyBuilder = null; // Ensure we use the default key builder.
        }

        public static CachingBackend InitializeTestWithCachingBackend( string name )
        {
            InitializeTestWithoutBackend();

#if NETFRAMEWORK
            var backend = new MemoryCacheBackend(
                new Microsoft.Extensions.Caching.Memory.MemoryCache( new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() ) );
#else
            var backend = new MemoryCachingBackend( new System.Runtime.Caching.MemoryCache( "test-" + name ) );
#endif

            CachingServices.DefaultBackend = backend;

            return backend;
        }

        public static TestingCacheBackend InitializeTestWithTestingBackend( string name )
        {
            InitializeTestWithoutBackend();
            var backend = new TestingCacheBackend( "test-" + name );
            CachingServices.DefaultBackend = backend;

            return backend;
        }

        public static CachingProfile CreateProfile( string name )
        {
            var cacheProfile = new CachingProfile( name ) { IsEnabled = true };

            CachingServices.Profiles.Register( cacheProfile );

            return cacheProfile;
        }

        public static void DisposeTest()
        {
            CachingServices.Profiles.Reset();
            TestableCachingComponentDisposer.Dispose( CachingServices.DefaultBackend );
            CachingServices.DefaultBackend = null;
        }

        public static async Task DisposeTestAsync()
        {
            CachingServices.Profiles.Reset();
            await TestableCachingComponentDisposer.DisposeAsync( CachingServices.DefaultBackend );
            CachingServices.DefaultBackend = null;
        }
    }
}