// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global

namespace Metalama.Patterns.Caching.Tests
{
    public sealed partial class ImperativeInvalidationTests : InvalidationTestsBase
    {
        private const string _profileNamePrefix = "Caching.Tests.ImperativeInvalidationTests_";

        private void DoInvalidateCacheAttributeTest(
            string profileName,
            Func<CachedValueClass>[] cachedMethods,
            Action[] invalidatingMethods,
            string testDescription,
            bool firstPairShouldWork,
            bool otherPairsShouldWork )
        {
            var invalidatingMethodProxies = new Func<CachedValueClass>[invalidatingMethods.Length];

            for ( var i = 0; i < invalidatingMethods.Length; i++ )
            {
                var localIndex = i;

                invalidatingMethodProxies[i] = () =>
                {
                    var cachedValue = cachedMethods[localIndex].Invoke();
                    invalidatingMethods[localIndex].Invoke();

                    return cachedValue;
                };
            }

            this.DoInvalidateCacheAttributeTest(
                profileName,
                cachedMethods,
                invalidatingMethodProxies,
                testDescription,
                firstPairShouldWork,
                otherPairsShouldWork,
                true );
        }

        private sealed class NullCachedValueClass : CachedValueClass
        {
            public static readonly NullCachedValueClass Instance = new();

            private NullCachedValueClass() { }
        }

        private void DoTestSimpleImperativeInvalidation(
            string profileName,
            Func<CachedValueClass> cachedMethod,
            Action invalidatingMethod,
            Func<bool> resetMethod )
        {
            CachedValueClass InvalidatingMethodProxy()
            {
                invalidatingMethod();

                return NullCachedValueClass.Instance;
            }

            this.DoTestSimpleImperativeInvalidation( profileName, cachedMethod, InvalidatingMethodProxy, resetMethod );
        }

        private void DoTestSimpleImperativeInvalidation(
            string profileName,
            Func<CachedValueClass> cachedMethod,
            Func<CachedValueClass> invalidatingOrRecachingMethod,
            Func<bool> resetMethod )
        {
            this.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                var initialValue = cachedMethod();
                Assert.True( resetMethod(), "The cached method has not been called for the first time before invalidation." );

                var cachedValueBeforeInvalidation = cachedMethod();
                Assert.False( resetMethod(), "The cached method has been called for the second time before invalidation." );

                AssertEx.Equal( initialValue, cachedValueBeforeInvalidation, "The initial value and the cached value before invalidation are not the same." );

                var recachedValue = invalidatingOrRecachingMethod();

                // ReSharper disable once PossibleUnintendedReferenceComparison
                if ( recachedValue == NullCachedValueClass.Instance )
                {
                    // Just invalidating (not recaching)

                    var valueAfterInvalidation = cachedMethod();
                    Assert.True( resetMethod(), "The cached method has not been called for the first time after invalidation." );

                    var cachedValueAfterInvalidation = cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the second time after invalidation." );

                    AssertEx.Equal(
                        valueAfterInvalidation,
                        cachedValueAfterInvalidation,
                        "The initial value and the cached value after invalidation are not the same." );
                }
                else
                {
                    // Recaching (not just invalidating)

                    Assert.True( resetMethod(), "The cached method has not been called during recaching." );

                    var valueAfterRecaching = cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the first time after recaching." );

                    AssertEx.Equal( recachedValue, valueAfterRecaching, "The recached value and the cached value after recaching are not the same." );
                }
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private async Task DoTestSimpleImperativeInvalidationAsync(
            string profileName,
            Func<Task<CachedValueClass>> cachedMethod,
            Func<ValueTask> invalidatingMethod,
            Func<bool> resetMethod )
        {
            async Task<CachedValueClass> InvalidatingMethodProxy()
            {
                await invalidatingMethod();

                return NullCachedValueClass.Instance;
            }

            await this.DoTestSimpleImperativeInvalidationAsync( profileName, cachedMethod, InvalidatingMethodProxy, resetMethod );
        }

        private async Task DoTestSimpleImperativeInvalidationAsync(
            string profileName,
            Func<Task<CachedValueClass>> cachedMethod,
            Func<Task<CachedValueClass>> invalidatingOrRecachingMethod,
            Func<bool> resetMethod )
        {
            this.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                var initialValue = await cachedMethod();
                Assert.True( resetMethod(), "The cached method has not been called for the first time before invalidation." );

                var cachedValueBeforeInvalidation = await cachedMethod();
                Assert.False( resetMethod(), "The cached method has been called for the second time before invalidation." );

                AssertEx.Equal( initialValue, cachedValueBeforeInvalidation, "The initial value and the cached value before invalidation are not the same." );

                var recachedValue = await invalidatingOrRecachingMethod();

                // ReSharper disable once PossibleUnintendedReferenceComparison
                if ( recachedValue == NullCachedValueClass.Instance )
                {
                    // Just invalidating (not recaching)

                    var valueAfterInvalidation = await cachedMethod();
                    Assert.True( resetMethod(), "The cached method has not been called for the first time after invalidation." );

                    var cachedValueAfterInvalidation = await cachedMethod();
                    Assert.False( resetMethod(), "The cached method has been called for the second time after invalidation." );

                    AssertEx.Equal(
                        valueAfterInvalidation,
                        cachedValueAfterInvalidation,
                        "The initial value and the cached value after invalidation are not the same." );
                }
                else
                {
                    // Recaching (not just invalidating)

                    Assert.True( resetMethod(), "The cached method has not been called during recaching." );

                    var valueAfterRecaching = await cachedMethod();
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

        private const string _testImperativeInvalidationProfileName = _profileNamePrefix + nameof(TestImperativeInvalidation);

        [CachingConfiguration( ProfileName = _testImperativeInvalidationProfileName )]
        private sealed class TestImperativeInvalidationCachingClass
        {
            private static int _counter;

            public CallsCounters CallsCounters { get; } = new( 4 );

            [Cache]
            public CachedValueClass GetValue()
            {
                this.CallsCounters.Increment( 0 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                this.CallsCounters.Increment( 1 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );

                return new CachedValueClass( _counter++ );
            }
        }

        [Fact]
        public void TestImperativeInvalidation()
        {
            var cachingClass =
                new TestImperativeInvalidationCachingClass();

            var cachedValue0 = new CachedValueChildClass( 0 );
            var cachedValue2 = new CachedValueChildClass( 2 );

            var cachedMethods =
                new[]
                {
                    cachingClass.GetValue,
                    () => cachingClass.GetValue( 1 ),
                    () => cachingClass.GetValue( 1, cachedValue2 ),
                    () => cachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            var invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1 ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, cachedValue2 ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, cachedValue2, 3, 5 )
                };

            var testName = "Matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testImperativeInvalidationProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                true );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    2,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 0 ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 0, cachedValue0 ),
                    () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 0, cachedValue0, 0, 5 )
                };

            testName = "Not matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testImperativeInvalidationProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                false );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    i == 0 ? 4 : 3,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestImperativeInvalidation

        #region TestImperativeInvalidationAsync

        private const string _testImperativeInvalidationAsyncProfileName = _profileNamePrefix + nameof(TestImperativeInvalidationAsync);

        [CachingConfiguration( ProfileName = _testImperativeInvalidationAsyncProfileName )]
        private sealed class TestImperativeInvalidationAsyncCachingClass
        {
            private static int _counter;

            public CallsCounters CallsCounters { get; } = new( 4 );

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.CallsCounters.Increment( 0 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                this.CallsCounters.Increment( 1 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }
        }

        [Fact]
        public void TestImperativeInvalidationAsync()
        {
            var cachingClass =
                new TestImperativeInvalidationAsyncCachingClass();

            var cachedValue0 = new CachedValueChildClass( 0 );
            var cachedValue2 = new CachedValueChildClass( 2 );

            var cachedMethods =
                new[]
                {
                    () => cachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            var invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1 ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, cachedValue2 ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, cachedValue2, 3, 5 ).Wait()
                };

            var testName = "Matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testImperativeInvalidationAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                true );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    2,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0 ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0, cachedValue0 ).Wait(),
                    () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 0, cachedValue0, 0, 5 ).Wait()
                };

            testName = "Not matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testImperativeInvalidationAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                false );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    i == 0 ? 4 : 3,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestImperativeInvalidationAsync

        #region TestRecaching

        private const string _testRecachingProfileName = _profileNamePrefix + nameof(TestRecaching);

        [CachingConfiguration( ProfileName = _testRecachingProfileName )]
        private sealed class TestRecachingCachingClass
        {
            private static int _counter;

            public CallsCounters CallsCounters { get; } = new( 4 );

            [Cache]
            public CachedValueClass GetValue()
            {
                this.CallsCounters.Increment( 0 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                this.CallsCounters.Increment( 1 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );

                return new CachedValueClass( _counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );

                return new CachedValueClass( _counter++ );
            }
        }

        [Fact]
        public void TestRecaching()
        {
            var cachingClass =
                new TestRecachingCachingClass();

            var cachedValue0 = new CachedValueChildClass( 0 );
            var cachedValue2 = new CachedValueChildClass( 2 );

            var cachedMethods =
                new[]
                {
                    cachingClass.GetValue,
                    () => cachingClass.GetValue( 1 ),
                    () => cachingClass.GetValue( 1, cachedValue2 ),
                    () => cachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            var invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1 ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, cachedValue2 ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, cachedValue2, 3, 5 )
                };

            var testName = "Matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testRecachingProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                true );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    2,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new Action[]
                {
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 0 ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 0, cachedValue0 ),
                    () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 0, cachedValue0, 0, 5 )
                };

            testName = "Not matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testRecachingProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    4,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestRecaching

        #region TestRecachingAsync

        private const string _testRecachingAsyncProfileName = _profileNamePrefix + nameof(TestRecachingAsync);

        [CachingConfiguration( ProfileName = _testRecachingAsyncProfileName )]
        private sealed class TestRecachingAsyncCachingClass
        {
            private static int _counter;

            public CallsCounters CallsCounters { get; } = new( 4 );

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                this.CallsCounters.Increment( 0 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                this.CallsCounters.Increment( 1 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                this.CallsCounters.Increment( 2 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                this.CallsCounters.Increment( 3 );

                return await Task.Run( () => new CachedValueClass( _counter++ ) );
            }
        }

        [Fact]
        public void TestRecachingAsync()
        {
            var cachingClass =
                new TestRecachingAsyncCachingClass();

            var cachedValue0 = new CachedValueChildClass( 0 );
            var cachedValue2 = new CachedValueChildClass( 2 );

            var cachedMethods =
                new[]
                {
                    () => cachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => cachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            var invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1 ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, cachedValue2 ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, cachedValue2, 3, 5 ).Wait()
                };

            var testName = "Matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testRecachingAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                testName,
                true,
                true );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    2,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }

            invalidatingMethods =
                new[]
                {
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0 ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0, cachedValue0 ).Wait(),
                    () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 0, cachedValue0, 0, 5 ).Wait()
                };

            testName = "Not matching values test";

            this.DoInvalidateCacheAttributeTest(
                _testRecachingAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );

            for ( var i = 0; i < cachedMethods.Length; i++ )
            {
                AssertEx.Equal(
                    4,
                    cachingClass.CallsCounters[i],
                    $"{testName}: The method #{i} has not been called the expected number of times." );
            }
        }

        #endregion TestRecachingAsync

        #region TestImperativeInvalidationWithNestedContexts

        private const string _testImperativeInvalidationWithNestedContextsProfileName =
            _profileNamePrefix + nameof(TestImperativeInvalidationWithNestedContexts);

        [CachingConfiguration( ProfileName = _testImperativeInvalidationWithNestedContextsProfileName )]
        public sealed class TestImperativeInvalidationWithNestedContextsClass
        {
            private int _invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this._invocations++;
            }
        }

        [Fact]
        public void TestImperativeInvalidationWithNestedContexts()
        {
            this.InitializeTestWithCachingBackend( _testImperativeInvalidationWithNestedContextsProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testImperativeInvalidationWithNestedContextsProfileName );

            try
            {
                var c = new TestImperativeInvalidationWithNestedContextsClass();
                var call1 = c.OuterMethod();
                CachingServices.Default.Invalidation.Invalidate( c.InnerMethod );
                var call2 = c.OuterMethod();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestImperativeInvalidationWithNestedContexts

        #region TestImperativeInvalidationWithNestedContextsAsync

        private const string _testImperativeInvalidationWithNestedContextsAsyncProfileName =
            _profileNamePrefix + nameof(TestImperativeInvalidationWithNestedContextsAsync);

        [CachingConfiguration( ProfileName = _testImperativeInvalidationWithNestedContextsAsyncProfileName )]
        public sealed class TestImperativeInvalidationWithNestedContextsAsyncClass
        {
            private int _invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this._invocations++ );
            }
        }

        [Fact]
        public async Task TestImperativeInvalidationWithNestedContextsAsync()
        {
            this.InitializeTestWithCachingBackend( _testImperativeInvalidationWithNestedContextsAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testImperativeInvalidationWithNestedContextsAsyncProfileName );

            try
            {
                var c = new TestImperativeInvalidationWithNestedContextsAsyncClass();
                var call1 = await c.OuterMethodAsync();
                await CachingServices.Default.Invalidation.InvalidateAsync( c.InnerMethodAsync );
                var call2 = await c.OuterMethodAsync();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestImperativeInvalidationWithNestedContextsAsync

        #region TestRecachingOfInnerMethod

        private const string _testRecachingOfInnerMethodProfileName =
            _profileNamePrefix + nameof(TestRecachingOfInnerMethod);

        [CachingConfiguration( ProfileName = _testRecachingOfInnerMethodProfileName )]
        public sealed class TestRecachingOfInnerMethodClass
        {
            private int _invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this._invocations++;
            }
        }

        [Fact]
        public void TestRecachingOfInnerMethod()
        {
            this.InitializeTestWithCachingBackend( _testRecachingOfInnerMethodProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testRecachingOfInnerMethodProfileName );

            try
            {
                var c = new TestRecachingOfInnerMethodClass();
                var call1 = c.OuterMethod();
                var call2 = CachingServices.Default.Invalidation.Recache( c.InnerMethod );
                var call3 = c.OuterMethod();

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

        private const string _testRecachingOfInnerMethodAsyncProfileName =
            _profileNamePrefix + nameof(TestRecachingOfInnerMethodAsync);

        [CachingConfiguration( ProfileName = _testRecachingOfInnerMethodAsyncProfileName )]
        public sealed class TestRecachingOfInnerMethodAsyncClass
        {
            private int _invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this._invocations++ );
            }
        }

        [Fact]
        public async Task TestRecachingOfInnerMethodAsync()
        {
            this.InitializeTestWithCachingBackend( _testRecachingOfInnerMethodAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testRecachingOfInnerMethodAsyncProfileName );

            try
            {
                var c = new TestRecachingOfInnerMethodAsyncClass();
                var call1 = await c.OuterMethodAsync();
                var call2 = await CachingServices.Default.Invalidation.RecacheAsync( c.InnerMethodAsync );
                var call3 = await c.OuterMethodAsync();

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

        private const string _testRecachingOfOuterMethodProfileName =
            _profileNamePrefix + nameof(TestRecachingOfOuterMethod);

        [CachingConfiguration( ProfileName = _testRecachingOfOuterMethodProfileName )]
        public sealed class TestRecachingOfOuterMethodClass
        {
            private int _invocations;

            [Cache]
            public int OuterMethod()
            {
                return this.InnerMethod();
            }

            [Cache]
            public int InnerMethod()
            {
                return this._invocations++;
            }
        }

        [Fact]
        public void TestRecachingOfOuterMethod()
        {
            this.InitializeTestWithCachingBackend( _testRecachingOfOuterMethodProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testRecachingOfOuterMethodProfileName );

            try
            {
                var c = new TestRecachingOfOuterMethodClass();
                var call1 = c.OuterMethod();
                var call2 = CachingServices.Default.Invalidation.Recache( c.OuterMethod );
                var call3 = c.OuterMethod();

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

        private const string _testRecachingOfOuterMethodAsyncProfileName =
            _profileNamePrefix + nameof(TestRecachingOfOuterMethodAsync);

        [CachingConfiguration( ProfileName = _testRecachingOfOuterMethodAsyncProfileName )]
        public sealed class TestRecachingOfOuterMethodAsyncClass
        {
            private int _invocations;

            [Cache]
            public async Task<int> OuterMethodAsync()
            {
                return await this.InnerMethodAsync();
            }

            [Cache]
            public async Task<int> InnerMethodAsync()
            {
                return await Task.FromResult( this._invocations++ );
            }
        }

        [Fact]
        public async Task TestRecachingOfOuterMethodAsync()
        {
            this.InitializeTestWithCachingBackend( _testRecachingOfOuterMethodAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( _testRecachingOfOuterMethodAsyncProfileName );

            try
            {
                var c = new TestRecachingOfOuterMethodAsyncClass();
                var call1 = await c.OuterMethodAsync();
                var call2 = await CachingServices.Default.Invalidation.RecacheAsync( c.OuterMethodAsync );
                var call3 = await c.OuterMethodAsync();

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

        private const string _testSimpleImperativeInvalidationWith0ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith0Parameters);

        private sealed class TestSimpleImperativeInvalidationWith0ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = _testSimpleImperativeInvalidationWith0ParametersProfileName )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith0Parameters()
        {
            var cachingClass = new TestSimpleImperativeInvalidationWith0ParametersCachingClass();

            this.DoTestSimpleImperativeInvalidation(
                _testSimpleImperativeInvalidationWith0ParametersProfileName,
                cachingClass.GetValue,
                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue ),
                cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeInvalidationWith0ParametersAsync

        private const string _testSimpleImperativeInvalidationWith0ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith0ParametersAsync);

        private sealed class TestSimpleImperativeInvalidationWith0ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = _testSimpleImperativeInvalidationWith0ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith0ParametersAsync()
        {
            var cachingClass =
                new TestSimpleImperativeInvalidationWith0ParametersAsyncCachingClass();

            await this.DoTestSimpleImperativeInvalidationAsync(
                _testSimpleImperativeInvalidationWith0ParametersAsyncProfileName,
                cachingClass.GetValueAsync,
                () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync ),
                cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeRecachingWith0Parameters

        private const string _testSimpleImperativeRecachingWith0ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith0Parameters);

        private sealed class TestSimpleImperativeRecachingWith0ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = _testSimpleImperativeRecachingWith0ParametersProfileName )]
            public override CachedValueClass GetValue()
            {
                return base.GetValue();
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith0Parameters()
        {
            var cachingClass = new TestSimpleImperativeRecachingWith0ParametersCachingClass();

            this.DoTestSimpleImperativeInvalidation(
                _testSimpleImperativeRecachingWith0ParametersProfileName,
                cachingClass.GetValue,
                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue ),
                cachingClass.Reset );
        }

        #endregion

        #region TestSimpleImperativeRecachingWith0ParametersAsync

        private const string _testSimpleImperativeRecachingWith0ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith0ParametersAsync);

        private sealed class TestSimpleImperativeRecachingWith0ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = _testSimpleImperativeRecachingWith0ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync()
            {
                return await base.GetValueAsync();
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith0ParametersAsync()
        {
            var cachingClass = new TestSimpleImperativeRecachingWith0ParametersAsyncCachingClass();

            await this.DoTestSimpleImperativeInvalidationAsync(
                _testSimpleImperativeRecachingWith0ParametersAsyncProfileName,
                cachingClass.GetValueAsync,
                () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync ),
                cachingClass.Reset );
        }

        #endregion

        public ImperativeInvalidationTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }
    }
}