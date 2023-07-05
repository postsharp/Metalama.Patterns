// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using CacheItemPriority = Metalama.Patterns.Caching.Implementation.CacheItemPriority;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class CacheAttributeTests
    {
        // We don't like timeouts in tests but we need them to avoid test suites to hang in case of issues.
        private static TimeSpan timeout = TimeSpan.FromMinutes( 2 );

        private const string profileNamePrefix = "Caching.Tests.CacheAttributeTests_";

        #region TestSync

        private const string testSyncProfileName = profileNamePrefix + "TestSync";

        [CacheConfiguration( ProfileName = testSyncProfileName )]
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testSyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testSyncProfileName );

            try
            {
                var cachingClass = new TestSyncCachingClass();
                var currentId = 0;

                Func<CachedValueClass>[] cachedMethods =
                    new Func<CachedValueClass>[]
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
                    CachedValueClass value1 = cachedMethods[i].Invoke();
                    bool called = cachingClass.Reset();
                    Assert.True( called, $"The method #{i} was not called on expected cache miss." );
                    AssertEx.Equal( currentId, value1.Id, $"The cached value of method #{i} has unexpected ID." );

                    currentId++;

                    CachedValueClass value2 = cachedMethods[i].Invoke();
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testSyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testSyncProfileName );

            try
            {
                var cachingClass = new TestSyncCachingClass();
                const int currentId = 0;

                CachedValueClass value1 = cachingClass.GetValue();
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );
                cachingClass.Reset();

                CachingServices.Invalidation.Invalidate( cachingClass.GetValue );

                CachedValueClass value2 = cachingClass.GetValue();
                bool called = cachingClass.Reset();
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

        private const string TestNotCacheKeyProfileName = profileNamePrefix + "TestNotCacheKey";

        [CacheConfiguration( ProfileName = TestNotCacheKeyProfileName )]
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( TestNotCacheKeyProfileName );
            TestProfileConfigurationFactory.CreateProfile( TestNotCacheKeyProfileName );

            try
            {
                var cachingClass = new TestNotCacheKeyCachingClass();
                var currentId = 0;

                Func<CachedValueClass>[][] cachedMethods =
                    new[]
                    {
                        new Func<CachedValueClass>[] { () => cachingClass.GetValue( 0 ), () => cachingClass.GetValue( 1 ) },
                        new Func<CachedValueClass>[] { () => cachingClass.GetValue( 0, 0 ), () => cachingClass.GetValue( 0, 1 ) }
                    };

                for ( var group = 0; group < cachedMethods.Length; group++ )
                {
                    CachedValueClass value1 = cachedMethods[group][0].Invoke();
                    bool called = cachingClass.Reset();
                    Assert.True( called, $"The first method of group #{group} was not called on expected cache miss." );
                    AssertEx.Equal( currentId, value1.Id, $"The cached value of the first method of group #{group} has unexpected ID." );
                    currentId++;

                    for ( var i = 0; i < cachedMethods.Length; i++ )
                    {
                        CachedValueClass value2 = cachedMethods[group][i].Invoke();
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

        private const string testReturnsNullProfileName = profileNamePrefix + "TestReturnsNull";

        [Fact]
        public void TestReturnsNull()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testReturnsNullProfileName );
            TestProfileConfigurationFactory.CreateProfile( testReturnsNullProfileName );

            try
            {
                this.methodReturningNullInvocations = 0;
                var s = this.MethodReturningNull();
                var s2 = this.MethodReturningNull();

                Assert.Null( s );
                Assert.Null( s2 );
                Assert.Equal( 1, this.methodReturningNullInvocations );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private int methodReturningNullInvocations;

        [Cache( ProfileName = testReturnsNullProfileName )]
        private string MethodReturningNull()
        {
            this.methodReturningNullInvocations++;

            return null;
        }

        #endregion

        #region TestSyncGeneric

        private const string testSyncGenericProfileName = profileNamePrefix + "TestSyncGeneric";

        [CacheConfiguration( ProfileName = testSyncGenericProfileName )]
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testSyncGenericProfileName );
            TestProfileConfigurationFactory.CreateProfile( testSyncGenericProfileName );

            try
            {
                TestSyncGenericCachingClass<CachedValueClass> cachingClass = new();
                const int currentId = 0;

                CachedValueClass value1 = cachingClass.GetValue();
                bool called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache should be empty." );
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );

                CachedValueClass value2 = cachingClass.GetValue();
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

        private const string testAsyncProfileName = profileNamePrefix + "TestAsync";

        [CacheConfiguration( ProfileName = testAsyncProfileName )]
        private sealed class TestAsyncCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        private static async Task DoAsyncTest<T>( CachingClass<T> cachingClass, TestingCacheBackend backend ) where T : CachedValueClass, new()
        {
            const int currentId = 0;

            T value = await cachingClass.GetValueAsync();

            bool called = cachingClass.Reset();
            Assert.True( called );

            AssertEx.Equal( currentId, value.Id, "The first given value has unexpected ID." );

            T value2 = await cachingClass.GetValueAsync();
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testAsyncProfileName );

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

        private const string testAsyncGenericProfileName = profileNamePrefix + "TestAsyncGeneric";

        [CacheConfiguration( ProfileName = testAsyncGenericProfileName )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAsyncGenericProfileName );
            TestProfileConfigurationFactory.CreateProfile( testAsyncGenericProfileName );

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

        private const string testDisabledProfileName = profileNamePrefix + "TestDisabled";

        [CacheConfiguration( ProfileName = testDisabledProfileName )]
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testDisabledProfileName );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testDisabledProfileName );
            profile.IsEnabled = false;

            try
            {
                var cachingClass = new TestDisabledCachingClass();

                CachedValueClass value1 = cachingClass.GetValue();
                cachingClass.Reset();
                CachedValueClass value2 = cachingClass.GetValue();
                bool called = cachingClass.Reset();
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

        private const string testDisabledAsyncProfileName = profileNamePrefix + "TestDisabledAsync";

        [CacheConfiguration( ProfileName = testDisabledAsyncProfileName )]
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
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testDisabledAsyncProfileName );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testDisabledAsyncProfileName );
            profile.IsEnabled = false;

            try
            {
                var cachingClass = new TestDisabledAsyncCachingClass();

                CachedValueClass value1 = await cachingClass.GetValueAsync();
                cachingClass.Reset();
                CachedValueClass value2 = await cachingClass.GetValueAsync();
                bool called = cachingClass.Reset();
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
            CachedValueClass value1 = cachingClass.GetValueAsDependency();
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
            CachingServices.Invalidation.Invalidate( value1 );

            var called = itemSetEvent.Wait( timeout );
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: timeout." );

            called = cachingClass.Reset();
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: reset." );

            ++currentId;
            CachedValueClass value2 = cachingClass.GetValueAsDependency();
            called = cachingClass.Reset();
            Assert.False( called, "The method was called when the return value should be cached." );

            AssertEx.Equal( currentId, value2.Id, "The second given value has unexpected ID." );
        }

        private const string testAutoReloadProfileName1 = profileNamePrefix + "TestAutoReload1";

        [CacheConfiguration( ProfileName = testAutoReloadProfileName1 )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadProfileName1 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testAutoReloadProfileName1 );
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

        private const string testAutoReloadProfileName2 = profileNamePrefix + "TestAutoReload2";

        [CacheConfiguration( ProfileName = testAutoReloadProfileName2, AutoReload = true )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadProfileName2 );
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testAutoReloadProfileName2 );
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

        private const string testAutoReloadProfileName3 = profileNamePrefix + "TestAutoReload3";

        [CacheConfiguration( ProfileName = testAutoReloadProfileName3 )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadProfileName3 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testAutoReloadProfileName3 );
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
            CachedValueClass value1 = await cachingClass.GetValueAsDependencyAsync();
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
            await CachingServices.Invalidation.InvalidateAsync( value1 );

            var called = await Task.WhenAny( itemSetEvent.Task, Task.Delay( timeout ) ) == itemSetEvent.Task;
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: timeout." );

            called = cachingClass.Reset();
            Assert.True( called, "The method was not called automatically when the first cache item got invalidated: reset." );

            ++currentId;
            CachedValueClass value2 = await cachingClass.GetValueAsDependencyAsync();
            called = cachingClass.Reset();
            Assert.False( called, "The method was called when the return value should be cached." );

            AssertEx.Equal( currentId, value2.Id, "The second given value has unexpected ID." );
        }

        private const string testAutoReloadAsyncProfileName1 = profileNamePrefix + "TestAutoReloadAsync1";

        [CacheConfiguration( ProfileName = testAutoReloadAsyncProfileName1 )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadAsyncProfileName1 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testAutoReloadAsyncProfileName1 );
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

        private const string testAutoReloadAsyncProfileName2 = profileNamePrefix + "TestAutoReloadAsync2";

        [CacheConfiguration( ProfileName = testAutoReloadAsyncProfileName2, AutoReload = true )]
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadAsyncProfileName2 );
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testAutoReloadAsyncProfileName2 );
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

        private const string testAutoReloadAsyncProfileName3 = profileNamePrefix + "TestAutoReloadAsync3";

        [CacheConfiguration( ProfileName = testAutoReloadAsyncProfileName3 )]
        private sealed class TestAutoReloadAsyncSetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( AutoReload = true )]
            public override async Task<CachedValueClass> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task TestAutoReloadAsyncSetInCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAutoReloadAsyncProfileName3 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testAutoReloadAsyncProfileName3 );
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
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAsyncGenericProfileName );

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

        private const double absoluteExpirationOffsetTestValue1 = 1.1;
        private static readonly TimeSpan absoluteExpirationTestValue1 = TimeSpan.FromMinutes( absoluteExpirationOffsetTestValue1 );

        private const double absoluteExpirationOffsetTestValue2 = 1.2;
        private static readonly TimeSpan absoluteExpirationTestValue2 = TimeSpan.FromMinutes( absoluteExpirationOffsetTestValue2 );

        private const double absoluteExpirationOffsetTestValue3 = 1.3;
        private static readonly TimeSpan absoluteExpirationTestValue3 = TimeSpan.FromMinutes( absoluteExpirationOffsetTestValue3 );

        private const string testAbsoluteExpirationProfileName1 = profileNamePrefix + "TestAbsoluteExpiration1";
        private const string testAbsoluteExpirationProfileName2 = profileNamePrefix + "TestAbsoluteExpiration2";

        [CacheConfiguration( ProfileName = testAbsoluteExpirationProfileName1 )]
        private sealed class TestAbsoluteExpirationSetInProfileAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue1()
            {
                return null;
            }

            [Cache( ProfileName = testAbsoluteExpirationProfileName2 )]
            public object GetValue2()
            {
                return null;
            }

            [Cache( AbsoluteExpiration = absoluteExpirationOffsetTestValue3 )]
            public object GetValue3()
            {
                return null;
            }
        }

        [Fact]
        public void TestAbsoluteExpirationSetInProfileAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAbsoluteExpirationProfileName1 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationProfileName1 );
            profile1.AbsoluteExpiration = absoluteExpirationTestValue1;
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationProfileName2 );
            profile2.AbsoluteExpiration = absoluteExpirationTestValue2;
            var cachingClass = new TestAbsoluteExpirationSetInProfileAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue1();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( absoluteExpirationTestValue1, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                cachingClass.GetValue2();
                Assert.Equal( absoluteExpirationTestValue2, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                cachingClass.GetValue3();
                Assert.Equal( absoluteExpirationTestValue3, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const double absoluteExpirationOffsetTestValue4 = 1.4;
        private static readonly TimeSpan absoluteExpirationTestValue4 = TimeSpan.FromMinutes( absoluteExpirationOffsetTestValue4 );

        private const double absoluteExpirationOffsetTestValue5 = 1.5;
        private static readonly TimeSpan absoluteExpirationTestValue5 = TimeSpan.FromMinutes( absoluteExpirationOffsetTestValue5 );

        private const string testAbsoluteExpirationProfileName4 = profileNamePrefix + "TestAbsoluteExpiration4";

        [CacheConfiguration( ProfileName = testAbsoluteExpirationProfileName4, AbsoluteExpiration = absoluteExpirationOffsetTestValue4 )]
        private sealed class TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue4()
            {
                return null;
            }

            [Cache( AbsoluteExpiration = absoluteExpirationOffsetTestValue5 )]
            public object GetValue5()
            {
                return null;
            }
        }

        [Fact]
        public void TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAbsoluteExpirationProfileName4 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationProfileName4 );
            profile1.AbsoluteExpiration = null;

            var cachingClass =
                new TestAbsoluteExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue4();
                Assert.Equal( absoluteExpirationTestValue4, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                cachingClass.GetValue5();
                Assert.Equal( absoluteExpirationTestValue5, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestAbsoluteExpiration

        #region TestAbsoluteExpirationAsync

        private const string testAbsoluteExpirationAsyncProfileName1 = profileNamePrefix + "TestAbsoluteExpirationAsync1";
        private const string testAbsoluteExpirationAsyncProfileName2 = profileNamePrefix + "TestAbsoluteExpirationAsync2";

        [CacheConfiguration( ProfileName = testAbsoluteExpirationAsyncProfileName1 )]
        private sealed class TestAbsoluteExpirationAsyncSetInProfileAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue1Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( ProfileName = testAbsoluteExpirationAsyncProfileName2 )]
            public async Task<object> GetValue2Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( AbsoluteExpiration = absoluteExpirationOffsetTestValue3 )]
            public async Task<object> GetValue3Async()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestAbsoluteExpirationAsyncSetInProfileAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAbsoluteExpirationAsyncProfileName1 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationAsyncProfileName1 );
            profile1.AbsoluteExpiration = absoluteExpirationTestValue1;
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationAsyncProfileName2 );
            profile2.AbsoluteExpiration = absoluteExpirationTestValue2;

            var cachingClass =
                new TestAbsoluteExpirationAsyncSetInProfileAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue1Async();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( absoluteExpirationTestValue1, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                await cachingClass.GetValue2Async();
                Assert.Equal( absoluteExpirationTestValue2, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                await cachingClass.GetValue3Async();
                Assert.Equal( absoluteExpirationTestValue3, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string testAbsoluteExpirationAsyncProfileName4 = profileNamePrefix + "TestAbsoluteExpirationAsync4";

        [CacheConfiguration( ProfileName = testAbsoluteExpirationAsyncProfileName4, AbsoluteExpiration = absoluteExpirationOffsetTestValue4 )]
        private sealed class TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue4Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( AbsoluteExpiration = absoluteExpirationOffsetTestValue5 )]
            public async Task<object> GetValue5Async()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testAbsoluteExpirationAsyncProfileName4 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testAbsoluteExpirationAsyncProfileName4 );
            profile1.AbsoluteExpiration = null;

            var cachingClass =
                new TestAbsoluteExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue4Async();
                Assert.Equal( absoluteExpirationTestValue4, backend.LastCachedItem.Configuration.AbsoluteExpiration );

                await cachingClass.GetValue5Async();
                Assert.Equal( absoluteExpirationTestValue5, backend.LastCachedItem.Configuration.AbsoluteExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestAbsoluteExpirationAsync

        #region TestSlidingExpiration

        private const double slidingExpirationOffsetTestDoubleValue1 = 1.1;
        private static readonly TimeSpan slidingExpirationOffsetTestTimeSpanValue1 = TimeSpan.FromMinutes( slidingExpirationOffsetTestDoubleValue1 );

        private const double slidingExpirationOffsetTestDoubleValue2 = 1.2;
        private static readonly TimeSpan slidingExpirationOffsetTestTimeSpanValue2 = TimeSpan.FromMinutes( slidingExpirationOffsetTestDoubleValue2 );

        private const double slidingExpirationOffsetTestDoubleValue3 = 1.3;
        private static readonly TimeSpan slidingExpirationOffsetTestTimeSpanValue3 = TimeSpan.FromMinutes( slidingExpirationOffsetTestDoubleValue3 );

        private const string testSlidingExpirationProfileName1 = profileNamePrefix + "TestSlidingExpiration1";
        private const string testSlidingExpirationProfileName2 = profileNamePrefix + "TestSlidingExpiration2";

        [CacheConfiguration( ProfileName = testSlidingExpirationProfileName1 )]
        private sealed class TestSlidingExpirationSetInProfileCachingClass
        {
            [Cache]
            public object GetValue1()
            {
                return null;
            }

            [Cache( ProfileName = testSlidingExpirationProfileName2 )]
            public object GetValue2()
            {
                return null;
            }

            [Cache( SlidingExpiration = slidingExpirationOffsetTestDoubleValue3 )]
            public object GetValue3()
            {
                return null;
            }
        }

        [Fact]
        public void TestSlidingExpiration()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testSlidingExpirationProfileName1 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationProfileName1 );
            profile1.SlidingExpiration = slidingExpirationOffsetTestTimeSpanValue1;
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationProfileName2 );
            profile2.SlidingExpiration = slidingExpirationOffsetTestTimeSpanValue2;
            var cachingClass = new TestSlidingExpirationSetInProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue1();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue1, backend.LastCachedItem.Configuration.SlidingExpiration );

                cachingClass.GetValue2();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue2, backend.LastCachedItem.Configuration.SlidingExpiration );

                cachingClass.GetValue3();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue3, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private const double slidingExpirationOffsetTestDoubleValue4 = 1.4;
        private static readonly TimeSpan slidingExpirationOffsetTestTimeSpanValue4 = TimeSpan.FromMinutes( slidingExpirationOffsetTestDoubleValue4 );

        private const double slidingExpirationOffsetTestDoubleValue5 = 1.5;
        private static readonly TimeSpan slidingExpirationOffsetTestTimeSpanValue5 = TimeSpan.FromMinutes( slidingExpirationOffsetTestDoubleValue5 );

        private const string testSlidingExpirationProfileName4 = profileNamePrefix + "TestSlidingExpiration4";

        [CacheConfiguration( ProfileName = testSlidingExpirationProfileName4, SlidingExpiration = slidingExpirationOffsetTestDoubleValue4 )]
        private sealed class TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public object GetValue4()
            {
                return null;
            }

            [Cache( SlidingExpiration = slidingExpirationOffsetTestDoubleValue5 )]
            public object GetValue5()
            {
                return null;
            }
        }

        [Fact]
        public void TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testSlidingExpirationProfileName4 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationProfileName4 );
            profile1.SlidingExpiration = null;

            var cachingClass =
                new TestSlidingExpirationSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValue4();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue4, backend.LastCachedItem.Configuration.SlidingExpiration );

                cachingClass.GetValue5();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue5, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestSlidingExpiration

        #region TestSlidingExpirationAsync

        private const string testSlidingExpirationAsyncProfileName1 = profileNamePrefix + "TestSlidingExpirationAsync1";
        private const string testSlidingExpirationAsyncProfileName2 = profileNamePrefix + "TestSlidingExpirationAsync2";

        [CacheConfiguration( ProfileName = testSlidingExpirationAsyncProfileName1 )]
        private sealed class TestSlidingExpirationAsyncSetInProfileCachingClass
        {
            [Cache]
            public async Task<object> GetValue1Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( ProfileName = testSlidingExpirationAsyncProfileName2 )]
            public async Task<object> GetValue2Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( SlidingExpiration = slidingExpirationOffsetTestDoubleValue3 )]
            public async Task<object> GetValue3Async()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestSlidingExpirationAsync()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testSlidingExpirationAsyncProfileName1 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationAsyncProfileName1 );
            profile1.SlidingExpiration = slidingExpirationOffsetTestTimeSpanValue1;
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationAsyncProfileName2 );
            profile2.SlidingExpiration = slidingExpirationOffsetTestTimeSpanValue2;
            var cachingClass = new TestSlidingExpirationAsyncSetInProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue1Async();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue1, backend.LastCachedItem.Configuration.SlidingExpiration );

                await cachingClass.GetValue2Async();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue2, backend.LastCachedItem.Configuration.SlidingExpiration );

                await cachingClass.GetValue3Async();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue3, backend.LastCachedItem.Configuration.SlidingExpiration );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string testSlidingExpirationAsyncProfileName4 = profileNamePrefix + "TestSlidingExpirationAsync4";

        [CacheConfiguration( ProfileName = testSlidingExpirationAsyncProfileName4, SlidingExpiration = slidingExpirationOffsetTestDoubleValue4 )]
        private sealed class TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass
        {
            [Cache]
            public async Task<object> GetValue4Async()
            {
                await Task.Yield();

                return null;
            }

            [Cache( SlidingExpiration = slidingExpirationOffsetTestDoubleValue5 )]
            public async Task<object> GetValue5Async()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testSlidingExpirationAsyncProfileName4 );
            CachingProfile profile1 = TestProfileConfigurationFactory.CreateProfile( testSlidingExpirationAsyncProfileName4 );
            profile1.SlidingExpiration = null;

            var cachingClass =
                new TestSlidingExpirationAsyncSetInCacheConfigurationAttributeAndCacheAttributeCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValue4Async();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue4, backend.LastCachedItem.Configuration.SlidingExpiration );

                await cachingClass.GetValue5Async();
                Assert.Equal( slidingExpirationOffsetTestTimeSpanValue5, backend.LastCachedItem.Configuration.SlidingExpiration );
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

            Assert.Equal( CacheItemPriority.NotRemovable, backend.LastCachedItem.Configuration.Priority );
        }

        private const string testCacheItemPriorityProfileName1 = profileNamePrefix + "TestCacheItemPriority1";

        [CacheConfiguration( ProfileName = testCacheItemPriorityProfileName1 )]
        private sealed class TestCacheItemPrioritySetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return null;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInProfile()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityProfileName1 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityProfileName1 );
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

        private const string testCacheItemPriorityProfileName2 = profileNamePrefix + "TestCacheItemPriority2";

        [CacheConfiguration( ProfileName = testCacheItemPriorityProfileName2, Priority = CacheItemPriority.NotRemovable )]
        private sealed class TestCacheItemPrioritySetInProfileUsingCacheAttributeCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValue()
            {
                return null;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInCacheConfigurationAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityProfileName2 );
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityProfileName2 );
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

        private const string testCacheItemPriorityProfileName3 = profileNamePrefix + "TestCacheItemPriority3";

        [CacheConfiguration( ProfileName = testCacheItemPriorityProfileName3 )]
        private sealed class TestCacheItemPrioritySetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( Priority = CacheItemPriority.NotRemovable )]
            public override CachedValueClass GetValue()
            {
                return null;
            }
        }

        [Fact]
        public void TestCacheItemPrioritySetInCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityProfileName3 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityProfileName3 );
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

            Assert.Equal( CacheItemPriority.NotRemovable, backend.LastCachedItem.Configuration.Priority );
        }

        private const string testCacheItemPriorityAsyncProfileName1 = profileNamePrefix + "TestCacheItemPriorityAsync1";

        [CacheConfiguration( ProfileName = testCacheItemPriorityAsyncProfileName1 )]
        private sealed class TestCacheItemPriorityAsyncSetInProfileUsingConfigurationAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInProfile()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityAsyncProfileName1 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityAsyncProfileName1 );
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

        private const string testCacheItemPriorityAsyncProfileName2 = profileNamePrefix + "TestCacheItemPriorityAsync2";

        [CacheConfiguration( ProfileName = testCacheItemPriorityAsyncProfileName2, Priority = CacheItemPriority.NotRemovable )]
        private sealed class TestCacheItemPriorityAsyncSetInProfileUsingCacheAttributeCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInCacheConfigurationAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityAsyncProfileName2 );
            CachingProfile profile2 = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityAsyncProfileName2 );
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

        private const string testCacheItemPriorityAsyncProfileName3 = profileNamePrefix + "TestCacheItemPriorityAsync3";

        [CacheConfiguration( ProfileName = testCacheItemPriorityAsyncProfileName3 )]
        private sealed class TestCacheItemPriorityAsyncSetInCacheAttributeCachingClass : CachingClass
        {
            [Cache( Priority = CacheItemPriority.NotRemovable )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestCacheItemPriorityAsyncSetInCacheAttribute()
        {
            TestingCacheBackend backend = TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCacheItemPriorityAsyncProfileName3 );
            CachingProfile profile = TestProfileConfigurationFactory.CreateProfile( testCacheItemPriorityAsyncProfileName3 );
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
            public string Name { get; private set; }

            public NamedCachingClass( string name )
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
            bool called = cachingClassNotIgnoringThisParameter1.Reset();
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

        private const string testIgnoreThisParameterProfileName1 = profileNamePrefix + "TestIgnoreThisParameter1";

        [CacheConfiguration( ProfileName = testIgnoreThisParameterProfileName1 )]
        private sealed class CachingClassNotIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public CachingClassNotIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [CacheConfiguration( ProfileName = testIgnoreThisParameterProfileName1 )]
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
            TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testIgnoreThisParameterProfileName1 );
            TestProfileConfigurationFactory.CreateProfile( testIgnoreThisParameterProfileName1 );

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

        private const string testIgnoreThisParameterProfileName2 = profileNamePrefix + "TestIgnoreThisParameter2";

        [CacheConfiguration( ProfileName = testIgnoreThisParameterProfileName2 )]
        private sealed class CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public CachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [CacheConfiguration( ProfileName = testIgnoreThisParameterProfileName2, IgnoreThisParameter = true )]
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
            TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testIgnoreThisParameterProfileName2 );
            TestProfileConfigurationFactory.CreateProfile( testIgnoreThisParameterProfileName2 );

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
            bool called = cachingClassNotIgnoringThisParameter1.Reset();
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

        private const string testIgnoreThisParameterAsyncProfileName1 = profileNamePrefix + "TestIgnoreThisParameterAsync1";

        [CacheConfiguration( ProfileName = testIgnoreThisParameterAsyncProfileName1 )]
        private sealed class AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute : NamedCachingClass
        {
            public AsyncCachingClassNotIgnoringThisParameterSetInCacheAttribute( string name ) : base( name ) { }

            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [CacheConfiguration( ProfileName = testIgnoreThisParameterAsyncProfileName1 )]
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
            TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testIgnoreThisParameterAsyncProfileName1 );
            TestProfileConfigurationFactory.CreateProfile( testIgnoreThisParameterAsyncProfileName1 );

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

        private const string TestIgnoreThisParameterAsyncProfileName2 = profileNamePrefix + "TestIgnoreThisParameterAsync2";

        [CacheConfiguration( ProfileName = TestIgnoreThisParameterAsyncProfileName2 )]
        private sealed class AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute : NamedCachingClass
        {
            public AsyncCachingClassNotIgnoringThisParameterSetInCacheConfigurationAttribute( string name ) : base( name ) { }

            [Cache]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [CacheConfiguration( ProfileName = TestIgnoreThisParameterAsyncProfileName2, IgnoreThisParameter = true )]
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
            TestProfileConfigurationFactory.InitializeTestWithTestingBackend( TestIgnoreThisParameterAsyncProfileName2 );
            TestProfileConfigurationFactory.CreateProfile( TestIgnoreThisParameterAsyncProfileName2 );

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

        private const string testMethodLevelProfileNameSetInConfigurationAttribute = profileNamePrefix + "MethodLevelProfileNameSetInConfigurationAttribute";
        private const string testMethodLevelProfileNameSetInCacheAttribute = profileNamePrefix + "MethodLevelProfileNameSetInCacheAttribute";

        [CacheConfiguration( ProfileName = testMethodLevelProfileNameSetInConfigurationAttribute )]
        private sealed class TestMethodLevelProfileCachingClass
        {
            [Cache]
            public object GetValueUsingSetInConfigurationAttribute()
            {
                return null;
            }

            [Cache( ProfileName = testMethodLevelProfileNameSetInCacheAttribute )]
            public object GetValueUsingSetInCacheAttribute()
            {
                return null;
            }
        }

        [Fact]
        public void TestMethodLevelProfile()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testMethodLevelProfileNameSetInConfigurationAttribute );

            TestProfileConfigurationFactory.CreateProfile( testMethodLevelProfileNameSetInConfigurationAttribute );
            TestProfileConfigurationFactory.CreateProfile( testMethodLevelProfileNameSetInCacheAttribute );
            var cachingClass = new TestMethodLevelProfileCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                cachingClass.GetValueUsingSetInConfigurationAttribute();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( testMethodLevelProfileNameSetInConfigurationAttribute, backend.LastCachedItem.Configuration.ProfileName );

                string firstCachedKey = backend.LastCachedKey;

                cachingClass.GetValueUsingSetInCacheAttribute();

                Assert.NotEqual( firstCachedKey, backend.LastCachedKey );

                Assert.Equal( testMethodLevelProfileNameSetInCacheAttribute, backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion

        #region TestMethodLevelProfileAsync

        private const string testMethodLevelProfileAsyncNameSetInConfigurationAttribute =
            profileNamePrefix + "MethodLevelProfileNameSetInConfigurationAttribute";

        private const string testMethodLevelProfileAsyncNameSetInCacheAttribute = profileNamePrefix + "MethodLevelProfileNameSetInCacheAttribute";

        [CacheConfiguration( ProfileName = testMethodLevelProfileAsyncNameSetInConfigurationAttribute )]
        private sealed class TestMethodLevelProfileAsyncCachingClass
        {
            [Cache]
            public async Task<object> GetValueUsingSetInConfigurationAttributeAsync()
            {
                await Task.Yield();

                return null;
            }

            [Cache( ProfileName = testMethodLevelProfileAsyncNameSetInCacheAttribute )]
            public async Task<object> GetValueUsingSetInCacheAttributeAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        [Fact]
        public async Task TestMethodLevelProfileAsync()
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testMethodLevelProfileAsyncNameSetInConfigurationAttribute );

            TestProfileConfigurationFactory.CreateProfile( testMethodLevelProfileAsyncNameSetInConfigurationAttribute );
            TestProfileConfigurationFactory.CreateProfile( testMethodLevelProfileAsyncNameSetInCacheAttribute );
            var cachingClass = new TestMethodLevelProfileAsyncCachingClass();

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await cachingClass.GetValueUsingSetInConfigurationAttributeAsync();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal( testMethodLevelProfileAsyncNameSetInConfigurationAttribute, backend.LastCachedItem.Configuration.ProfileName );

                string firstCachedKey = backend.LastCachedKey;

                await cachingClass.GetValueUsingSetInCacheAttributeAsync();

                Assert.NotEqual( firstCachedKey, backend.LastCachedKey );

                Assert.Equal( testMethodLevelProfileAsyncNameSetInCacheAttribute, backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion
    }
}