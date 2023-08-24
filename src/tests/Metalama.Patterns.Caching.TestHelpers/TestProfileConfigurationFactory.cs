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
                CachingServices.DefaultService.DefaultBackend is UninitializedCachingBackend or NullCachingBackend,
                "Each test has to use the TestProfileConfigurationFactory." );

            CachingServices.DefaultService.KeyBuilder = null; // Ensure we use the default key builder.
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static CachingBackend InitializeTestWithCachingBackend( string name )
        {
            InitializeTestWithoutBackend();
            var backend = new MemoryCachingBackend();
            CachingServices.DefaultService.DefaultBackend = backend;

            return backend;
        }

        public static TestingCacheBackend InitializeTestWithTestingBackend( string name )
        {
            InitializeTestWithoutBackend();
            var backend = new TestingCacheBackend( "test-" + name );
            CachingServices.DefaultService.DefaultBackend = backend;

            return backend;
        }

        public static CachingProfile CreateProfile( string name ) => CachingServices.DefaultService.Profiles[name];

        public static void DisposeTest()
        {
            CachingServices.DefaultService.Profiles.Reset();
            TestableCachingComponentDisposer.Dispose( CachingServices.DefaultService.DefaultBackend );
            CachingServices.DefaultService.DefaultBackend = null;
        }

        public static async Task DisposeTestAsync()
        {
            CachingServices.DefaultService.Profiles.Reset();
            await TestableCachingComponentDisposer.DisposeAsync( CachingServices.DefaultService.DefaultBackend );
            CachingServices.DefaultService.DefaultBackend = null;
        }
    }
}