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
                CachingService.Default.DefaultBackend is UninitializedCachingBackend or NullCachingBackend,
                "Each test has to use the TestProfileConfigurationFactory." );

            CachingService.Default.KeyBuilder = null; // Ensure we use the default key builder.
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static CachingBackend InitializeTestWithCachingBackend( string name, IServiceProvider serviceProvider )
        {
            InitializeTestWithoutBackend();
            var backend = new MemoryCachingBackend( new MemoryCachingBackendConfiguration { ServiceProvider = serviceProvider } ) { DebugName = name };
            CachingService.Default.DefaultBackend = backend;

            return backend;
        }

        public static TestingCacheBackend InitializeTestWithTestingBackend( string name, IServiceProvider? serviceProvider )
        {
            InitializeTestWithoutBackend();
            var backend = new TestingCacheBackend( "test-" + name, serviceProvider );
            CachingService.Default.DefaultBackend = backend;

            return backend;
        }

        public static CachingProfile CreateProfile( string name ) => CachingService.Default.Profiles[name];

        public static void DisposeTest()
        {
            CachingService.Default.Profiles.Reset();
            TestableCachingComponentDisposer.Dispose( CachingService.Default.DefaultBackend );
            CachingService.Default.DefaultBackend = null;
        }

        public static async Task DisposeTestAsync()
        {
            CachingService.Default.Profiles.Reset();
            await TestableCachingComponentDisposer.DisposeAsync( CachingService.Default.DefaultBackend );
            CachingService.Default.DefaultBackend = null;
        }
    }
}