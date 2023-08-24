// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Global
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class DependencyPropagationTests
    {
        private const string _profileNamePrefix = "Caching.Tests.DependencyPropagationTests_";

        #region TestDependencyPropagation

        private const string _testDependencyPropagationProfileName = _profileNamePrefix + "DependencyPropagation";

        [CacheConfiguration( ProfileName = _testDependencyPropagationProfileName )]
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
            _ = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( _testDependencyPropagationProfileName );

            TestProfileConfigurationFactory.CreateProfile( _testDependencyPropagationProfileName );

            try
            {
                var cachingClass = new TestDependencyPropagationCachingClass();

                cachingClass.GetValue();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                CachingServices.DefaultService.Invalidation.Invalidate( cachingClass.GetValueDependency );

                cachingClass.GetValue();

                Assert.True( cachingClass.WasGetValueCalled, "The outer method was not invalidated." );
                Assert.True( cachingClass.WasGetValueIntermediateCalled, "The intermediate method was not invalidated." );
                Assert.True( cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestDependencyPropagationAsync

        private const string _testDependencyPropagationAsyncProfileName = _profileNamePrefix + "DependencyPropagationAsync";

        [CacheConfiguration( ProfileName = _testDependencyPropagationAsyncProfileName )]
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
            _ = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( _testDependencyPropagationAsyncProfileName );

            TestProfileConfigurationFactory.CreateProfile( _testDependencyPropagationAsyncProfileName );

            try
            {
                var cachingClass = new TestDependencyPropagationAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.DefaultService.Invalidation.InvalidateAsync( cachingClass.GetValueDependencyAsync );

                await cachingClass.GetValueAsync();

                Assert.True( cachingClass.WasGetValueCalled, "The outer method was not invalidated." );
                Assert.True( cachingClass.WasGetValueIntermediateCalled, "The intermediate method was not invalidated." );
                Assert.True( cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated." );
            }
            finally
            {
                // [Porting] Won't fix, can't be certain of original intent.
                // ReSharper disable once MethodHasAsyncOverload
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestDisposedContextAsync

        private const string _testDisposedContextAsyncProfileName = _profileNamePrefix + "DisposedContextAsync";

        // [Porting] Won't fix, can't be certain of original intent.
#pragma warning disable CA1001
        [CacheConfiguration( ProfileName = _testDisposedContextAsyncProfileName )]
        private sealed class TestDisposedContextAsyncCachingClass
#pragma warning restore CA1001
        {
            public bool WasGetValueCalled { get; set; }

            public bool WasGetValueDependencyCalled { get; set; }

            private volatile Task? _syncTask;

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.WasGetValueCalled = true;
                await Task.Yield();
                this._syncTask = new Task( () => { } );
                var dependencyTask = await this.GetValueIntermediateAsync();
                this._syncTask.Start();

                return await dependencyTask;
            }

            public async Task<Task<CachedValueClass>> GetValueIntermediateAsync()
            {
                using ( CachingContext.OpenCacheContext( "k", CachingServices.DefaultService ) )
                {
                    await Task.Yield();

                    return Task.Run( async () => await this.GetValueDependencyAsync() );
                }
            }

            [Cache]
            public async Task<CachedValueClass> GetValueDependencyAsync()
            {
                this.WasGetValueDependencyCalled = true;
                await this._syncTask!;

                return new CachedValueClass();
            }
        }

        [Fact]
        public async Task TestDisposedContextAsync()
        {
            _ = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( _testDisposedContextAsyncProfileName );

            TestProfileConfigurationFactory.CreateProfile( _testDisposedContextAsyncProfileName );

            try
            {
                // This test is trying not-so-nicely replicate previously crashing scenario with context being disposed before it's child context.
                // The real-world scenario is probably much less strange and much more complicated.

                var cachingClass = new TestDisposedContextAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.DefaultService.Invalidation.InvalidateAsync( cachingClass.GetValueDependencyAsync );

                await cachingClass.GetValueAsync();

                Assert.True( cachingClass.WasGetValueCalled, "The outer method was not invalidated." );
                Assert.True( cachingClass.WasGetValueDependencyCalled, "The inner method was not invalidated." );
            }
            finally
            {
                // [Porting] Won't fix, can't be certain of original intent.
                // ReSharper disable once MethodHasAsyncOverload
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestSuspendedDependencyPropagation

        private const string _testSuspendedDependencyPropagationProfileName = _profileNamePrefix + "SuspendedDependencyPropagation";

        [CacheConfiguration( ProfileName = _testSuspendedDependencyPropagationProfileName )]
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

                using ( CachingServices.SuspendDependencyPropagation() )
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
            _ = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( _testSuspendedDependencyPropagationProfileName );

            TestProfileConfigurationFactory.CreateProfile( _testSuspendedDependencyPropagationProfileName );

            try
            {
                var cachingClass = new TestSuspendedDependencyPropagationCachingClass();

                cachingClass.GetValue();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                CachingServices.DefaultService.Invalidation.Invalidate( cachingClass.GetValueDependency );

                cachingClass.GetValue();

                Assert.False( cachingClass.WasGetValueCalled, "The outer method was invalidated." );
                Assert.False( cachingClass.WasGetValueIntermediateCalled, "The intermediate method was invalidated." );
                Assert.False( cachingClass.WasGetValueDependencyCalled, "The dependency method was called." );

                cachingClass.GetValueDependency();

                Assert.True( cachingClass.WasGetValueDependencyCalled, "The intermediate method was not invalidated." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestSuspendedDependencyPropagationAsync

        private const string _testSuspendedDependencyPropagationAsyncProfileName = _profileNamePrefix + "SuspendedDependencyPropagationAsync";

        [CacheConfiguration( ProfileName = _testSuspendedDependencyPropagationAsyncProfileName )]
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

                using ( CachingServices.SuspendDependencyPropagation() )
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
            _ = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( _testSuspendedDependencyPropagationAsyncProfileName );

            TestProfileConfigurationFactory.CreateProfile( _testSuspendedDependencyPropagationAsyncProfileName );

            try
            {
                var cachingClass = new TestSuspendedDependencyPropagationAsyncCachingClass();

                await cachingClass.GetValueAsync();

                cachingClass.WasGetValueCalled = false;
                cachingClass.WasGetValueIntermediateCalled = false;
                cachingClass.WasGetValueDependencyCalled = false;

                await CachingServices.DefaultService.Invalidation.InvalidateAsync( cachingClass.GetValueDependencyAsync );

                await cachingClass.GetValueAsync();

                Assert.False( cachingClass.WasGetValueCalled, "The outer method was invalidated." );
                Assert.False( cachingClass.WasGetValueIntermediateCalled, "The intermediate method was invalidated." );
                Assert.False( cachingClass.WasGetValueDependencyCalled, "The dependency method was called." );

                await cachingClass.GetValueDependencyAsync();

                Assert.True( cachingClass.WasGetValueDependencyCalled, "The intermediate method was not invalidated." );
            }
            finally
            {
                // [Porting] Won't fix, can't be certain of original intent.
                // ReSharper disable once MethodHasAsyncOverload
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion
    }
}