// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class InvalidateCacheAttributeTests : InvalidationTestsBase
    {
        private const string profileNamePrefix = "Caching.Tests.InvalidateCacheAttributeTests_";

        #region TestFromDifferentTypeIgnoringThisParameter

        private const string testFromDifferentTypeIgnoringThisParameterProfileName = profileNamePrefix + nameof(TestFromDifferentTypeIgnoringThisParameter);

        [CacheConfiguration( ProfileName = testFromDifferentTypeIgnoringThisParameterProfileName )]
        private sealed class TestFromDifferentTypeIgnoringThisParameterCachingClass
        {
            private static int counter = 0;

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }
        }

        private sealed class TestFromDifferentTypeIgnoringThisParameterInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }
        }

        [Fact]
        public void TestFromDifferentTypeIgnoringThisParameter()
        {
            var cachingClass =
                new TestFromDifferentTypeIgnoringThisParameterCachingClass();

            var invalidatingClass =
                new TestFromDifferentTypeIgnoringThisParameterInvalidatingClass();

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

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingClass.Invalidate( 1, cachedMethods[1] ),
                    () => invalidatingClass.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => invalidatingClass.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingClass.Invalidate( 0, cachedMethods[1] ),
                    () => invalidatingClass.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => invalidatingClass.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeIgnoringThisParameter

        #region TestFromDifferentTypeIgnoringThisParameterAsync

        private const string testFromDifferentTypeIgnoringThisParameterAsyncProfileName =
            profileNamePrefix + nameof(TestFromDifferentTypeIgnoringThisParameterAsync);

        [CacheConfiguration( ProfileName = testFromDifferentTypeIgnoringThisParameterAsyncProfileName )]
        private sealed class TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass
        {
            private static int counter = 0;

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }
        }

        private sealed class TestFromDifferentTypeIgnoringThisParameterAsyncInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass),
                nameof(TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }
        }

        [Fact]
        public void TestFromDifferentTypeIgnoringThisParameterAsync()
        {
            var cachingClass =
                new TestFromDifferentTypeIgnoringThisParameterAsyncCachingClass();

            var invalidatingClass =
                new TestFromDifferentTypeIgnoringThisParameterAsyncInvalidatingClass();

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

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeIgnoringThisParameterAsync

        #region TestFromDerivedTypeNotIgnoringThisParameter

        private const string testFromDerivedTypeNotIgnoringThisParameterProfileName = profileNamePrefix + nameof(TestFromDerivedTypeNotIgnoringThisParameter);

        [CacheConfiguration( ProfileName = testFromDerivedTypeNotIgnoringThisParameterProfileName )]
        private class TestFromDerivedTypeNotIgnoringThisParameterCachingClass
        {
            private static int counter = 0;

            [Cache]
            public CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }
        }

        private sealed class TestFromDerivedTypeNotIgnoringThisParameterInvalidatingClass : TestFromDerivedTypeNotIgnoringThisParameterCachingClass
        {
            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterCachingClass),
                nameof(GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterCachingClass),
                nameof(GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterCachingClass),
                nameof(GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterCachingClass),
                nameof(GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }
        }

        [Fact]
        public void TestFromDerivedTypeNotIgnoringThisParameter()
        {
            var invalidatingAndCachingClass =
                new TestFromDerivedTypeNotIgnoringThisParameterInvalidatingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    invalidatingAndCachingClass.GetValue,
                    () => invalidatingAndCachingClass.GetValue( 1 ),
                    () => invalidatingAndCachingClass.GetValue( 1, cachedValue2 ),
                    () => invalidatingAndCachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingAndCachingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingAndCachingClass.Invalidate( 1, cachedMethods[1] ),
                    () => invalidatingAndCachingClass.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => invalidatingAndCachingClass.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDerivedTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingAndCachingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingAndCachingClass.Invalidate( 0, cachedMethods[1] ),
                    () => invalidatingAndCachingClass.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => invalidatingAndCachingClass.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDerivedTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDerivedTypeNotIgnoringThisParameter

        #region TestFromDerivedTypeNotIgnoringThisParameterAsync

        private const string testFromDerivedTypeNotIgnoringThisParameterAsyncProfileName =
            profileNamePrefix + nameof(TestFromDerivedTypeNotIgnoringThisParameterAsync);

        [CacheConfiguration( ProfileName = testFromDerivedTypeNotIgnoringThisParameterAsyncProfileName )]
        private class TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass
        {
            private static int counter = 0;

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }
        }

        private sealed class TestFromDerivedTypeNotIgnoringThisParameterAsyncInvalidatingClass : TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass
        {
            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass),
                nameof(GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass),
                nameof(GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass),
                nameof(GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDerivedTypeNotIgnoringThisParameterAsyncCachingClass),
                nameof(GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }
        }

        [Fact]
        public void TestFromDerivedTypeNotIgnoringThisParameterAsync()
        {
            var invalidatingAndCachingClass =
                new TestFromDerivedTypeNotIgnoringThisParameterAsyncInvalidatingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingAndCachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingAndCachingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDerivedTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingAndCachingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingAndCachingClass.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDerivedTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDerivedTypeNotIgnoringThisParameterAsync

        #region TestFromDifferentTypeInstanceMethodOnStaticMethod

        private const string testFromDifferentTypeInstanceMethodOnStaticMethodProfileName =
            profileNamePrefix + nameof(TestFromDifferentTypeInstanceMethodOnStaticMethod);

        [CacheConfiguration( ProfileName = testFromDifferentTypeInstanceMethodOnStaticMethodProfileName )]
        private sealed class TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass
        {
            private static int counter = 0;

            [Cache]
            public static CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }
        }

        private sealed class TestFromDifferentTypeInstanceMethodOnStaticMethodInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }
        }

        [Fact]
        public void TestFromDifferentTypeInstanceMethodOnStaticMethod()
        {
            var invalidatingClass =
                new TestFromDifferentTypeInstanceMethodOnStaticMethodInvalidatingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue,
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue( 1 ),
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue( 1, cachedValue2 ),
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodCachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingClass.Invalidate( 1, cachedMethods[1] ),
                    () => invalidatingClass.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => invalidatingClass.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeInstanceMethodOnStaticMethodProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.Invalidate( cachedMethods[0] ),
                    () => invalidatingClass.Invalidate( 0, cachedMethods[1] ),
                    () => invalidatingClass.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => invalidatingClass.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeInstanceMethodOnStaticMethodProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeInstanceMethodOnStaticMethod

        #region TestFromDifferentTypeInstanceMethodOnStaticMethodAsync

        private const string testFromDifferentTypeInstanceMethodOnStaticMethodAsyncProfileName =
            profileNamePrefix + nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsync);

        [CacheConfiguration( ProfileName = testFromDifferentTypeInstanceMethodOnStaticMethodAsyncProfileName )]
        private sealed class TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass
        {
            private static int counter = 0;

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }
        }

        private sealed class TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }
        }

        [Fact]
        public void TestFromDifferentTypeInstanceMethodOnStaticMethodAsync()
        {
            var invalidatingClass =
                new TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncInvalidatingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeInstanceMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeInstanceMethodOnStaticMethodAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => invalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => invalidatingClass.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeInstanceMethodOnStaticMethodAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeInstanceMethodOnStaticMethodAsync

        #region TestFromDifferentTypeStaticMethodOnStaticMethod

        private const string testFromDifferentTypeStaticMethodOnStaticMethodProfileName =
            profileNamePrefix + nameof(TestFromDifferentTypeStaticMethodOnStaticMethod);

        [CacheConfiguration( ProfileName = testFromDifferentTypeStaticMethodOnStaticMethodProfileName )]
        private sealed class TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass
        {
            private static int counter = 0;

            [Cache]
            public static CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public static CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }
        }

        private sealed class TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public static CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public static CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public static CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue),
                AllowMultipleOverloads = true )]
            public static CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }
        }

        [Fact]
        public void TestFromDifferentTypeStaticMethodOnStaticMethod()
        {
            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue,
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue( 1 ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue( 1, cachedValue2 ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodCachingClass.GetValue( 1, cachedValue2, 3, 4 )
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( cachedMethods[0] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 1, cachedMethods[1] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeStaticMethodOnStaticMethodProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( cachedMethods[0] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 0, cachedMethods[1] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodInvalidatingClass.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeStaticMethodOnStaticMethodProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeStaticMethodOnStaticMethod

        #region TestFromDifferentTypeStaticMethodOnStaticMethodAsync

        private const string testFromDifferentTypeStaticMethodOnStaticMethodAsyncProfileName =
            profileNamePrefix + nameof(TestFromDifferentTypeStaticMethodOnStaticMethodAsync);

        [CacheConfiguration( ProfileName = testFromDifferentTypeStaticMethodOnStaticMethodAsyncProfileName )]
        private sealed class TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass
        {
            private static int counter = 0;

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public static async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }
        }

        private sealed class TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass
        {
            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public static async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public static async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public static async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache(
                typeof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass),
                nameof(TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync),
                AllowMultipleOverloads = true )]
            public static async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }
        }

        [Fact]
        public void TestFromDifferentTypeStaticMethodOnStaticMethodAsync()
        {
            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync().GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncCachingClass.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass
                        .InvalidateAsync( 1, cachedValue2, cachedMethods[2] )
                        .GetAwaiter()
                        .GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass
                        .InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] )
                        .GetAwaiter()
                        .GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeStaticMethodOnStaticMethodAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass
                        .InvalidateAsync( 0, cachedValue0, cachedMethods[2] )
                        .GetAwaiter()
                        .GetResult(),
                    () => TestFromDifferentTypeStaticMethodOnStaticMethodAsyncInvalidatingClass
                        .InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] )
                        .GetAwaiter()
                        .GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromDifferentTypeStaticMethodOnStaticMethodAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching values test",
                true,
                false );
        }

        #endregion TestFromDifferentTypeStaticMethodOnStaticMethodAsync

        #region TestFromTheSameTypeIgnoringThisParameter

        private const string testFromTheSameTypeIgnoringThisParameterProfileName =
            profileNamePrefix + nameof(TestFromTheSameTypeIgnoringThisParameter);

        [CacheConfiguration( ProfileName = testFromTheSameTypeIgnoringThisParameterProfileName )]
        private sealed class TestFromTheSameTypeIgnoringThisParameterInvalidatingAndCachingClass
        {
            private static int counter = 0;

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache( IgnoreThisParameter = true )]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            // We use ToString() to distinguish target type instances by default
            public override string ToString()
            {
                return $"{this.GetType().Name}@{this.GetHashCode()}";
            }
        }

        [Fact]
        public void TestFromTheSameTypeIgnoringThisParameter()
        {
            var testClass1 =
                new TestFromTheSameTypeIgnoringThisParameterInvalidatingAndCachingClass();

            var testClass2 =
                new TestFromTheSameTypeIgnoringThisParameterInvalidatingAndCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    testClass1.GetValue,
                    () => testClass1.GetValue( 1 ),
                    () => testClass1.GetValue( 1, cachedValue2 ),
                    () => testClass1.GetValue( 1, cachedValue2, 3, 4 )
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.Invalidate( cachedMethods[0] ),
                    () => testClass1.Invalidate( 1, cachedMethods[1] ),
                    () => testClass1.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => testClass1.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.Invalidate( cachedMethods[0] ),
                    () => testClass1.Invalidate( 0, cachedMethods[1] ),
                    () => testClass1.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => testClass1.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with matching this value test",
                true,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.Invalidate( cachedMethods[0] ),
                    () => testClass2.Invalidate( 1, cachedMethods[1] ),
                    () => testClass2.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => testClass2.Invalidate( 1, cachedValue2, 3, 4, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching parameter values with not matching this value test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.Invalidate( cachedMethods[0] ),
                    () => testClass2.Invalidate( 0, cachedMethods[1] ),
                    () => testClass2.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => testClass2.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with not matching this value test",
                true,
                false );
        }

        #endregion TestFromTheSameTypeIgnoringThisParameter

        #region TestFromTheSameTypeIgnoringThisParameterAsync

        private const string testFromTheSameTypeIgnoringThisParameterAsyncProfileName =
            profileNamePrefix + nameof(TestFromTheSameTypeIgnoringThisParameterAsync);

        [CacheConfiguration( ProfileName = testFromTheSameTypeIgnoringThisParameterAsyncProfileName )]
        private sealed class TestFromTheSameTypeIgnoringThisParameterAsyncInvalidatingAndCachingClass
        {
            private static int counter = 0;

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache( IgnoreThisParameter = true )]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            // We use ToString() to distinguish target type instances by default
            public override string ToString()
            {
                return $"{this.GetType().Name}@{this.GetHashCode()}";
            }
        }

        [Fact]
        public void TestFromTheSameTypeIgnoringThisParameterAsync()
        {
            var testClass1 =
                new TestFromTheSameTypeIgnoringThisParameterAsyncInvalidatingAndCachingClass();

            var testClass2 =
                new TestFromTheSameTypeIgnoringThisParameterAsyncInvalidatingAndCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.GetValueAsync().GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with matching this value test",
                true,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedValue2, 3, 4, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching parameter values with not matching this value test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with not matching this value test",
                true,
                false );
        }

        #endregion TestFromTheSameTypeIgnoringThisParameterAsync

        #region TestFromTheSameTypeNotIgnoringThisParameter

        private const string testFromTheSameTypeNotIgnoringThisParameterProfileName =
            profileNamePrefix + nameof(TestFromTheSameTypeNotIgnoringThisParameter);

        [CacheConfiguration( ProfileName = testFromTheSameTypeNotIgnoringThisParameterProfileName )]
        private sealed class TestFromTheSameTypeNotIgnoringThisParameterInvalidatingAndCachingClass
        {
            private static int counter = 0;

            [Cache]
            public CachedValueClass GetValue()
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2 )
            {
                return new CachedValueClass( counter++ );
            }

            [Cache]
            public CachedValueClass GetValue( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return new CachedValueClass( counter++ );
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            [InvalidateCache( nameof(GetValue), AllowMultipleOverloads = true )]
            public CachedValueClass Invalidate( int param1, CachedValueChildClass param2, int param3, int param4, Func<CachedValueClass> cachedMethod )
            {
                return cachedMethod.Invoke();
            }

            // We use ToString() to distinguish target type instances by default
            public override string ToString()
            {
                return $"{this.GetType().Name}@{this.GetHashCode()}";
            }
        }

        [Fact]
        public void TestFromTheSameTypeNotIgnoringThisParameter()
        {
            var testClass1 =
                new TestFromTheSameTypeNotIgnoringThisParameterInvalidatingAndCachingClass();

            var testClass2 =
                new TestFromTheSameTypeNotIgnoringThisParameterInvalidatingAndCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    testClass1.GetValue,
                    () => testClass1.GetValue( 1 ),
                    () => testClass1.GetValue( 1, cachedValue2 ),
                    () => testClass1.GetValue( 1, cachedValue2, 3, 4 )
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.Invalidate( cachedMethods[0] ),
                    () => testClass1.Invalidate( 1, cachedMethods[1] ),
                    () => testClass1.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => testClass1.Invalidate( 1, cachedValue2, 3, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.Invalidate( cachedMethods[0] ),
                    () => testClass1.Invalidate( 0, cachedMethods[1] ),
                    () => testClass1.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => testClass1.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with matching this value test",
                true,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.Invalidate( cachedMethods[0] ),
                    () => testClass2.Invalidate( 1, cachedMethods[1] ),
                    () => testClass2.Invalidate( 1, cachedValue2, cachedMethods[2] ),
                    () => testClass2.Invalidate( 1, cachedValue2, 3, 4, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching parameter values with not matching this value test",
                false,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.Invalidate( cachedMethods[0] ),
                    () => testClass2.Invalidate( 0, cachedMethods[1] ),
                    () => testClass2.Invalidate( 0, cachedValue0, cachedMethods[2] ),
                    () => testClass2.Invalidate( 0, cachedValue0, 0, 5, cachedMethods[3] )
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with not matching this value test",
                false,
                false );
        }

        #endregion TestFromTheSameTypeNotIgnoringThisParameter

        #region TestFromTheSameTypeNotIgnoringThisParameterAsync

        private const string testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName =
            profileNamePrefix + nameof(TestFromTheSameTypeNotIgnoringThisParameterAsync);

        [CacheConfiguration( ProfileName = testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName )]
        private sealed class TestFromTheSameTypeNotIgnoringThisParameterAsyncInvalidatingAndCachingClass
        {
            private static int counter = 0;

            [Cache]
            public async Task<CachedValueClass> GetValueAsync()
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [Cache]
            public async Task<CachedValueClass> GetValueAsync( int param1, CachedValueClass param2, int param3, [NotCacheKey] int param4 )
            {
                return await Task.FromResult( new CachedValueClass( counter++ ) );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync( int param1, CachedValueChildClass param2, Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            [InvalidateCache( nameof(GetValueAsync), AllowMultipleOverloads = true )]
            public async Task<CachedValueClass> InvalidateAsync(
                int param1,
                CachedValueChildClass param2,
                int param3,
                int param4,
                Func<CachedValueClass> cachedMethod )
            {
                return await Task.FromResult( cachedMethod() );
            }

            // We use ToString() to distinguish target type instances by default
            public override string ToString()
            {
                return $"{this.GetType().Name}@{this.GetHashCode()}";
            }
        }

        [Fact]
        public void TestFromTheSameTypeNotIgnoringThisParameterAsync()
        {
            var testClass1 =
                new TestFromTheSameTypeNotIgnoringThisParameterAsyncInvalidatingAndCachingClass();

            var testClass2 =
                new TestFromTheSameTypeNotIgnoringThisParameterAsyncInvalidatingAndCachingClass();

            CachedValueChildClass cachedValue0 = new CachedValueChildClass( 0 );
            CachedValueChildClass cachedValue2 = new CachedValueChildClass( 2 );

            Func<CachedValueClass>[] cachedMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.GetValueAsync().GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1 ).GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1, cachedValue2 ).GetAwaiter().GetResult(),
                    () => testClass1.GetValueAsync( 1, cachedValue2, 3, 4 ).GetAwaiter().GetResult()
                };

            Func<CachedValueClass>[] invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 1, cachedValue2, 3, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching values test",
                true,
                true );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass1.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass1.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with matching this value test",
                true,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedValue2, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 1, cachedValue2, 3, 4, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Matching parameter values with not matching this value test",
                false,
                false );

            invalidatingMethods =
                new Func<CachedValueClass>[]
                {
                    () => testClass2.InvalidateAsync( cachedMethods[0] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedMethods[1] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedValue0, cachedMethods[2] ).GetAwaiter().GetResult(),
                    () => testClass2.InvalidateAsync( 0, cachedValue0, 0, 5, cachedMethods[3] ).GetAwaiter().GetResult()
                };

            DoInvalidateCacheAttributeTest(
                testFromTheSameTypeNotIgnoringThisParameterAsyncProfileName,
                cachedMethods,
                invalidatingMethods,
                "Not matching parameter values with not matching this value test",
                false,
                false );
        }

        #endregion TestFromTheSameTypeNotIgnoringThisParameterAsync

        #region TestNestedContexts

        private const string testNestedContextsProfileName =
            profileNamePrefix + nameof(TestNestedContexts);

        [CacheConfiguration( ProfileName = testNestedContextsProfileName )]
        public sealed class TestNestedContextsClass
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

            [InvalidateCache( nameof(InnerMethod) )]
            public void InvalidateInnerMethod() { }
        }

        [Fact]
        public void TestNestedContexts()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testNestedContextsProfileName );
            TestProfileConfigurationFactory.CreateProfile( testNestedContextsProfileName );

            try
            {
                var c = new TestNestedContextsClass();
                var call1 = c.OuterMethod();
                c.InvalidateInnerMethod();
                var call2 = c.OuterMethod();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestNestedContexts

        #region TestNestedContextsAsync

        private const string testNestedContextsAsyncProfileName =
            profileNamePrefix + nameof(TestNestedContextsAsync);

        [CacheConfiguration( ProfileName = testNestedContextsAsyncProfileName )]
        public sealed class TestNestedContextsAsyncClass
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

            [InvalidateCache( nameof(InnerMethodAsync) )]
            public async Task InvalidateInnerMethodAsync()
            {
                await Task.CompletedTask;
            }
        }

        [Fact]
        public async Task TestNestedContextsAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testNestedContextsAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testNestedContextsAsyncProfileName );

            try
            {
                var c = new TestNestedContextsAsyncClass();
                var call1 = await c.OuterMethodAsync();
                await c.InvalidateInnerMethodAsync();
                var call2 = await c.OuterMethodAsync();

                Assert.NotEqual( call1, call2 );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestNestedContextsAsync
    }
}