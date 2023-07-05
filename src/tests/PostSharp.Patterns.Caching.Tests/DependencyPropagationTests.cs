using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Caching.Implementation;
using System.Threading.Tasks;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests
{
    public sealed class DependencyPropagationTests
    {
        private const string profileNamePrefix = "Caching.Tests.DependencyPropagationTests_";

        #region TestDependencyPropagation

        private const string testDependencyPropagationProfileName = profileNamePrefix + "DependencyPropagation";

        [CacheConfiguration(ProfileName = testDependencyPropagationProfileName)]
        private sealed class TestDependencyPropagationCachingClass
        {
            public bool WasGetValueCalled { get; set; }
            public bool WasGetValueIntermediateCalled { get; set; }
            public bool WasGetValueDependencyCalled { get; set; }

            [Cache]
            public CachedValueClass GetValue()
            {
                this.WasGetValueCalled = true;
                return this.GetValueIntermediate();
            }
            [Cache]
            public CachedValueClass GetValueIntermediate()
            {
                this.WasGetValueIntermediateCalled = true;
                return this.GetValueDependency();
            }

            [Cache]
            public CachedValueClass GetValueDependency()
            {
                this.WasGetValueDependencyCalled = true;
                return new CachedValueClass();
            }
        }

        [Fact]
        public void TestDependencyPropagation()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend(testDependencyPropagationProfileName);
            TestProfileConfigurationFactory.CreateProfile(testDependencyPropagationProfileName);
            try
            {
                TestDependencyPropagationCachingClass cachingClass = new TestDependencyPropagationCachingClass();

                cachingClass.GetValue();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                CachingServices.Invalidation.Invalidate(cachingClass.GetValueDependency);

                cachingClass.GetValue();

                Assert.True(cachingClass.WasGetValueCalled, "The outer method was not invalidated.");
                Assert.True(cachingClass.WasGetValueIntermediateCalled, "The intermediate method was not invalidated.");
                Assert.True(cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated.");
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestDependencyPropagationAsync

        private const string testDependencyPropagationAsyncProfileName = profileNamePrefix + "DependencyPropagationAsync";

        [CacheConfiguration(ProfileName = testDependencyPropagationAsyncProfileName)]
        private sealed class TestDependencyPropagationAsyncCachingClass
        {
            public bool WasGetValueCalled { get; set; }
            public bool WasGetValueIntermediateCalled { get; set; }
            public bool WasGetValueDependencyCalled { get; set; }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.WasGetValueCalled = true;
                await Task.Yield();
                return await this.GetValueIntermediateAsync();
            }
            [Cache]
            public async Task<CachedValueClass> GetValueIntermediateAsync()
            {
                this.WasGetValueIntermediateCalled = true;
                await Task.Yield();
                return await this.GetValueDependencyAsync();
            }

            [Cache]
            public async Task<CachedValueClass> GetValueDependencyAsync()
            {
                this.WasGetValueDependencyCalled = true;
                await Task.Yield();
                return new CachedValueClass();
            }
        }

        [Fact]
        public async Task TestDependencyPropagationAsync()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend(testDependencyPropagationAsyncProfileName);
            TestProfileConfigurationFactory.CreateProfile(testDependencyPropagationAsyncProfileName);
            try
            {
                TestDependencyPropagationAsyncCachingClass cachingClass = new TestDependencyPropagationAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.Invalidation.InvalidateAsync(cachingClass.GetValueDependencyAsync);

                await cachingClass.GetValueAsync();

                Assert.True(cachingClass.WasGetValueCalled, "The outer method was not invalidated.");
                Assert.True(cachingClass.WasGetValueIntermediateCalled, "The intermediate method was not invalidated.");
                Assert.True(cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated.");
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion
        
        #region TestDisposedContextAsync

        private const string testDisposedContextAsyncProfileName = profileNamePrefix + "DisposedContextAsync";

        [CacheConfiguration(ProfileName = testDisposedContextAsyncProfileName)]
        private sealed class TestDisposedContextAsyncCachingClass
        {
            public bool WasGetValueCalled { get; set; }
            public bool WasGetValueDependencyCalled { get; set; }

            private volatile Task syncTask;

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.WasGetValueCalled = true;
                await Task.Yield();
                this.syncTask = new Task(() => { });
                Task<CachedValueClass> dependencyTask = await this.GetValueIntermediateAsync();
                syncTask.Start();
                return await dependencyTask;
            }

            public async Task<Task<CachedValueClass>> GetValueIntermediateAsync()
            {
                using (CachingContext.OpenCacheContext("k"))
                {
                    await Task.Yield();
                    return Task.Run(async () => await GetValueDependencyAsync());
                }
            }

            [Cache]
            public async Task<CachedValueClass> GetValueDependencyAsync()
            {
                this.WasGetValueDependencyCalled = true;
                await this.syncTask;
                return new CachedValueClass();
            }
        }

        [Fact]
        public async Task TestDisposedContextAsync()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend(testDisposedContextAsyncProfileName);
            TestProfileConfigurationFactory.CreateProfile(testDisposedContextAsyncProfileName);
            try
            {
                // This test is trying not-so-nicely replicate previously crashing scenario with context being disposed before it's child context.
                // The real-world scenario is probably much less strange and much more complicated.

                TestDisposedContextAsyncCachingClass cachingClass = new TestDisposedContextAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.Invalidation.InvalidateAsync(cachingClass.GetValueDependencyAsync);

                await cachingClass.GetValueAsync();

                Assert.True(cachingClass.WasGetValueCalled, "The outer method was not invalidated.");
                Assert.True(cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated.");
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion


        #region TestSuspendedDependencyPropagation

        private const string testSuspendedDependencyPropagationProfileName = profileNamePrefix + "SuspendedDependencyPropagation";

        [CacheConfiguration(ProfileName = testSuspendedDependencyPropagationProfileName)]
        private sealed class TestSuspendedDependencyPropagationCachingClass
        {
            public bool WasGetValueCalled { get; set; }
            public bool WasGetValueIntermediateCalled { get; set; }
            public bool WasGetValueDependencyCalled { get; set; }

            [Cache]
            public CachedValueClass GetValue()
            {
                this.WasGetValueCalled = true;
                return this.GetValueIntermediate();
            }

            [Cache]
            public CachedValueClass GetValueIntermediate()
            {
                this.WasGetValueIntermediateCalled = true;

                using (CachingServices.SuspendDependencyPropagation())
                {
                    return this.GetValueDependency();
                }
            }

            [Cache]
            public CachedValueClass GetValueDependency()
            {
                this.WasGetValueDependencyCalled = true;
                return new CachedValueClass();
            }
        }

        [Fact]
        public void TestSuspendedDependencyPropagation()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend(testSuspendedDependencyPropagationProfileName);
            TestProfileConfigurationFactory.CreateProfile(testSuspendedDependencyPropagationProfileName);
            try
            {
                TestSuspendedDependencyPropagationCachingClass cachingClass = new TestSuspendedDependencyPropagationCachingClass();

                cachingClass.GetValue();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                CachingServices.Invalidation.Invalidate(cachingClass.GetValueDependency);

                cachingClass.GetValue();

                Assert.False(cachingClass.WasGetValueCalled, "The outer method was invalidated.");
                Assert.False(cachingClass.WasGetValueIntermediateCalled, "The intermediate method was invalidated.");
                Assert.False(cachingClass.WasGetValueDependencyCalled, "The dependency method was called.");

                cachingClass.GetValueDependency();

                Assert.True(cachingClass.WasGetValueDependencyCalled, "The intermediate method was not invalidated.");
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestSuspendedDependencyPropagationAsync

        private const string testSuspendedDependencyPropagationAsyncProfileName = profileNamePrefix + "SuspendedDependencyPropagationAsync";

        [CacheConfiguration(ProfileName = testSuspendedDependencyPropagationAsyncProfileName)]
        private sealed class TestSuspendedDependencyPropagationAsyncCachingClass
        {
            public bool WasGetValueCalled { get; set; }
            public bool WasGetValueIntermediateCalled { get; set; }
            public bool WasGetValueDependencyCalled { get; set; }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.WasGetValueCalled = true;
                await Task.Yield();
                return await this.GetValueIntermediateAsync();
            }
            [Cache]
            public async Task<CachedValueClass> GetValueIntermediateAsync()
            {
                this.WasGetValueIntermediateCalled = true;
                await Task.Yield();
                using (CachingServices.SuspendDependencyPropagation())
                {
                    return await this.GetValueDependencyAsync();
                }
            }

            [Cache]
            public async Task<CachedValueClass> GetValueDependencyAsync()
            {
                this.WasGetValueDependencyCalled = true;
                await Task.Yield();
                return new CachedValueClass();
            }
        }

        [Fact]
        public async Task TestSuspendedDependencyPropagationAsync()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend(testSuspendedDependencyPropagationAsyncProfileName);
            TestProfileConfigurationFactory.CreateProfile(testSuspendedDependencyPropagationAsyncProfileName);
            try
            {
                TestSuspendedDependencyPropagationAsyncCachingClass cachingClass = new TestSuspendedDependencyPropagationAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.Invalidation.InvalidateAsync(cachingClass.GetValueDependencyAsync);

                await cachingClass.GetValueAsync();

                Assert.False(cachingClass.WasGetValueCalled, "The outer method was invalidated.");
                Assert.False(cachingClass.WasGetValueIntermediateCalled, "The intermediate method was invalidated.");
                Assert.False(cachingClass.WasGetValueDependencyCalled, "The dependency method was called.");

                await cachingClass.GetValueDependencyAsync();

                Assert.True(cachingClass.WasGetValueDependencyCalled, "The intermediate method was not invalidated.");
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion
    }
}
