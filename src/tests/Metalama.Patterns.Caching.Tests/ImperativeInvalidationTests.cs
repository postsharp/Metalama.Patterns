using System;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed partial class ImperativeInvalidationTests : InvalidationTestsBase
    {
        private const string profileNamePrefix = "Caching.Tests.ImperativeInvalidationTests_";

        private static void DoInvalidateCacheAttributeTest(
            string profileName,
            Func<CachedValueClass>[] cachedMethods,
            Action[] invalidatingMethods,
            string testDescription,
            bool firstPairShouldWork,
            bool otherPairsShouldWork
        )
        {
            Func<CachedValueClass>[] invalidatingMethodProxies = new Func<CachedValueClass>[invalidatingMethods.Length];

            for ( int i = 0; i < invalidatingMethods.Length; i++ )
            {
                int localIndex = i;
                invalidatingMethodProxies[i] = () =>
                                               {
                                                   CachedValueClass cachedValue = cachedMethods[localIndex].Invoke();
                                                   invalidatingMethods[localIndex].Invoke();
                                                   return cachedValue;
                                               };
            }

            DoInvalidateCacheAttributeTest( profileName, cachedMethods, invalidatingMethodProxies, testDescription,
                                            firstPairShouldWork, otherPairsShouldWork, true );
        }

        sealed class NullCachedValueClass : CachedValueClass
        {
            public static readonly NullCachedValueClass Instance = new NullCachedValueClass();

            private NullCachedValueClass()
            {
            }
        }

        private static void DoTestSimpleImperativeInvalidation( string profileName,
                                                                Func<CachedValueClass> cachedMethod,
                                                                Action invalidatingMethod,
                                                                Func<bool> resetMethod )
        {
            Func<CachedValueClass> invalidatingMethodProxy = () =>
                                                             {
                                                                 invalidatingMethod();
                                                                 return NullCachedValueClass.Instance;
                                                             };

            DoTestSimpleImperativeInvalidation( profileName, cachedMethod, invalidatingMethodProxy, resetMethod );
        }

        private static void DoTestSimpleImperativeInvalidation( string profileName,
                                                                Func<CachedValueClass> cachedMethod,
                                                                Func<CachedValueClass> invalidatingOrRecachingMethod,
                                                                Func<bool> resetMethod )
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                CachedValueClass initialValue = cachedMethod();
                Assert.True( resetMethod(), "The cached method has not been called for the first time before invalidation." );

                CachedValueClass cachedValueBeforeInvalidation = cachedMethod();
                Assert.False( resetMethod(), "The cached method has been called for the second time before invalidation." );

                AssertEx.Equal( initialValue, cachedValueBeforeInvalidation, "The initial value and the cached value before invalidation are not the same." );

                CachedValueClass recachedValue = invalidatingOrRecachingMethod();

                if ( recachedValue == NullCachedValueClass.Instance )
                {
                    // Just invalidating (not recaching)

                    CachedValueClass valueAfterInvalidation = cachedMethod();
                    Assert.True( resetMethod(), "The cached method has not been called for the first time after invalidation." );

                    CachedValueClass cachedValueAfterInvalidation = cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the second time after invalidation." );

                    AssertEx.Equal( valueAfterInvalidation, cachedValueAfterInvalidation,
                                     "The initial value and the cached value after invalidation are not the same." );
                }
                else
                {
                    // Recaching (not just invalidating)

                    Assert.True( resetMethod(), "The cached method has not been called during recaching." );

                    CachedValueClass valueAfterRecaching = cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the first time after recaching." );

                    AssertEx.Equal( recachedValue, valueAfterRecaching, "The recached value and the cached value after recaching are not the same." );
                }
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private static async Task DoTestSimpleImperativeInvalidationAsync( string profileName,
                                                                           Func<Task<CachedValueClass>> cachedMethod,
                                                                           Func<Task> invalidatingMethod,
                                                                           Func<bool> resetMethod )
        {
            Func<Task<CachedValueClass>> invalidatingMethodProxy = async () =>
                                                                   {
                                                                       await invalidatingMethod();
                                                                       return NullCachedValueClass.Instance;
                                                                   };

            await DoTestSimpleImperativeInvalidationAsync( profileName, cachedMethod, invalidatingMethodProxy, resetMethod );
        }

        private static async Task DoTestSimpleImperativeInvalidationAsync( string profileName,
                                                                           Func<Task<CachedValueClass>> cachedMethod,
                                                                           Func<Task<CachedValueClass>> invalidatingOrRecachingMethod,
                                                                           Func<bool> resetMethod )
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                CachedValueClass initialValue = await cachedMethod();
                Assert.True( resetMethod(), "The cached method has not been called for the first time before invalidation." );

                CachedValueClass cachedValueBeforeInvalidation = await cachedMethod();
                Assert.False( resetMethod(), "The cached method has been called for the second time before invalidation." );

                AssertEx.Equal( initialValue, cachedValueBeforeInvalidation, "The initial value and the cached value before invalidation are not the same." );

                CachedValueClass recachedValue = await invalidatingOrRecachingMethod();

                if ( recachedValue == NullCachedValueClass.Instance )
                {
                    // Just invalidating (not recaching)

                    CachedValueClass valueAfterInvalidation = await cachedMethod();
                    Assert.True( resetMethod(), "The cached method has not been called for the first time after invalidation." );

                    CachedValueClass cachedValueAfterInvalidation = await cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the second time after invalidation." );

                    AssertEx.Equal( valueAfterInvalidation, cachedValueAfterInvalidation,
                                     "The initial value and the cached value after invalidation are not the same." );
                }
                else
                {
                    // Recaching (not just invalidating)

                    Assert.True( resetMethod(), "The cached method has not been called during recaching." );

                    CachedValueClass valueAfterRecaching = await cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the first time after recaching." );

                    AssertEx.Equal( recachedValue, valueAfterRecaching, "The recached value and the cached value after recaching are not the same." );
                }
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #region TestImperativeInvalidation

        private const string testImperativeInvalidationProfileName = profileNamePrefix + nameof(TestImperativeInvalidation);

        [CacheConfiguration( ProfileName = testImperativeInvalidationProfileName )]
        private sealed class TestImperativeInvalidationCachingClass
        {
            private static int counter = 0;

            public readonly CallsCounters CallsCounters = new CallsCounters( 4 );

            [Cache]
            public CachedValueClass GetValue()
            {
                this.CallsCounters.Increment( 0 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                this.CallsCounters.Increment( 1 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );
                return new CachedValueClass( counter++ );
            }
        }

        [Fact]
        public void TestImperativeInvalidation()
        {

            TestImperativeInvalidationCachingClass cachingClass =
                new TestImperativeInvalidationCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    cachingClass.GetValue,
                    () => cachingClass.GetValue( 1 ),
                    () => cachingClass.GetValue( 1, cachedValue2 ),
                    () => cachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            Action[] invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 1 ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 1, cachedValue2 ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 1, cachedValue2, 3, 5 )
                };

            string testName = "Matching values test";

            DoInvalidateCacheAttributeTest( testImperativeInvalidationProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            true );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 2, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 0 ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 0, cachedValue0 ),
                    () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue, 0, cachedValue0, 0, 5 )
                };

            testName = "Not matching values test";

            DoInvalidateCacheAttributeTest( testImperativeInvalidationProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            false );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( i == 0 ? 4 : 3, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestImperativeInvalidation

        #region TestImperativeInvalidationAsync

        private const string testImperativeInvalidationAsyncProfileName = profileNamePrefix + nameof(TestImperativeInvalidationAsync);

        [CacheConfiguration( ProfileName = testImperativeInvalidationAsyncProfileName )]
        private sealed class TestImperativeInvalidationAsyncCachingClass
        {
            private static int counter = 0;

            public readonly CallsCounters CallsCounters = new CallsCounters( 4 );

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.CallsCounters.Increment( 0 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                this.CallsCounters.Increment( 1 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }
        }

        [Fact]
        public void TestImperativeInvalidationAsync()
        {

            TestImperativeInvalidationAsyncCachingClass cachingClass =
                new TestImperativeInvalidationAsyncCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => cachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Action[] invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1 ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, cachedValue2 ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, cachedValue2, 3, 5 ).Wait()
                };

            string testName = "Matching values test";

            DoInvalidateCacheAttributeTest( testImperativeInvalidationAsyncProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            true );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 2, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0 ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0, cachedValue0 ).Wait(),
                    () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0, cachedValue0, 0, 5 ).Wait()
                };

            testName = "Not matching values test";

            DoInvalidateCacheAttributeTest( testImperativeInvalidationAsyncProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            false );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( i == 0 ? 4 : 3, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestImperativeInvalidationAsync

        #region TestRecaching

        private const string testRecachingProfileName = profileNamePrefix + nameof(TestRecaching);

        [CacheConfiguration( ProfileName = testRecachingProfileName )]
        private sealed class TestRecachingCachingClass
        {
            private static int counter = 0;

            public readonly CallsCounters CallsCounters = new CallsCounters( 4 );

            [Cache]
            public CachedValueClass GetValue()
            {
                this.CallsCounters.Increment( 0 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                this.CallsCounters.Increment( 1 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );
                return new CachedValueClass( counter++ );
            }
        }

        [Fact]
        public void TestRecaching()
        {

            TestRecachingCachingClass cachingClass =
                new TestRecachingCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    cachingClass.GetValue,
                    () => cachingClass.GetValue( 1 ),
                    () => cachingClass.GetValue( 1, cachedValue2 ),
                    () => cachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            Action[] invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 1 ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 1, cachedValue2 ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 1, cachedValue2, 3, 5 )
                };

            string testName = "Matching values test";

            DoInvalidateCacheAttributeTest( testRecachingProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            true );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 2, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 0 ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 0, cachedValue0 ),
                    () => CachingServices.Invalidation.Recache( cachingClass.GetValue, 0, cachedValue0, 0, 5 )
                };

            testName = "Not matching values test";

            DoInvalidateCacheAttributeTest( testRecachingProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            "Not matching values test",
                                            true,
                                            false );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 4, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestRecaching

        #region TestRecachingAsync

        private const string testRecachingAsyncProfileName = profileNamePrefix + nameof(TestRecachingAsync);

        [CacheConfiguration( ProfileName = testRecachingAsyncProfileName )]
        private sealed class TestRecachingAsyncCachingClass
        {
            private static int counter = 0;

            public readonly CallsCounters CallsCounters = new CallsCounters( 4 );

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.CallsCounters.Increment( 0 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                this.CallsCounters.Increment( 1 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );
                return await Task.Run( () => new CachedValueClass( counter++ ) );
            }
        }

        [Fact]
        public void TestRecachingAsync()
        {

            TestRecachingAsyncCachingClass cachingClass =
                new TestRecachingAsyncCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => cachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Action[] invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1 ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, cachedValue2 ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, cachedValue2, 3, 5 ).Wait()
                };

            string testName = "Matching values test";

            DoInvalidateCacheAttributeTest( testRecachingAsyncProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            testName,
                                            true,
                                            true );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 2, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0 ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0, cachedValue0 ).Wait(),
                    () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0, cachedValue0, 0, 5 ).Wait()
                };

            testName = "Not matching values test";

            DoInvalidateCacheAttributeTest( testRecachingAsyncProfileName,
                                            cachedMethods,
                                            invalidatingMethods,
                                            "Not matching values test",
                                            true,
                                            false );

            for ( int i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal( 4, cachingClass.CallsCounters[i],
                                 $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestRecachingAsync

        #region TestImperativeInvalidationWithNestedContexts

        private const string testImperativeInvalidationWithNestedContextsProfileName =
            profileNamePrefix + nameof(TestImperativeInvalidationWithNestedContexts);

        [CacheConfiguration( ProfileName = testImperativeInvalidationWithNestedContextsProfileName )]
        public sealed class TestImperativeInvalidationWithNestedContextsClass
        {
            private int invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this.invocations++;
            }
        }

        [Fact]
        public void TestImperativeInvalidationWithNestedContexts()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testImperativeInvalidationWithNestedContextsProfileName );
            TestProfileConfigurationFactory.CreateProfile( testImperativeInvalidationWithNestedContextsProfileName );

            try
            {
                TestImperativeInvalidationWithNestedContextsClass c = new TestImperativeInvalidationWithNestedContextsClass();
                int call1 = c.OuterMethod();
                CachingServices.Invalidation.Invalidate( c.InnerMethod );
                int call2 = c.OuterMethod();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestImperativeInvalidationWithNestedContexts

        #region TestImperativeInvalidationWithNestedContextsAsync

        private const string testImperativeInvalidationWithNestedContextsAsyncProfileName =
            profileNamePrefix + nameof(TestImperativeInvalidationWithNestedContextsAsync);

        [CacheConfiguration( ProfileName = testImperativeInvalidationWithNestedContextsAsyncProfileName )]
        public sealed class TestImperativeInvalidationWithNestedContextsAsyncClass
        {
            private int invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this.invocations++ );
            }
        }

        [Fact]
        public async Task TestImperativeInvalidationWithNestedContextsAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testImperativeInvalidationWithNestedContextsAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testImperativeInvalidationWithNestedContextsAsyncProfileName );

            try
            {
                TestImperativeInvalidationWithNestedContextsAsyncClass c = new TestImperativeInvalidationWithNestedContextsAsyncClass();
                int call1 = await c.OuterMethodAsync();
                await CachingServices.Invalidation.InvalidateAsync( c.InnerMethodAsync );
                int call2 = await c.OuterMethodAsync();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestImperativeInvalidationWithNestedContextsAsync

        #region TestRecachingOfInnerMethod

        private const string testRecachingOfInnerMethodProfileName =
            profileNamePrefix + nameof(TestRecachingOfInnerMethod);

        [CacheConfiguration( ProfileName = testRecachingOfInnerMethodProfileName )]
        public sealed class TestRecachingOfInnerMethodClass
        {
            private int invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this.invocations++;
            }
        }

        [Fact]
        public void TestRecachingOfInnerMethod()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testRecachingOfInnerMethodProfileName );
            TestProfileConfigurationFactory.CreateProfile( testRecachingOfInnerMethodProfileName );

            try
            {
                TestRecachingOfInnerMethodClass c = new TestRecachingOfInnerMethodClass();
                int call1 = c.OuterMethod();
                int call2 = CachingServices.Invalidation.Recache( c.InnerMethod );
                int call3 = c.OuterMethod();

                Assert.NotEqual( call1, call2 );
                Assert.NotEqual( call2, call3 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestRecachingOfInnerMethod

        #region TestRecachingOfInnerMethodAsync

        private const string testRecachingOfInnerMethodAsyncProfileName =
            profileNamePrefix + nameof(TestRecachingOfInnerMethodAsync);

        [CacheConfiguration( ProfileName = testRecachingOfInnerMethodAsyncProfileName )]
        public sealed class TestRecachingOfInnerMethodAsyncClass
        {
            private int invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this.invocations++ );
            }
        }

        [Fact]
        public async Task TestRecachingOfInnerMethodAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testRecachingOfInnerMethodAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testRecachingOfInnerMethodAsyncProfileName );

            try
            {
                TestRecachingOfInnerMethodAsyncClass c = new TestRecachingOfInnerMethodAsyncClass();
                int call1 = await c.OuterMethodAsync();
                int call2 = await CachingServices.Invalidation.RecacheAsync( c.InnerMethodAsync );
                int call3 = await c.OuterMethodAsync();

                Assert.NotEqual( call1, call2 );
                Assert.NotEqual( call2, call3 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestRecachingOfInnerMethodAsync

        #region TestRecachingOfOuterMethod

        private const string testRecachingOfOuterMethodProfileName =
            profileNamePrefix + nameof(TestRecachingOfOuterMethod);

        [CacheConfiguration( ProfileName = testRecachingOfOuterMethodProfileName )]
        public sealed class TestRecachingOfOuterMethodClass
        {
            private int invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this.invocations++;
            }
        }

        [Fact]
        public void TestRecachingOfOuterMethod()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testRecachingOfOuterMethodProfileName );
            TestProfileConfigurationFactory.CreateProfile( testRecachingOfOuterMethodProfileName );

            try
            {
                TestRecachingOfOuterMethodClass c = new TestRecachingOfOuterMethodClass();
                int call1 = c.OuterMethod();
                int call2 = CachingServices.Invalidation.Recache( c.OuterMethod );
                int call3 = c.OuterMethod();

                Assert.Equal( call1, call2 );
                Assert.Equal( call2, call3 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestRecachingOfOuterMethod

        #region TestRecachingOfOuterMethodAsync

        private const string testRecachingOfOuterMethodAsyncProfileName =
            profileNamePrefix + nameof(TestRecachingOfOuterMethodAsync);

        [CacheConfiguration( ProfileName = testRecachingOfOuterMethodAsyncProfileName )]
        public sealed class TestRecachingOfOuterMethodAsyncClass
        {
            private int invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this.invocations++ );
            }
        }

        [Fact]
        public async Task TestRecachingOfOuterMethodAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testRecachingOfOuterMethodAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testRecachingOfOuterMethodAsyncProfileName );

            try
            {
                TestRecachingOfOuterMethodAsyncClass c = new TestRecachingOfOuterMethodAsyncClass();
                int call1 = await c.OuterMethodAsync();
                int call2 = await CachingServices.Invalidation.RecacheAsync( c.OuterMethodAsync );
                int call3 = await c.OuterMethodAsync();

                Assert.Equal( call1, call2 );
                Assert.Equal( call2, call3 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestRecachingOfOuterMethodAsync

        #region TestSimpleImperativeInvalidationWith0Parameters

        private const string testSimpleImperativeInvalidationWith0ParametersProfileName =
            profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith0Parameters);

        class TestSimpleImperativeInvalidationWith0ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith0ParametersProfileName )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith0Parameters()
        {
            TestSimpleImperativeInvalidationWith0ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith0ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith0ParametersProfileName,
                                                cachingClass.GetValue,
                                                () => CachingServices.Invalidation.Invalidate( cachingClass.GetValue ),
                                                cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeInvalidationWith0ParametersAsync

        private const string testSimpleImperativeInvalidationWith0ParametersAsyncProfileName =
            profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith0ParametersAsync);

        class TestSimpleImperativeInvalidationWith0ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith0ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith0ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith0ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith0ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith0ParametersAsyncProfileName,
                                                           cachingClass.GetValueAsync,
                                                           () => CachingServices.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ),
                                                           cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeRecachingWith0Parameters

        private const string testSimpleImperativeRecachingWith0ParametersProfileName =
            profileNamePrefix + nameof(TestSimpleImperativeRecachingWith0Parameters);

        class TestSimpleImperativeRecachingWith0ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith0ParametersProfileName )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith0Parameters()
        {
            TestSimpleImperativeRecachingWith0ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith0ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith0ParametersProfileName,
                                                cachingClass.GetValue,
                                                () => CachingServices.Invalidation.Recache( cachingClass.GetValue ),
                                                cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeRecachingWith0ParametersAsync

        private const string testSimpleImperativeRecachingWith0ParametersAsyncProfileName =
            profileNamePrefix + nameof(TestSimpleImperativeRecachingWith0ParametersAsync);

        class TestSimpleImperativeRecachingWith0ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith0ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith0ParametersAsync()
        {
            TestSimpleImperativeRecachingWith0ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith0ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith0ParametersAsyncProfileName,
                                                           cachingClass.GetValueAsync,
                                                           () => CachingServices.Invalidation.RecacheAsync( cachingClass.GetValueAsync ),
                                                           cachingClass.Reset );
        }

        #endregion
    }
}
