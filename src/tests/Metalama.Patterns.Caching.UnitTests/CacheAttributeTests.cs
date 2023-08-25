// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;
using CacheItemPriority = Metalama.Patterns.Caching.Implementation.CacheItemPriority;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable MemberCanBeMadeStatic.Local
#pragma warning disable SA1203
#pragma warning disable CA1822

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class CacheAttributeTests : BaseCachingTests
    {
        private const string _profileNamePrefix = "Caching.Tests.CacheAttributeTests_";

        // We don't like timeouts in tests but we need them to avoid test suites to hang in case of issues.
        private static readonly TimeSpan _timeout = TimeSpan.FromMinutes( 2 );

        public CacheAttributeTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }

        #region TestSync

        private const string _testSyncProfileName = _profileNamePrefix + "TestSync";

        [CacheConfiguration( ProfileName = _testSyncProfileName )]
        private sealed class TestSyncCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }

            [Cache]
            public override CachedValueClass GetValue( int param1 )
            {
                return base.GetValue( param1 );
            }

            [Cache]
            public override CachedValueClass GetValue( int param1, int param2 )
            {
                return base.GetValue( param1, param2 );
            }

            [Cache]
            public override CachedValueClass GetValue( int param1, int param2, int param3 )
            {
                return base.GetValue( param1, param2, param3 );
            }
        }

        [Fact]
        public void TestSync()
        {
            this.InitializeTestWithCachingBackend( _testSyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testSyncProfileName );

            try
            {
                var cachingClass = new TestSyncCachingClass();
                var currentId = 0;

                var cachedMethods =
                    new[]
                    {
                        cachingClass.GetValue,
                        () => cachingClass.GetValue( 0 ),
                        () => cachingClass.GetValue( 0, 0 ),
                        () => cachingClass.GetValue( 0, 0, 0 ),
                        () => cachingClass.GetValue( 1 ),
                        () => cachingClass.GetValue( 1, 0 ),
                        () => cachingClass.GetValue( 1, 0, 0 ),
                        () => cachingClass.GetValue( 0, 1 ),
                        () => cachingClass.GetValue( 0, 1, 0 ),
                        () => cachingClass.GetValue( 0, 0, 1 )
                    };

                for ( var i = 0; i < cachedMethods.Length; i++ )
                {
                    var value1 = cachedMethods[i].Invoke();
                    var called = cachingClass.Reset();
                    Assert.True( called, $"The method #{i} was not called on expected cache miss." );
                    AssertEx.Equal( currentId, value1.Id, $"The cached value of method #{i} has unexpected ID." );

                    currentId++;

                    var value2 = cachedMethods[i].Invoke();
                    called = cachingClass.Reset();
                    Assert.False( called, $"The method #{i} was called on expected cache hit." );

                    AssertEx.Equal(
                        value1,
                        value2,
                        $"The value of the method #{i}, which should be returned from the cache, is not the same as the one which should have been cached." );
                }
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        [Fact]
        public void TestInvalidateMethod()
        {
            this.InitializeTestWithCachingBackend( _testSyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testSyncProfileName );

            try
            {
                var cachingClass = new TestSyncCachingClass();
                const int currentId = 0;

                var value1 = cachingClass.GetValue();
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );
                cachingClass.Reset();

                CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue );

                var value2 = cachingClass.GetValue();
                var called = cachingClass.Reset();
                Assert.True( called, "The method was NOT called when its return value should NOT be cached." );
                Assert.NotEqual( value1, value2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestSync

        #region TestNotCacheKey

        private const string _testNotCacheKeyProfileName = _profileNamePrefix + "TestNotCacheKey";

        [CacheConfiguration( ProfileName = _testNotCacheKeyProfileName )]
        private sealed class TestNotCacheKeyCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue( [NotCacheKey] int param1 )
            {
                return base.GetValue( param1 );
            }

            [Cache]
            public override CachedValueClass GetValue( int param1, [NotCacheKey] int param2 )
            {
                return base.GetValue( param1, param2 );
            }
        }

        [Fact]
        public void TestNotCacheKey()
        {
            this.InitializeTestWithCachingBackend( _testNotCacheKeyProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testNotCacheKeyProfileName );

            try
            {
                var cachingClass = new TestNotCacheKeyCachingClass();
                var currentId = 0;

                var cachedMethods =
                    new[]
                    {
                        new[] { () => cachingClass.GetValue( 0 ), () => cachingClass.GetValue( 1 ) },
                        new[] { () => cachingClass.GetValue( 0, 0 ), () => cachingClass.GetValue( 0, 1 ) }
                    };

                for ( var group = 0; group < cachedMethods.Length; group++ )
                {
                    var value1 = cachedMethods[group][0].Invoke();
                    var called = cachingClass.Reset();
                    Assert.True( called, $"The first method of group #{group} was not called on expected cache miss." );
                    AssertEx.Equal( currentId, value1.Id, $"The cached value of the first method of group #{group} has unexpected ID." );
                    currentId++;

                    for ( var i = 0; i < cachedMethods.Length; i++ )
                    {
                        var value2 = cachedMethods[group][i].Invoke();
                        called = cachingClass.Reset();
                        Assert.False( called, $"The method #{i} from group #{group} was called on expected cache hit." );

                        AssertEx.Equal(
                            value1,
                            value2,
                            $"The value of the method #{i} from group #{group}, which should be returned from the cache, is not the same as the one which should have been cached." );
                    }
                }
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestNotCacheKey

        #region TestReturnsNull

        private const string _testReturnsNullProfileName = _profileNamePrefix + "TestReturnsNull";

        [Fact]
        public void TestReturnsNull()
        {
            this.InitializeTestWithCachingBackend( _testReturnsNullProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testReturnsNullProfileName );

            try
            {
                this._methodReturningNullInvocations = 0;
                var s = this.MethodReturningNull();
                var s2 = this.MethodReturningNull();

                Assert.Null( s );
                Assert.Null( s2 );
                Assert.Equal( 1, this._methodReturningNullInvocations );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private int _methodReturningNullInvocations;

        [Cache( ProfileName = _testReturnsNullProfileName )]
        private string? MethodReturningNull()
        {
            this._methodReturningNullInvocations++;

            return null;
        }

        #endregion

        #region TestSyncGeneric

        private const string _testSyncGenericProfileName = _profileNamePrefix + "TestSyncGeneric";

        [CacheConfiguration( ProfileName = _testSyncGenericProfileName )]
        private sealed class TestSyncGenericCachingClass<T> : CachingClass<T>
            where T : CachedValueClass, new()
        {
            [Cache]
            public override T GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestSyncGeneric()
        {
            this.InitializeTestWithCachingBackend( _testSyncGenericProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testSyncGenericProfileName );

            try
            {
                TestSyncGenericCachingClass<CachedValueClass> cachingClass = new();
                const int currentId = 0;

                var value1 = cachingClass.GetValue();
                var called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache should be empty." );
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );

                var value2 = cachingClass.GetValue();
                called = cachingClass.Reset();
                Assert.False( called, "The method was called when its return value should be cached." );

                AssertEx.Equal(
                    value1,
                    value2,
                    "The value, which should be returned from the cache, is not the same as the one which should have been cached." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestSyncGeneric

        #region TestAsync

        private const string _testAsyncProfileName = _profileNamePrefix + "TestAsync";

        [CacheConfiguration( ProfileName = _testAsyncProfileName )]
        private sealed class TestAsyncCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        private static async Task DoAsyncTest<T>( CachingClass<T> cachingClass, TestingCacheBackend backend )
            where T : CachedValueClass, new()
        {
            const int currentId = 0;

            var value = await cachingClass.GetValueAsync();

            var called = cachingClass.Reset();
            Assert.True( called );

            AssertEx.Equal( currentId, value.Id, "The first given value has unexpected ID." );

            var value2 = await cachingClass.GetValueAsync();
            called = cachingClass.Reset();
            Assert.False( called, "The cached method was called before awaiting the cached value." );

            Assert.False( called, "The cached method was called when awaiting the cached value." );

            AssertEx.Equal(
                value,
                value2,
                "The value, which should be returned from the cache, " +
                "is not the same as the one which should have been cached." );
        }

        [Fact]
        public async Task TestAsync()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testAsyncProfileName );

            try
            {
                var cachingClass = new TestAsyncCachingClass();

                await DoAsyncTest( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestAsync

        #region TestAsyncGeneric

        private const string _testAsyncGenericProfileName = _profileNamePrefix + "TestAsyncGeneric";

        [CacheConfiguration( ProfileName = _testAsyncGenericProfileName )]
        private sealed class TestAsyncGenericCachingClass<T> : CachingClass<T>
            where T : CachedValueClass, new()
        {
            [Cache]
            public override async Task<T> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestAsyncGeneric()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAsyncGenericProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testAsyncGenericProfileName );

            try
            {
                TestAsyncGenericCachingClass<CachedValueClass> cachingClass = new();

                await DoAsyncTest( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestAsyncGeneric

        #region TestDisabled

        private const string _testDisabledProfileName = _profileNamePrefix + "TestDisabled";

        [CacheConfiguration( ProfileName = _testDisabledProfileName )]
        private sealed class TestDisabledCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestDisabled()
        {
            this.InitializeTestWithCachingBackend( _testDisabledProfileName );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testDisabledProfileName );
            profile.IsEnabled = false;

            try
            {
                var cachingClass = new TestDisabledCachingClass();

                var value1 = cachingClass.GetValue();
                cachingClass.Reset();
                var value2 = cachingClass.GetValue();
                var called = cachingClass.Reset();
                Assert.True( called, "The method was not called when cached should be disabled." );

                AssertEx.NotEqual(
                    value1,
                    value2,
                    "The value, which should not be returned from the cache, is the same as the one which would have been cached." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestDisabled

        #region TestDisabledAsync

        private const string _testDisabledAsyncProfileName = _profileNamePrefix + "TestDisabledAsync";

        [CacheConfiguration( ProfileName = _testDisabledAsyncProfileName )]
        private sealed class TestDisabledAsyncCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestDisabledAsync()
        {
            this.InitializeTestWithCachingBackend( _testDisabledAsyncProfileName );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testDisabledAsyncProfileName );
            profile.IsEnabled = false;

            try
            {
                var cachingClass = new TestDisabledAsyncCachingClass();

                var value1 = await cachingClass.GetValueAsync();
                cachingClass.Reset();
                var value2 = await cachingClass.GetValueAsync();
                var called = cachingClass.Reset();
                Assert.True( called, "The method was not called when cached should be disabled." );

                AssertEx.NotEqual(
                    value1,
                    value2,
                    "The value, which should not be returned from the cache, is the same as the one which would have been cached." );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestDisabledAsync

        #region TestAutoReload

        private static void DoAutoReloadTest( CachingClass cachingClass, TestingCacheBackend backend )
        {
            var currentId = 0;

            backend.ExpectedGetCount = 1;
            backend.ExpectedSetCount = 1;
            var value1 = cachingClass.GetValueAsDependency();
            cachingClass.Reset();
            backend.ResetTest( "When calling the method the first time" );

            // The auto-refresh feature is asynchronous,
            // so we need to wait for the item to be set.
            var itemSetEvent = new ManualResetEventSlim( false );

            backend.ItemSet += ( sender, args ) =>
            {
                // We can't use assert here as any exception is swallowed
                // by the AutoReloadManager
                if ( !value1.Equals( args.Item.Value ) )
                {
                    itemSetEvent.Set();
                }
            };

            backend.ExpectedContainsKeyCount = 1;
            backend.ExpectedRemoveCount = 1;
            backend.ExpectedSetCount = 1;
            CachingServices.Default.Invalidation.Invalidate( value1 );

            var called = itemSetEvent.Wait( _timeout );
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: timeout." );

            called = cachingClass.Reset();
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: reset." );

            ++currentId;
            var value2 = cachingClass.GetValueAsDependency();
            called = cachingClass.Reset();
            Assert.False( called, "The method was called when the return value should be cached." );

            AssertEx.Equal( currentId, value2.Id, "The second given value has unexpected ID." );
        }

        private const string _testAutoReloadProfileName1 = _profileNamePrefix + "TestAutoReload1";

        [CacheConfiguration( ProfileName = _testAutoReloadProfileName1 )]
        private sealed class TestAutoReloadSetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValueAsDependency()
            {
                return base.GetValueAsDependency();
            }
        }

        [Fact]
        public void TestAutoReloadSetInProfile()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadProfileName1 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadProfileName1 );
            profile.AutoReload = true;

            var cachingClass =
                new TestAutoReloadSetInProfileUsingConfigurationAttributeCachingClass();

            try
            {
                DoAutoReloadTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const string _testAutoReloadProfileName2 = _profileNamePrefix + "TestAutoReload2";

        [CacheConfiguration( ProfileName = _testAutoReloadProfileName2, AutoReload = true )]
        private sealed class TestAutoReloadSetInCacheConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValueAsDependency()
            {
                return base.GetValueAsDependency();
            }
        }

        [Fact]
        public void TestAutoReloadSetInCacheConfigurationAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadProfileName2 );
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadProfileName2 );
            profile2.AutoReload = false;

            var cachingClass =
                new TestAutoReloadSetInCacheConfigurationAttributeCachingClass();

            try
            {
                DoAutoReloadTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const string _testAutoReloadProfileName3 = _profileNamePrefix + "TestAutoReload3";

        [CacheConfiguration( ProfileName = _testAutoReloadProfileName3 )]
        private sealed class TestAutoReloadSetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( AutoReload = true )]
            public override CachedValueClass GetValueAsDependency()
            {
                return base.GetValueAsDependency();
            }
        }

        [Fact]
        public void TestAutoReloadSetInCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadProfileName3 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadProfileName3 );
            profile.AutoReload = false;
            var cachingClass = new TestAutoReloadSetInCacheAttributeCachingClass();

            try
            {
                DoAutoReloadTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestAutoReload

        #region TestAutoReloadAsync

        private static async Task DoAutoReloadTestAsync( CachingClass cachingClass, TestingCacheBackend backend )
        {
            var currentId = 0;

            backend.ExpectedGetCount = 1;
            backend.ExpectedSetCount = 1;
            var value1 = await cachingClass.GetValueAsDependencyAsync();
            cachingClass.Reset();
            backend.ResetTest( "When calling the method the first time" );

            // The auto-refresh feature is asynchronous,
            // so we need to wait for the item to be set.
            var itemSetEvent = new TaskCompletionSource<bool>();

            backend.ItemSet += ( sender, args ) =>
            {
                // We can't use assert here as any exception is swallowed
                // by the AutoReloadManager
                if ( !value1.Equals( args.Item.Value ) )
                {
                    itemSetEvent.SetResult( true );
                }
            };

            backend.ExpectedContainsKeyCount = 1;
            backend.ExpectedRemoveCount = 1;
            backend.ExpectedSetCount = 1;
            await CachingServices.Default.Invalidation.InvalidateAsync( value1 );

            var called = await Task.WhenAny( itemSetEvent.Task, Task.Delay( _timeout ) ) == itemSetEvent.Task;
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: timeout." );

            called = cachingClass.Reset();
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: reset." );

            ++currentId;
            var value2 = await cachingClass.GetValueAsDependencyAsync();
            called = cachingClass.Reset();
            Assert.False( called, "The method was called when the return value should be cached." );

            AssertEx.Equal( currentId, value2.Id, "The second given value has unexpected ID." );
        }

        private const string _testAutoReloadAsyncProfileName1 = _profileNamePrefix + "TestAutoReloadAsync1";

        [CacheConfiguration( ProfileName = _testAutoReloadAsyncProfileName1 )]
        private sealed class TestAutoReloadAsyncSetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task TestAutoReloadAsyncSetInProfile()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadAsyncProfileName1 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadAsyncProfileName1 );
            profile.AutoReload = true;

            var cachingClass =
                new TestAutoReloadAsyncSetInProfileUsingConfigurationAttributeCachingClass();

            try
            {
                await DoAutoReloadTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testAutoReloadAsyncProfileName2 = _profileNamePrefix + "TestAutoReloadAsync2";

        [CacheConfiguration( ProfileName = _testAutoReloadAsyncProfileName2, AutoReload = true )]
        private sealed class TestAutoReloadAsyncSetInCacheConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task TestAutoReloadAsyncSetInCacheConfigurationAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadAsyncProfileName2 );
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadAsyncProfileName2 );
            profile2.AutoReload = false;

            var cachingClass =
                new TestAutoReloadAsyncSetInCacheConfigurationAttributeCachingClass();

            try
            {
                await DoAutoReloadTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testAutoReloadAsyncProfileName3 = _profileNamePrefix + "TestAutoReloadAsync3";

        [CacheConfiguration( ProfileName = _testAutoReloadAsyncProfileName3 )]
        private sealed class TestAutoReloadAsyncSetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( AutoReload = true )]
            public override async Task<CachedValueClass> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task AutoReload_ReproducingDeadlock()
        {
            await this.TestAutoReloadAsyncSetInCacheAttribute();
            this.TestAutoReloadSetInCacheConfigurationAttribute();
        }

        [Fact]
        public async Task TestAutoReloadAsyncSetInCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAutoReloadAsyncProfileName3 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testAutoReloadAsyncProfileName3 );
            profile.AutoReload = false;
            var cachingClass = new TestAutoReloadAsyncSetInCacheAttributeCachingClass();

            try
            {
                await DoAutoReloadTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestAutoReloadAsync

        #region TestAsyncContext

        [Fact]
        public async Task TestAsyncContext()
        {
            _ = this.InitializeTestWithTestingBackend( _testAsyncGenericProfileName );

            try
            {
                await this.CachedAsyncMethod1();
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        [Cache]
        private async Task<string> CachedAsyncMethod1()
        {
            var context = CachingContext.Current;

            for ( var i = 0; i < 10; i++ )
            {
                await Task.Yield();
                Assert.Same( context, CachingContext.Current );
            }

            await NonCachedAsyncMethod2( CachingContext.Current );

            return "";
        }

        private static async Task<string> NonCachedAsyncMethod2( ICachingContext parentContext )
        {
            for ( var i = 0; i < 10; i++ )
            {
                Assert.Same( parentContext, CachingContext.Current );
                await Task.Yield();
            }

            await CachedAsyncMethod3( parentContext );

            return "";
        }

        [Cache]
        private static async Task<string> CachedAsyncMethod3( ICachingContext parentContext )
        {
            for ( var i = 0; i < 10; i++ )
            {
                Assert.Same( parentContext, CachingContext.Current.Parent );
                await Task.Yield();
            }

            return "";
        }

        #endregion

        #region TestAbsoluteExpiration

        private const double _absoluteExpirationOffsetTestValue1 = 1.1;
        private static readonly TimeSpan _absoluteExpirationTestValue1 = TimeSpan.FromMinutes( _absoluteExpirationOffsetTestValue1 );

        private const double _absoluteExpirationOffsetTestValue2 = 1.2;
        private static readonly TimeSpan _absoluteExpirationTestValue2 = TimeSpan.FromMinutes( _absoluteExpirationOffsetTestValue2 );

        private const double _absoluteExpirationOffsetTestValue3 = 1.3;
        private static readonly TimeSpan _absoluteExpirationTestValue3 = TimeSpan.FromMinutes( _absoluteExpirationOffsetTestValue3 );

        private const string _testAbsoluteExpirationProfileName1 = _profileNamePrefix + "TestAbsoluteExpiration1";
        private const string _testAbsoluteExpirationProfileName2 = _profileNamePrefix + "TestAbsoluteExpiration2";

        [CacheConfiguration( ProfileName = _testAbsoluteExpirationProfileName1 )]
        private sealed class TestAbsoluteExpirationSetInProfileAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue1()
            {
                return null!;
            }

            [Cache( ProfileName = _testAbsoluteExpirationProfileName2 )]
            public object GetValue2()
            {
                return null!;
            }

            [Cache( AbsoluteExpiration = _absoluteExpirationOffsetTestValue3 )]
            public object GetValue3()
            {
                return null!;
            }
        }

        [Fact]
        public void TestAbsoluteExpirationSetInProfileAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAbsoluteExpirationProfileName1 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationProfileName1 );
            profile1.AbsoluteExpiration = _absoluteExpirationTestValue1;
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationProfileName2 );
            profile2.AbsoluteExpiration = _absoluteExpirationTestValue2;
            var cachingClass = new TestAbsoluteExpirationSetInProfileAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue1();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( _absoluteExpirationTestValue1, backend.LastCachedItem.Configuration!.AbsoluteExpiration );

                cachingClass.GetValue2();
                Assert.Equal( _absoluteExpirationTestValue2, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                cachingClass.GetValue3();
                Assert.Equal( _absoluteExpirationTestValue3, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const double _absoluteExpirationOffsetTestValue4 = 1.4;
        private static readonly TimeSpan _absoluteExpirationTestValue4 = TimeSpan.FromMinutes( _absoluteExpirationOffsetTestValue4 );

        private const double _absoluteExpirationOffsetTestValue5 = 1.5;
        private static readonly TimeSpan _absoluteExpirationTestValue5 = TimeSpan.FromMinutes( _absoluteExpirationOffsetTestValue5 );

        private const string _testAbsoluteExpirationProfileName4 = _profileNamePrefix + "TestAbsoluteExpiration4";

        [CacheConfiguration( ProfileName = _testAbsoluteExpirationProfileName4, AbsoluteExpiration = _absoluteExpirationOffsetTestValue4 )]
        private sealed class TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue4()
            {
                return null!;
            }

            [Cache( AbsoluteExpiration = _absoluteExpirationOffsetTestValue5 )]
            public object GetValue5()
            {
                return null!;
            }
        }

        [Fact]
        public void TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAbsoluteExpirationProfileName4 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationProfileName4 );
            profile1.AbsoluteExpiration = null;

            var cachingClass =
                new TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue4();
                Assert.Equal( _absoluteExpirationTestValue4, backend.LastCachedItem!.Configuration!.AbsoluteExpiration );

                cachingClass.GetValue5();
                Assert.Equal( _absoluteExpirationTestValue5, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestAbsoluteExpiration

        #region TestAbsoluteExpirationAsync

        private const string _testAbsoluteExpirationAsyncProfileName1 = _profileNamePrefix + "TestAbsoluteExpirationAsync1";
        private const string _testAbsoluteExpirationAsyncProfileName2 = _profileNamePrefix + "TestAbsoluteExpirationAsync2";

        [CacheConfiguration( ProfileName = _testAbsoluteExpirationAsyncProfileName1 )]
        private sealed class TestAbsoluteExpirationAsyncSetInProfileAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue1Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( ProfileName = _testAbsoluteExpirationAsyncProfileName2 )]
            public async Task<object> GetValue2Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( AbsoluteExpiration = _absoluteExpirationOffsetTestValue3 )]
            public async Task<object> GetValue3Async()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestAbsoluteExpirationAsyncSetInProfileAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAbsoluteExpirationAsyncProfileName1 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationAsyncProfileName1 );
            profile1.AbsoluteExpiration = _absoluteExpirationTestValue1;
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationAsyncProfileName2 );
            profile2.AbsoluteExpiration = _absoluteExpirationTestValue2;

            var cachingClass =
                new TestAbsoluteExpirationAsyncSetInProfileAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue1Async();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( _absoluteExpirationTestValue1, backend.LastCachedItem!.Configuration!.AbsoluteExpiration );

                await cachingClass.GetValue2Async();
                Assert.Equal( _absoluteExpirationTestValue2, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                await cachingClass.GetValue3Async();
                Assert.Equal( _absoluteExpirationTestValue3, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testAbsoluteExpirationAsyncProfileName4 = _profileNamePrefix + "TestAbsoluteExpirationAsync4";

        [CacheConfiguration( ProfileName = _testAbsoluteExpirationAsyncProfileName4, AbsoluteExpiration = _absoluteExpirationOffsetTestValue4 )]
        private sealed class TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue4Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( AbsoluteExpiration = _absoluteExpirationOffsetTestValue5 )]
            public async Task<object> GetValue5Async()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testAbsoluteExpirationAsyncProfileName4 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testAbsoluteExpirationAsyncProfileName4 );
            profile1.AbsoluteExpiration = null;

            var cachingClass =
                new TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue4Async();
                Assert.Equal( _absoluteExpirationTestValue4, backend.LastCachedItem!.Configuration!.AbsoluteExpiration );

                await cachingClass.GetValue5Async();
                Assert.Equal( _absoluteExpirationTestValue5, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestAbsoluteExpirationAsync

        #region TestSlidingExpiration

        private const double _slidingExpirationOffsetTestDoubleValue1 = 1.1;
        private static readonly TimeSpan _slidingExpirationOffsetTestTimeSpanValue1 = TimeSpan.FromMinutes( _slidingExpirationOffsetTestDoubleValue1 );

        private const double _slidingExpirationOffsetTestDoubleValue2 = 1.2;
        private static readonly TimeSpan _slidingExpirationOffsetTestTimeSpanValue2 = TimeSpan.FromMinutes( _slidingExpirationOffsetTestDoubleValue2 );

        private const double _slidingExpirationOffsetTestDoubleValue3 = 1.3;
        private static readonly TimeSpan _slidingExpirationOffsetTestTimeSpanValue3 = TimeSpan.FromMinutes( _slidingExpirationOffsetTestDoubleValue3 );

        private const string _testSlidingExpirationProfileName1 = _profileNamePrefix + "TestSlidingExpiration1";
        private const string _testSlidingExpirationProfileName2 = _profileNamePrefix + "TestSlidingExpiration2";

        [CacheConfiguration( ProfileName = _testSlidingExpirationProfileName1 )]
        private sealed class TestSlidingExpirationSetInProfileCachingClass
        {
            [Cache]
            public object GetValue1()
            {
                return null!;
            }

            [Cache( ProfileName = _testSlidingExpirationProfileName2 )]
            public object GetValue2()
            {
                return null!;
            }

            [Cache( SlidingExpiration = _slidingExpirationOffsetTestDoubleValue3 )]
            public object GetValue3()
            {
                return null!;
            }
        }

        [Fact]
        public void TestSlidingExpiration()
        {
            var backend = this.InitializeTestWithTestingBackend( _testSlidingExpirationProfileName1 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationProfileName1 );
            profile1.SlidingExpiration = _slidingExpirationOffsetTestTimeSpanValue1;
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationProfileName2 );
            profile2.SlidingExpiration = _slidingExpirationOffsetTestTimeSpanValue2;
            var cachingClass = new TestSlidingExpirationSetInProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue1();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue1, backend.LastCachedItem!.Configuration!.SlidingExpiration );

                cachingClass.GetValue2();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue2, backend.LastCachedItem.Configuration.SlidingExpiration );

                cachingClass.GetValue3();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue3, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const double _slidingExpirationOffsetTestDoubleValue4 = 1.4;
        private static readonly TimeSpan _slidingExpirationOffsetTestTimeSpanValue4 = TimeSpan.FromMinutes( _slidingExpirationOffsetTestDoubleValue4 );

        private const double _slidingExpirationOffsetTestDoubleValue5 = 1.5;
        private static readonly TimeSpan _slidingExpirationOffsetTestTimeSpanValue5 = TimeSpan.FromMinutes( _slidingExpirationOffsetTestDoubleValue5 );

        private const string _testSlidingExpirationProfileName4 = _profileNamePrefix + "TestSlidingExpiration4";

        [CacheConfiguration( ProfileName = _testSlidingExpirationProfileName4, SlidingExpiration = _slidingExpirationOffsetTestDoubleValue4 )]
        private sealed class TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue4()
            {
                return null!;
            }

            [Cache( SlidingExpiration = _slidingExpirationOffsetTestDoubleValue5 )]
            public object GetValue5()
            {
                return null!;
            }
        }

        [Fact]
        public void TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testSlidingExpirationProfileName4 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationProfileName4 );
            profile1.SlidingExpiration = null;

            var cachingClass =
                new TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue4();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue4, backend.LastCachedItem!.Configuration!.SlidingExpiration );

                cachingClass.GetValue5();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue5, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestSlidingExpiration

        #region TestSlidingExpirationAsync

        private const string _testSlidingExpirationAsyncProfileName1 = _profileNamePrefix + "TestSlidingExpirationAsync1";
        private const string _testSlidingExpirationAsyncProfileName2 = _profileNamePrefix + "TestSlidingExpirationAsync2";

        [CacheConfiguration( ProfileName = _testSlidingExpirationAsyncProfileName1 )]
        private sealed class TestSlidingExpirationAsyncSetInProfileCachingClass
        {
            [Cache]
            public async Task<object> GetValue1Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( ProfileName = _testSlidingExpirationAsyncProfileName2 )]
            public async Task<object> GetValue2Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( SlidingExpiration = _slidingExpirationOffsetTestDoubleValue3 )]
            public async Task<object> GetValue3Async()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestSlidingExpirationAsync()
        {
            var backend = this.InitializeTestWithTestingBackend( _testSlidingExpirationAsyncProfileName1 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationAsyncProfileName1 );
            profile1.SlidingExpiration = _slidingExpirationOffsetTestTimeSpanValue1;
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationAsyncProfileName2 );
            profile2.SlidingExpiration = _slidingExpirationOffsetTestTimeSpanValue2;
            var cachingClass = new TestSlidingExpirationAsyncSetInProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue1Async();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue1, backend.LastCachedItem!.Configuration!.SlidingExpiration );

                await cachingClass.GetValue2Async();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue2, backend.LastCachedItem.Configuration.SlidingExpiration );

                await cachingClass.GetValue3Async();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue3, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testSlidingExpirationAsyncProfileName4 = _profileNamePrefix + "TestSlidingExpirationAsync4";

        [CacheConfiguration( ProfileName = _testSlidingExpirationAsyncProfileName4, SlidingExpiration = _slidingExpirationOffsetTestDoubleValue4 )]
        private sealed class TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue4Async()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( SlidingExpiration = _slidingExpirationOffsetTestDoubleValue5 )]
            public async Task<object> GetValue5Async()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testSlidingExpirationAsyncProfileName4 );
            var profile1 = TestProfileConfigurationFactory.CreateProfile( _testSlidingExpirationAsyncProfileName4 );
            profile1.SlidingExpiration = null;

            var cachingClass =
                new TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue4Async();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue4, backend.LastCachedItem!.Configuration!.SlidingExpiration );

                await cachingClass.GetValue5Async();
                Assert.Equal( _slidingExpirationOffsetTestTimeSpanValue5, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestSlidingExpirationAsync

        #region TestCacheItemPriority

        private static void DoCacheItemPriorityTest( CachingClass cachingClass, TestingCacheBackend backend )
        {
            Assert.Null( backend.LastCachedKey );
            Assert.Null( backend.LastCachedItem );

            cachingClass.GetValue();

            Assert.NotNull( backend.LastCachedKey );
            Assert.NotNull( backend.LastCachedItem );

            Assert.Equal( CacheItemPriority.NotRemovable, backend.LastCachedItem!.Configuration!.Priority );
        }

        private const string _testCacheItemPriorityProfileName1 = _profileNamePrefix + "TestCacheItemPriority1";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityProfileName1 )]
        private sealed class TestCacheItemPrioritySetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return null!;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInProfile()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityProfileName1 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityProfileName1 );
            profile.Priority = CacheItemPriority.NotRemovable;

            var cachingClass =
                new TestCacheItemPrioritySetInProfileUsingConfigurationAttributeCachingClass();

            try
            {
                DoCacheItemPriorityTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const string _testCacheItemPriorityProfileName2 = _profileNamePrefix + "TestCacheItemPriority2";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityProfileName2, Priority = CacheItemPriority.NotRemovable )]
        private sealed class TestCacheItemPrioritySetInProfileUsingCacheAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return null!;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInCacheConfigurationAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityProfileName2 );
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityProfileName2 );
            profile2.Priority = CacheItemPriority.Default;

            var cachingClass =
                new TestCacheItemPrioritySetInProfileUsingCacheAttributeCachingClass();

            try
            {
                DoCacheItemPriorityTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const string _testCacheItemPriorityProfileName3 = _profileNamePrefix + "TestCacheItemPriority3";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityProfileName3 )]
        private sealed class TestCacheItemPrioritySetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( Priority = CacheItemPriority.NotRemovable )]
            public override CachedValueClass GetValue()
            {
                return null!;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityProfileName3 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityProfileName3 );
            profile.Priority = CacheItemPriority.Default;
            var cachingClass = new TestCacheItemPrioritySetInCacheAttributeCachingClass();

            try
            {
                DoCacheItemPriorityTest( cachingClass, backend );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestCacheItemPriority

        #region TestCacheItemPriorityAsync

        private static async Task DoCacheItemPriorityTestAsync( CachingClass cachingClass, TestingCacheBackend backend )
        {
            Assert.Null( backend.LastCachedKey );
            Assert.Null( backend.LastCachedItem );

            await cachingClass.GetValueAsync();

            Assert.NotNull( backend.LastCachedKey );
            Assert.NotNull( backend.LastCachedItem );

            Assert.Equal( CacheItemPriority.NotRemovable, backend.LastCachedItem!.Configuration!.Priority );
        }

        private const string _testCacheItemPriorityAsyncProfileName1 = _profileNamePrefix + "TestCacheItemPriorityAsync1";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityAsyncProfileName1 )]
        private sealed class TestCacheItemPriorityAsyncSetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInProfile()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityAsyncProfileName1 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityAsyncProfileName1 );
            profile.Priority = CacheItemPriority.NotRemovable;

            var cachingClass =
                new TestCacheItemPriorityAsyncSetInProfileUsingConfigurationAttributeCachingClass();

            try
            {
                await DoCacheItemPriorityTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testCacheItemPriorityAsyncProfileName2 = _profileNamePrefix + "TestCacheItemPriorityAsync2";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityAsyncProfileName2, Priority = CacheItemPriority.NotRemovable )]
        private sealed class TestCacheItemPriorityAsyncSetInProfileUsingCacheAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInCacheConfigurationAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityAsyncProfileName2 );
            var profile2 = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityAsyncProfileName2 );
            profile2.Priority = CacheItemPriority.Default;

            var cachingClass =
                new TestCacheItemPriorityAsyncSetInProfileUsingCacheAttributeCachingClass();

            try
            {
                await DoCacheItemPriorityTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testCacheItemPriorityAsyncProfileName3 = _profileNamePrefix + "TestCacheItemPriorityAsync3";

        [CacheConfiguration( ProfileName = _testCacheItemPriorityAsyncProfileName3 )]
        private sealed class TestCacheItemPriorityAsyncSetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( Priority = CacheItemPriority.NotRemovable )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInCacheAttribute()
        {
            var backend = this.InitializeTestWithTestingBackend( _testCacheItemPriorityAsyncProfileName3 );
            var profile = TestProfileConfigurationFactory.CreateProfile( _testCacheItemPriorityAsyncProfileName3 );
            profile.Priority = CacheItemPriority.Default;
            var cachingClass = new TestCacheItemPriorityAsyncSetInCacheAttributeCachingClass();

            try
            {
                await DoCacheItemPriorityTestAsync( cachingClass, backend );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestCacheItemPriorityAsync

        #region TestIgnoreThisParameter

        // The "this" parameter is distinguished using ToString by default.
        private abstract class NamedCachingClass : CachingClass
        {
            // ReSharper disable once MemberCanBePrivate.Local
            public string Name { get; }

            protected NamedCachingClass( string name )
            {
                this.Name = name;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }

        private static void DoIgnoreThisParameterTest(
            NamedCachingClass cachingClassNotIgnoringThisParameter1,
            NamedCachingClass cachingClassNotIgnoringThisParameter2,
            NamedCachingClass cachingClassIgnoringThisParameter1,
            NamedCachingClass cachingClassIgnoringThisParameter2 )
        {
            cachingClassNotIgnoringThisParameter1.GetValue();
            var called = cachingClassNotIgnoringThisParameter1.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassNotIgnoringThisParameter1)}." );

            cachingClassNotIgnoringThisParameter2.GetValue();
            called = cachingClassNotIgnoringThisParameter2.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassNotIgnoringThisParameter2)}." );

            cachingClassIgnoringThisParameter1.GetValue();
            called = cachingClassIgnoringThisParameter1.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassIgnoringThisParameter1)}." );

            cachingClassIgnoringThisParameter2.GetValue();
            called = cachingClassIgnoringThisParameter2.Reset();
            Assert.False( called, $"The cached method was called for {nameof(cachingClassIgnoringThisParameter2)}." );
        }

        private const string _testIgnoreThisParameterProfileName1 = _profileNamePrefix + "TestIgnoreThisParameter1";

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterProfileName1 )]
        private sealed class CachingClassNotIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public CachingClassNotIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterProfileName1 )]
        private sealed class CachingClassIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public CachingClassIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache( IgnoreThisParameter = true )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestIgnoreThisParameterOffsetSetInCacheAttribute()
        {
            this.InitializeTestWithTestingBackend( _testIgnoreThisParameterProfileName1 );
            TestProfileConfigurationFactory.CreateProfile( _testIgnoreThisParameterProfileName1 );

            CachingClassNotIgnoringThisParameterSetInCacheAttribute
                cachingClassNotIgnoringThisParameter1 = new( nameof(cachingClassNotIgnoringThisParameter1) );

            CachingClassNotIgnoringThisParameterSetInCacheAttribute
                cachingClassNotIgnoringThisParameter2 = new( nameof(cachingClassNotIgnoringThisParameter2) );

            CachingClassIgnoringThisParameterSetInCacheAttribute cachingClassIgnoringThisParameter1 = new( nameof(cachingClassIgnoringThisParameter1) );
            CachingClassIgnoringThisParameterSetInCacheAttribute cachingClassIgnoringThisParameter2 = new( nameof(cachingClassIgnoringThisParameter2) );

            try
            {
                DoIgnoreThisParameterTest(
                    cachingClassNotIgnoringThisParameter1,
                    cachingClassNotIgnoringThisParameter2,
                    cachingClassIgnoringThisParameter1,
                    cachingClassIgnoringThisParameter2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const string _testIgnoreThisParameterProfileName2 = _profileNamePrefix + "TestIgnoreThisParameter2";

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterProfileName2 )]
        private sealed class CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterProfileName2, IgnoreThisParameter = true )]
        private sealed class CachingClassIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public CachingClassIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestIgnoreThisParameterOffsetSetInCacheConfigurationAttribute()
        {
            this.InitializeTestWithTestingBackend( _testIgnoreThisParameterProfileName2 );
            TestProfileConfigurationFactory.CreateProfile( _testIgnoreThisParameterProfileName2 );

            CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassNotIgnoringThisParameter1 =
                new( nameof(cachingClassNotIgnoringThisParameter1) );

            CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassNotIgnoringThisParameter2 =
                new( nameof(cachingClassNotIgnoringThisParameter2) );

            CachingClassIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassIgnoringThisParameter1 =
                new( nameof(cachingClassIgnoringThisParameter1) );

            CachingClassIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassIgnoringThisParameter2 =
                new( nameof(cachingClassIgnoringThisParameter2) );

            try
            {
                DoIgnoreThisParameterTest(
                    cachingClassNotIgnoringThisParameter1,
                    cachingClassNotIgnoringThisParameter2,
                    cachingClassIgnoringThisParameter1,
                    cachingClassIgnoringThisParameter2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestIgnoreThisParameter

        #region TestIgnoreThisParameterAsync

        private static async Task DoIgnoreThisParameterTestAsync(
            NamedCachingClass cachingClassNotIgnoringThisParameter1,
            NamedCachingClass cachingClassNotIgnoringThisParameter2,
            NamedCachingClass cachingClassIgnoringThisParameter1,
            NamedCachingClass cachingClassIgnoringThisParameter2 )
        {
            await cachingClassNotIgnoringThisParameter1.GetValueAsync();
            var called = cachingClassNotIgnoringThisParameter1.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassNotIgnoringThisParameter1)}." );

            await cachingClassNotIgnoringThisParameter2.GetValueAsync();
            called = cachingClassNotIgnoringThisParameter2.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassNotIgnoringThisParameter2)}." );

            await cachingClassIgnoringThisParameter1.GetValueAsync();
            called = cachingClassIgnoringThisParameter1.Reset();
            Assert.True( called, $"The cached method was not called for {nameof(cachingClassIgnoringThisParameter1)}." );

            await cachingClassIgnoringThisParameter2.GetValueAsync();
            called = cachingClassIgnoringThisParameter2.Reset();
            Assert.False( called, $"The cached method was called for {nameof(cachingClassIgnoringThisParameter2)}." );
        }

        private const string _testIgnoreThisParameterAsyncProfileName1 = _profileNamePrefix + "TestIgnoreThisParameterAsync1";

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterAsyncProfileName1 )]
        private sealed class AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterAsyncProfileName1 )]
        private sealed class AsyncCachingClassIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public AsyncCachingClassIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache( IgnoreThisParameter = true )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestIgnoreThisParameterAsyncOffsetSetInCacheAttribute()
        {
            this.InitializeTestWithTestingBackend( _testIgnoreThisParameterAsyncProfileName1 );
            TestProfileConfigurationFactory.CreateProfile( _testIgnoreThisParameterAsyncProfileName1 );

            AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute cachingClassNotIgnoringThisParameter1 =
                new( nameof(cachingClassNotIgnoringThisParameter1) );

            AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute cachingClassNotIgnoringThisParameter2 =
                new( nameof(cachingClassNotIgnoringThisParameter2) );

            AsyncCachingClassIgnoringThisParameterSetInCacheAttribute cachingClassIgnoringThisParameter1 = new( nameof(cachingClassIgnoringThisParameter1) );
            AsyncCachingClassIgnoringThisParameterSetInCacheAttribute cachingClassIgnoringThisParameter2 = new( nameof(cachingClassIgnoringThisParameter2) );

            try
            {
                await DoIgnoreThisParameterTestAsync(
                    cachingClassNotIgnoringThisParameter1,
                    cachingClassNotIgnoringThisParameter2,
                    cachingClassIgnoringThisParameter1,
                    cachingClassIgnoringThisParameter2 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string _testIgnoreThisParameterAsyncProfileName2 = _profileNamePrefix + "TestIgnoreThisParameterAsync2";

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterAsyncProfileName2 )]
        private sealed class AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [CacheConfiguration( ProfileName = _testIgnoreThisParameterAsyncProfileName2, IgnoreThisParameter = true )]
        private sealed class AsyncCachingClassIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public AsyncCachingClassIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestIgnoreThisParameterAsyncOffsetSetInCacheConfigurationAttribute()
        {
            this.InitializeTestWithTestingBackend( _testIgnoreThisParameterAsyncProfileName2 );
            TestProfileConfigurationFactory.CreateProfile( _testIgnoreThisParameterAsyncProfileName2 );

            AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassNotIgnoringThisParameter1 =
                new( nameof(cachingClassNotIgnoringThisParameter1) );

            AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassNotIgnoringThisParameter2 =
                new( nameof(cachingClassNotIgnoringThisParameter2) );

            AsyncCachingClassIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassIgnoringThisParameter1 =
                new( nameof(cachingClassIgnoringThisParameter1) );

            AsyncCachingClassIgnoringThisParameterSetInCacheConfigurationAttribute cachingClassIgnoringThisParameter2 =
                new( nameof(cachingClassIgnoringThisParameter2) );

            try
            {
                await DoIgnoreThisParameterTestAsync(
                    cachingClassNotIgnoringThisParameter1,
                    cachingClassNotIgnoringThisParameter2,
                    cachingClassIgnoringThisParameter1,
                    cachingClassIgnoringThisParameter2 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestIgnoreThisParameterAsync

        #region TestMethodLevelProfile

        private const string _testMethodLevelProfileNameSetInConfigurationAttribute = _profileNamePrefix + "MethodLevelProfileNameSetInConfigurationAttribute";
        private const string _testMethodLevelProfileNameSetInCacheAttribute = _profileNamePrefix + "MethodLevelProfileNameSetInCacheAttribute";

        [CacheConfiguration( ProfileName = _testMethodLevelProfileNameSetInConfigurationAttribute )]
        private sealed class TestMethodLevelProfileCachingClass
        {
            [Cache]
            public object GetValueUsingSetInConfigurationAttribute()
            {
                return null!;
            }

            [Cache( ProfileName = _testMethodLevelProfileNameSetInCacheAttribute )]
            public object GetValueUsingSetInCacheAttribute()
            {
                return null!;
            }
        }

        [Fact]
        public void TestMethodLevelProfile()
        {
            var backend =
                this.InitializeTestWithTestingBackend( _testMethodLevelProfileNameSetInConfigurationAttribute );

            TestProfileConfigurationFactory.CreateProfile( _testMethodLevelProfileNameSetInConfigurationAttribute );
            TestProfileConfigurationFactory.CreateProfile( _testMethodLevelProfileNameSetInCacheAttribute );
            var cachingClass = new TestMethodLevelProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValueUsingSetInConfigurationAttribute();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( _testMethodLevelProfileNameSetInConfigurationAttribute, backend.LastCachedItem!.Configuration!.ProfileName );

                var firstCachedKey = backend.LastCachedKey;

                cachingClass.GetValueUsingSetInCacheAttribute();

                Assert.NotEqual( firstCachedKey, backend.LastCachedKey );

                Assert.Equal( _testMethodLevelProfileNameSetInCacheAttribute, backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestMethodLevelProfileAsync

        private const string _testMethodLevelProfileAsyncNameSetInConfigurationAttribute =
            _profileNamePrefix + "MethodLevelProfileNameSetInConfigurationAttribute";

        private const string _testMethodLevelProfileAsyncNameSetInCacheAttribute = _profileNamePrefix + "MethodLevelProfileNameSetInCacheAttribute";

        [CacheConfiguration( ProfileName = _testMethodLevelProfileAsyncNameSetInConfigurationAttribute )]
        private sealed class TestMethodLevelProfileAsyncCachingClass
        {
            [Cache]
            public async Task<object> GetValueUsingSetInConfigurationAttributeAsync()
            {
                await Task.Yield();

                return null!;
            }

            [Cache( ProfileName = _testMethodLevelProfileAsyncNameSetInCacheAttribute )]
            public async Task<object> GetValueUsingSetInCacheAttributeAsync()
            {
                await Task.Yield();

                return null!;
            }
        }

        [Fact]
        public async Task TestMethodLevelProfileAsync()
        {
            var backend =
                this.InitializeTestWithTestingBackend( _testMethodLevelProfileAsyncNameSetInConfigurationAttribute );

            TestProfileConfigurationFactory.CreateProfile( _testMethodLevelProfileAsyncNameSetInConfigurationAttribute );
            TestProfileConfigurationFactory.CreateProfile( _testMethodLevelProfileAsyncNameSetInCacheAttribute );
            var cachingClass = new TestMethodLevelProfileAsyncCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValueUsingSetInConfigurationAttributeAsync();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( _testMethodLevelProfileAsyncNameSetInConfigurationAttribute, backend.LastCachedItem!.Configuration!.ProfileName );

                var firstCachedKey = backend.LastCachedKey;

                await cachingClass.GetValueUsingSetInCacheAttributeAsync();

                Assert.NotEqual( firstCachedKey, backend.LastCachedKey );

                Assert.Equal( _testMethodLevelProfileAsyncNameSetInCacheAttribute, backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion
    }
}