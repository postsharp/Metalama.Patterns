// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metalama.Patterns.Caching.TestHelpers;

using Xunit;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class ObjectDependenciesTests
    {
        private const string profileNamePrefix = "Caching.Tests.ObjectDependenciesTests_";

        #region TestOneDependency

        private const string testOneDependencyProfileName = profileNamePrefix + "TestOneDependency";

        [CacheConfiguration( ProfileName = testOneDependencyProfileName )]
        private sealed class TestOneDependencyCachingClass : CachingClass
        {
            [Cache]
            public override CachedValueClass GetValueAsDependency()
            {
                return base.GetValueAsDependency();
            }
        }

        [Fact]
        public void TestOneDependency()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testOneDependencyProfileName );
            TestProfileConfigurationFactory.CreateProfile( testOneDependencyProfileName );

            try
            {
                var cachingClass = new TestOneDependencyCachingClass();
                var currentId = 0;

                CachedValueClass value1 = cachingClass.GetValueAsDependency();
                bool called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache should be empty." );
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );

                CachedValueClass value2 = cachingClass.GetValueAsDependency();
                called = cachingClass.Reset();
                Assert.False( called, "The method was called when its first return value should be cached." );

                AssertEx.Equal(
                    value1,
                    value2,
                    "The first value, which should be returned from the cache, is not the same as the one which should have been cached." );

                CachingServices.Invalidation.Invalidate( value1 );

                ++currentId;
                CachedValueClass value3 = cachingClass.GetValueAsDependency();
                called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache item should be invalidated." );
                AssertEx.Equal( currentId, value3.Id, "The second given value has unexpected ID." );

                CachedValueClass value4 = cachingClass.GetValueAsDependency();
                called = cachingClass.Reset();
                Assert.False( called, "The method was called when its second return value should be cached." );

                AssertEx.Equal(
                    value3,
                    value4,
                    "The second value, which should be returned from the cache, is not the same as the one which should have been cached." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestOneDependency

        #region TestOneDependencyAsync

        private const string testOneDependencyAsyncProfileName = profileNamePrefix + "TestOneDependencyAsync";

        [CacheConfiguration( ProfileName = testOneDependencyAsyncProfileName )]
        private sealed class TestOneDependencyAsyncCachingClass : CachingClass
        {
            [Cache]
            public override async Task<CachedValueClass> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task TestOneDependencyAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testOneDependencyAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testOneDependencyAsyncProfileName );

            try
            {
                var cachingClass = new TestOneDependencyAsyncCachingClass();
                var currentId = 0;

                CachedValueClass value1 = await cachingClass.GetValueAsDependencyAsync();
                bool called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache should be empty." );
                AssertEx.Equal( currentId, value1.Id, "The first given value has unexpected ID." );

                CachedValueClass value2 = await cachingClass.GetValueAsDependencyAsync();
                called = cachingClass.Reset();
                Assert.False( called, "The method was called when its first return value should be cached." );

                AssertEx.Equal(
                    value1,
                    value2,
                    "The first value, which should be returned from the cache, is not the same as the one which should have been cached." );

                await CachingServices.Invalidation.InvalidateAsync( value1 );

                ++currentId;
                CachedValueClass value3 = await cachingClass.GetValueAsDependencyAsync();
                called = cachingClass.Reset();
                Assert.True( called, "The method was not called when the cache item should be invalidated." );
                AssertEx.Equal( currentId, value3.Id, "The second given value has unexpected ID." );

                CachedValueClass value4 = await cachingClass.GetValueAsDependencyAsync();
                called = cachingClass.Reset();
                Assert.False( called, "The method was called when its second return value should be cached." );

                AssertEx.Equal(
                    value3,
                    value4,
                    "The second value, which should be returned from the cache, is not the same as the one which should have been cached." );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestOneDependencyAsync

        #region TestNestedDependencies

        private const string testNestedDependenciesProfileName = profileNamePrefix + "TestNestedDependencies";

        private sealed class TestNestedDependenciesCachedValueClass1 : CachedValueClass { }

        private sealed class TestNestedDependenciesCachedValueClass2 : CachedValueClass { }

        [CacheConfiguration( ProfileName = testNestedDependenciesProfileName )]
        private sealed class TestNestedDependenciesCachingClass1 : CachingClass<TestNestedDependenciesCachedValueClass1>
        {
            [Cache]
            public override TestNestedDependenciesCachedValueClass1 GetValueAsDependency()
            {
                return base.GetValueAsDependency();
            }
        }

        [CacheConfiguration( ProfileName = testNestedDependenciesProfileName )]
        private sealed class TestNestedDependenciesCachingClass2 : CachingClass<TestNestedDependenciesCachedValueClass2>
        {
            public TestNestedDependenciesCachingClass1 Class1 { get; private set; }

            public TestNestedDependenciesCachedValueClass1 Value1 { get; private set; }

            public TestNestedDependenciesCachingClass2()
            {
                this.Class1 = new TestNestedDependenciesCachingClass1();
            }

            [Cache]
            public override TestNestedDependenciesCachedValueClass2 GetValueAsDependency()
            {
                this.Value1 = this.Class1.GetValueAsDependency();

                return base.GetValueAsDependency();
            }
        }

        [Fact]
        public void TestNestedDependencies()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testNestedDependenciesProfileName );
            TestProfileConfigurationFactory.CreateProfile( testNestedDependenciesProfileName );

            try
            {
                var cachingClass2 = new TestNestedDependenciesCachingClass2();
                var cachingClass1 = cachingClass2.Class1;

                CachedValueClass value1 = cachingClass2.GetValueAsDependency();
                cachingClass1.Reset();
                cachingClass2.Reset();

                CachingServices.Invalidation.Invalidate( value1 );

                cachingClass2.GetValueAsDependency();
                bool called = cachingClass1.Reset();
                Assert.True( called, "The method result did not get invalidated by the automatic dependency." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestNestedDependencies

        #region TestNestedDependenciesAsync

        private const string testNestedDependenciesAsyncProfileName = profileNamePrefix + "TestNestedDependenciesAsync";

        private sealed class TestNestedDependenciesAsyncCachedValueClass1 : CachedValueClass { }

        private sealed class TestNestedDependenciesAsyncCachedValueClass2 : CachedValueClass { }

        [CacheConfiguration( ProfileName = testNestedDependenciesAsyncProfileName )]
        private sealed class TestNestedDependenciesAsyncCachingClass1 : CachingClass<TestNestedDependenciesAsyncCachedValueClass1>
        {
            [Cache]
            public override async Task<TestNestedDependenciesAsyncCachedValueClass1> GetValueAsDependencyAsync()
            {
                return await base.GetValueAsDependencyAsync();
            }
        }

        [CacheConfiguration( ProfileName = testNestedDependenciesAsyncProfileName )]
        private sealed class TestNestedDependenciesAsyncCachingClass2 : CachingClass<TestNestedDependenciesAsyncCachedValueClass2>
        {
            public TestNestedDependenciesAsyncCachingClass1 Class1 { get; private set; }

            public TestNestedDependenciesAsyncCachedValueClass1 Value1 { get; private set; }

            public TestNestedDependenciesAsyncCachingClass2()
            {
                this.Class1 = new TestNestedDependenciesAsyncCachingClass1();
            }

            [Cache]
            public override async Task<TestNestedDependenciesAsyncCachedValueClass2> GetValueAsDependencyAsync()
            {
                this.Value1 = await this.Class1.GetValueAsDependencyAsync();

                return await base.GetValueAsDependencyAsync();
            }
        }

        [Fact]
        public async Task TestNestedDependenciesAsync()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testNestedDependenciesAsyncProfileName );
            TestProfileConfigurationFactory.CreateProfile( testNestedDependenciesAsyncProfileName );

            try
            {
                var cachingClass2 = new TestNestedDependenciesAsyncCachingClass2();
                var cachingClass1 = cachingClass2.Class1;

                CachedValueClass value1 = await cachingClass2.GetValueAsDependencyAsync();
                cachingClass1.Reset();
                cachingClass2.Reset();

                await CachingServices.Invalidation.InvalidateAsync( value1 );

                await cachingClass2.GetValueAsDependencyAsync();
                bool called = cachingClass1.Reset();
                Assert.True( called, "The method result did not get invalidated by the automatic dependency." );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        #endregion TestNestedDependenciesAsync

        #region TestNestedDependenciesWithEnumerable

        private const string testNestedDependenciesWithEnumerableProfileName = profileNamePrefix + "TestNestedDependenciesWithEnumerable";

        private sealed class TestNestedDependenciesWithEnumerableCachedValueClass : CachedValueClass { }

        private sealed class TestNestedDependenciesWithEnumerableCachingClass1 : CachingClass<TestNestedDependenciesWithEnumerableCachedValueClass> { }

        [CacheConfiguration( ProfileName = testNestedDependenciesWithEnumerableProfileName )]
        private sealed class TestNestedDependenciesWithEnumerableCachingClass2
        {
            private int class1CallCount = 0;

            private readonly TestNestedDependenciesWithEnumerableCachingClass1 class1 = new();

            [Cache]
            public IEnumerable<TestNestedDependenciesWithEnumerableCachedValueClass> GetTwoValuesAsDependencies()
            {
                for ( var i = 0; i < 2; i++ )
                {
                    yield return this.class1.GetValueAsDependency();

                    if ( this.class1.Reset() )
                    {
                        ++this.class1CallCount;
                    }
                }
            }

            public int Reset()
            {
                var count = this.class1CallCount;
                this.class1CallCount = 0;

                return count;
            }
        }

        [Fact]
        public void TestNestedDependenciesWithEnumerable()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( testNestedDependenciesWithEnumerableProfileName );
            TestProfileConfigurationFactory.CreateProfile( testNestedDependenciesWithEnumerableProfileName );

            try
            {
                var cachingClass2 = new TestNestedDependenciesWithEnumerableCachingClass2();

                IList<TestNestedDependenciesWithEnumerableCachedValueClass> value1 = cachingClass2.GetTwoValuesAsDependencies().ToList();
                var class1CallsCount = cachingClass2.Reset();
                AssertEx.Equal( value1.Count, class1CallsCount, "The method did not get called for the first time." );

                CachingServices.Invalidation.Invalidate( value1[0] );

                IList<TestNestedDependenciesWithEnumerableCachedValueClass> value2 = cachingClass2.GetTwoValuesAsDependencies().ToList();
                class1CallsCount = cachingClass2.Reset();
                AssertEx.Equal( value2.Count, class1CallsCount, "The method result did not get invalidated by the automatic dependency." );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        #endregion TestNestedDependenciesWithEnumerable
    }
}