// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
using System.Runtime.Caching;
using System.Threading.Tasks;
using Xunit;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.TestHelpers.Shared;
using PostSharp.Patterns.Common.Tests.Helpers;
using PostSharp.Patterns.Caching.Implementation;


namespace PostSharp.Patterns.Caching.TestHelpers
{
    public static class TestProfileConfigurationFactory
    {
        public static void InitializeTestWithoutBackend()
        {
            Assert.True( CachingServices.DefaultBackend is UninitializedCachingBackend
                || CachingServices.DefaultBackend is NullCachingBackend,
                "Each test has to use the TestProfileConfigurationFactory." );
            CachingServices.DefaultKeyBuilder = null; // Ensure we use the default key builder.
        }

        public static CachingBackend InitializeTestWithCachingBackend( string name )
        {
            InitializeTestWithoutBackend();
#if RUNTIME_CACHING
            MemoryCachingBackend backend = new MemoryCachingBackend( new System.Runtime.Caching.MemoryCache( "test-" + name ) );
#elif EXTENSIONS_CACHING
            MemoryCacheBackend backend = new MemoryCacheBackend(
                new Microsoft.Extensions.Caching.Memory.MemoryCache( new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() ) );
#else
#error You must define at least one of: RUNTIME_CACHING, EXTENSIONS_CACHING.
#endif
            CachingServices.DefaultBackend = backend;
            return backend;
        }

        public static TestingCacheBackend InitializeTestWithTestingBackend( string name )
        {
            InitializeTestWithoutBackend();
            TestingCacheBackend backend = new TestingCacheBackend( "test-" + name );
            CachingServices.DefaultBackend = backend;
            return backend;
        }

        public static CachingProfile CreateProfile( string name )
        {
            CachingProfile cacheProfile = new CachingProfile(name)
                                                     {
                                                         IsEnabled = true,
                                                     };

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
            await TestableCachingComponentDisposer.DisposeAsync(CachingServices.DefaultBackend);
            CachingServices.DefaultBackend = null;
        }
    }
}