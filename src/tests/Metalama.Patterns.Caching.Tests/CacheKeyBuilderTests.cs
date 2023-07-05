// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Common.Tests.Helpers;
using Metalama.Patterns.Diagnostics;

namespace Metalama.Patterns.Caching.Tests
{
    public class CacheKeyBuilderTests
    {
        private class MyCacheKeyBuilder : CacheKeyBuilder
        {
            public string LastMethodKey;

            public override string BuildMethodKey( MethodInfo method, IList<object> arguments, object instance = null )
            {
                return this.LastMethodKey = base.BuildMethodKey( method, arguments, instance );
            }
        }

        private void DoTestMethod( string profileName, string expectedKey, Func<string> action )
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                var keyBuilder = new MyCacheKeyBuilder();
                CachingServices.DefaultKeyBuilder = keyBuilder;
                action();
                Console.WriteLine( keyBuilder.LastMethodKey );
                Assert.Equal( expectedKey, keyBuilder.LastMethodKey );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private async Task DoTestMethodAsync( string profileName, string expectedKey, Func<Task<string>> action )
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                var keyBuilder = new MyCacheKeyBuilder();
                CachingServices.DefaultKeyBuilder = keyBuilder;
                await action();
                Console.WriteLine( keyBuilder.LastMethodKey );
                Assert.Equal( expectedKey, keyBuilder.LastMethodKey );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        private const string profileNamePrefix = "Caching.Tests.CacheKeyBuilderTests_";

        #region TestInstanceMethod

        private const string testInstanceMethodProfileName = profileNamePrefix + "TestInstanceMethod";

        [Cache( ProfileName = testInstanceMethodProfileName )]
        private string CachedInstanceMethod()
        {
            return "";
        }

        [Fact]
        public void TestInstanceMethod()
        {
            this.DoTestMethod(
                testInstanceMethodProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethod(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests})",
                this.CachedInstanceMethod );
        }

        #endregion TestInstanceMethod

        #region TestInstanceMethodAsync

        private const string testInstanceMethodAsyncProfileName = profileNamePrefix + "TestInstanceMethodAsync";

        [Cache( ProfileName = testInstanceMethodAsyncProfileName )]
        private async Task<string> CachedInstanceMethodAsync()
        {
            await Task.Yield();

            return "";
        }

        [Fact]
        public async Task TestInstanceMethodAsync()
        {
            await this.DoTestMethodAsync(
                testInstanceMethodAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethodAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests})",
                this.CachedInstanceMethodAsync );
        }

        #endregion TestInstanceMethodAsync

        #region TestStaticMethod

        private const string testStaticMethodProfileName = profileNamePrefix + "TestStaticMethod";

        [Cache( ProfileName = testStaticMethodProfileName )]
        private static string CachedStaticMethod()
        {
            return "";
        }

        [Fact]
        public void TestStaticMethod()
        {
            this.DoTestMethod(
                testStaticMethodProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedStaticMethod()",
                CachedStaticMethod );
        }

        #endregion TestStaticMethod

        #region TestStaticMethodAsync

        private const string testStaticMethodAsyncProfileName = profileNamePrefix + "TestStaticMethodAsync";

        [Cache( ProfileName = testStaticMethodAsyncProfileName )]
        private static async Task<string> CachedStaticMethodAsync()
        {
            await Task.Yield();

            return "";
        }

        [Fact]
        public async Task TestStaticMethodAsync()
        {
            await this.DoTestMethodAsync(
                testStaticMethodAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedStaticMethodAsync()",
                CachedStaticMethodAsync );
        }

        #endregion TestStaticMethodAsync

        #region TestMethodWithParameters

        private const string testMethodWithParametersProfileName = profileNamePrefix + "TestMethodWithParameters";

        [Cache( ProfileName = testMethodWithParametersProfileName )]
        private string CachedInstanceMethodWithParameters( int intParam, string stringParam, object objectParam )
        {
            return "CachedInstanceMethodWithParameters1";
        }

        [Cache( ProfileName = testMethodWithParametersProfileName )]
        private string CachedInstanceMethodWithParameters( int intParam, object objectParam1, object objectParam2 )
        {
            return "CachedInstanceMethodWithParameters2";
        }

        [Fact]
        public void TestMethodWithParameters()
        {
            this.DoTestMethod(
                testMethodWithParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethodWithParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests}, (int) 0, (string) null, (object) null)",
                () => this.CachedInstanceMethodWithParameters( 0, null, null ) );
        }

        #endregion TestMethodWithParameters

        #region TestMethodWithParametersAsync

        private const string testMethodWithParametersAsyncProfileName = profileNamePrefix + "TestMethodWithParametersAsync";

        [Cache( ProfileName = testMethodWithParametersAsyncProfileName )]
        private async Task<string> CachedInstanceMethodWithParametersAsync( int intParam, string stringParam, object objectParam )
        {
            await Task.Yield();

            return "CachedInstanceMethodWithParametersAsync1";
        }

        [Cache( ProfileName = testMethodWithParametersAsyncProfileName )]
        private async Task<string> CachedInstanceMethodWithParametersAsync( int intParam, object objectParam1, object objectParam2 )
        {
            await Task.Yield();

            return "CachedInstanceMethodWithParametersAsync2";
        }

        [Fact]
        public async Task TestMethodWithParametersAsync()
        {
            await this.DoTestMethodAsync(
                testMethodWithParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethodWithParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests}, (int) 0, (string) null, (object) null)",
                () => this.CachedInstanceMethodWithParametersAsync( 0, null, null ) );
        }

        #endregion TestMethodWithParametersAsync

        #region TestMethodWithIgnoredParameters

        private const string testMethodWithIgnoredParametersProfileName = profileNamePrefix + "TestMethodWithIgnoredParameters";

        [Cache( ProfileName = testMethodWithIgnoredParametersProfileName )]
        private string CachedInstanceMethodWithIgnoredParameters( int intParam, object objectParam1, [NotCacheKey] int ignored )
        {
            return "CachedInstanceMethodWithIgnoredParameters";
        }

        [Fact]
        public void TestMethodWithIgnoredParameters()
        {
            this.DoTestMethod(
                testMethodWithIgnoredParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethodWithIgnoredParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests}, (int) 0, (object) null, (int) *)",
                () => this.CachedInstanceMethodWithIgnoredParameters( 0, null, -1 ) );
        }

        #endregion TestMethodWithIgnoredParameters

        #region TestMethodWithIgnoredParametersAsync

        private const string testMethodWithIgnoredParametersAsyncProfileName = profileNamePrefix + "TestMethodWithIgnoredParametersAsync";

        [Cache( ProfileName = testMethodWithIgnoredParametersAsyncProfileName )]
        private async Task<string> CachedInstanceMethodWithIgnoredParametersAsync( int intParam, object objectParam1, [NotCacheKey] int ignored )
        {
            await Task.Yield();

            return "CachedInstanceMethodWithIgnoredParametersAsync";
        }

        [Fact]
        public async Task TestMethodWithIgnoredParametersAsync()
        {
            await this.DoTestMethodAsync(
                testMethodWithIgnoredParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.CachedInstanceMethodWithIgnoredParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests}, (int) 0, (object) null, (int) *)",
                () => this.CachedInstanceMethodWithIgnoredParametersAsync( 0, null, -1 ) );
        }

        #endregion TestMethodWithIgnoredParametersAsync

        #region TestMethodWithIgnoredThisParameter

        private const string testMethodWithIgnoredThisParameterProfileName = profileNamePrefix + "TestMethodWithIgnoredThisParameter";

        [CacheConfiguration( ProfileName = testMethodWithIgnoredThisParameterProfileName, IgnoreThisParameter = true )]
        private class SomeClassWithIgnoredThisParameter
        {
            [Cache]
            public string SomeInstanceMethod()
            {
                return "SomeInstanceMethod";
            }
        }

        [Fact]
        public void TestMethodWithIgnoredThisParameter()
        {
            this.DoTestMethod(
                testMethodWithIgnoredThisParameterProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.SomeClassWithIgnoredThisParameter.SomeInstanceMethod()",
                () => new SomeClassWithIgnoredThisParameter().SomeInstanceMethod() );
        }

        #endregion TestMethodWithIgnoredThisParameter

        #region TestMethodWithIgnoredThisParameterAsync

        private const string testMethodWithIgnoredThisParameterAsyncProfileName = profileNamePrefix + "TestMethodWithIgnoredThisParameterAsync";

        [CacheConfiguration( ProfileName = testMethodWithIgnoredThisParameterAsyncProfileName, IgnoreThisParameter = true )]
        private class SomeAsyncClassWithIgnoredThisParameter
        {
            [Cache]
            public async Task<string> SomeInstanceMethodAsync()
            {
                await Task.Yield();

                return "SomeInstanceMethod";
            }
        }

        [Fact]
        public async Task TestMethodWithIgnoredThisParameterAsync()
        {
            await this.DoTestMethodAsync(
                testMethodWithIgnoredThisParameterAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.SomeAsyncClassWithIgnoredThisParameter.SomeInstanceMethodAsync()",
                () => new SomeAsyncClassWithIgnoredThisParameter().SomeInstanceMethodAsync() );
        }

        #endregion TestMethodWithIgnoredThisParameterAsync

        private class TestClassForCollections
        {
            public virtual string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return "CachedInstatceMethodWithCollectionParameters";
            }
        }

        private class AsyncTestClassForCollections
        {
            public virtual async Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                await Task.Yield();

                return "CachedInstatceMethodWithCollectionParameters";
            }
        }

        #region TestMethodWithNullCollectionParameters

        private const string testMethodWithNullCollectionParametersProfileName = profileNamePrefix + "TestMethodWithNullCollectionParameters";

        private class TestClassForNullCollectionParameters : TestClassForCollections
        {
            [Cache( ProfileName = testMethodWithNullCollectionParametersProfileName )]
            public override string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParameters( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public void TestMethodWithNullCollectionParameters()
        {
            var testObject = new TestClassForNullCollectionParameters();

            this.DoTestMethod(
                testMethodWithNullCollectionParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForNullCollectionParameters.CachedInstatceMethodWithCollectionParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForNullCollectionParameters}, (IEnumerable<int>) null, (IEnumerable<object>) null, (List<int>) null, (List<object>) null, (int[]) null, (object[]) null)",
                () => testObject.CachedInstatceMethodWithCollectionParameters(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null ) );
        }

        #endregion TestMethodWithNullCollectionParameters

        #region TestMethodWithNullCollectionParametersAsync

        private const string testMethodWithNullCollectionParametersAsyncProfileName = profileNamePrefix + "TestMethodWithNullCollectionParametersAsync";

        private class AsyncTestClassForNullCollectionParameters : AsyncTestClassForCollections
        {
            [Cache( ProfileName = testMethodWithNullCollectionParametersAsyncProfileName )]
            public override Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParametersAsync( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public async Task TestMethodWithNullCollectionParametersAsync()
        {
            var testObject = new AsyncTestClassForNullCollectionParameters();

            await this.DoTestMethodAsync(
                testMethodWithNullCollectionParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForNullCollectionParameters.CachedInstatceMethodWithCollectionParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForNullCollectionParameters}, (IEnumerable<int>) null, (IEnumerable<object>) null, (List<int>) null, (List<object>) null, (int[]) null, (object[]) null)",
                () => testObject.CachedInstatceMethodWithCollectionParametersAsync(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null ) );
        }

        #endregion TestMethodWithNullCollectionParametersAsync

        #region TestMethodWithEmptyCollectionParameters

        private const string testMethodWithEmptyCollectionParametersProfileName = profileNamePrefix + "TestMethodWithEmptyCollectionParameters";

        private class TestClassForEmptyCollectionParameters : TestClassForCollections
        {
            [Cache( ProfileName = testMethodWithEmptyCollectionParametersProfileName )]
            public override string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParameters( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public void TestMethodWithEmptyCollectionParameters()
        {
            var testObject = new TestClassForEmptyCollectionParameters();

            this.DoTestMethod(
                testMethodWithEmptyCollectionParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForEmptyCollectionParameters.CachedInstatceMethodWithCollectionParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForEmptyCollectionParameters}, (IEnumerable<int>) [], (IEnumerable<object>) [], (List<int>) [], (List<object>) [], (int[]) [], (object[]) [])",
                () => testObject.CachedInstatceMethodWithCollectionParameters(
                    new List<int>(),
                    new List<object>(),
                    new List<int>(),
                    new List<object>(),
                    new int[] { },
                    new object[] { } ) );
        }

        #endregion TestMethodWithEmptyCollectionParameters

        #region TestMethodWithEmptyCollectionParametersAsync

        private const string testMethodWithEmptyCollectionParametersAsyncProfileName = profileNamePrefix + "TestMethodWithEmptyCollectionParametersAsync";

        private class AsyncTestClassForEmptyCollectionParameters : AsyncTestClassForCollections
        {
            [Cache( ProfileName = testMethodWithEmptyCollectionParametersAsyncProfileName )]
            public override Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParametersAsync( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public async Task TestMethodWithEmptyCollectionParametersAsync()
        {
            var testObject = new AsyncTestClassForEmptyCollectionParameters();

            await this.DoTestMethodAsync(
                testMethodWithEmptyCollectionParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForEmptyCollectionParameters.CachedInstatceMethodWithCollectionParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForEmptyCollectionParameters}, (IEnumerable<int>) [], (IEnumerable<object>) [], (List<int>) [], (List<object>) [], (int[]) [], (object[]) [])",
                () => testObject.CachedInstatceMethodWithCollectionParametersAsync(
                    new List<int>(),
                    new List<object>(),
                    new List<int>(),
                    new List<object>(),
                    new int[] { },
                    new object[] { } ) );
        }

        #endregion TestMethodWithEmptyCollectionParametersAsync

        #region TestMethodWithOneItemCollectionParameters

        private const string testMethodWithOneItemCollectionParametersProfileName = profileNamePrefix + "TestMethodWithOneItemCollectionParameters";

        private class TestClassForOneCollectionParameters : TestClassForCollections
        {
            [Cache( ProfileName = testMethodWithOneItemCollectionParametersProfileName )]
            public override string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParameters( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public void TestMethodWithOneItemCollectionParameters()
        {
            var testObject = new TestClassForOneCollectionParameters();

            this.DoTestMethod(
                testMethodWithOneItemCollectionParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForOneCollectionParameters.CachedInstatceMethodWithCollectionParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForOneCollectionParameters}, (IEnumerable<int>) [ 1 ], (IEnumerable<object>) [ \"Object1\" ], (List<int>) [ 2 ], (List<object>) [ \"Object2\" ], (int[]) [ 3 ], (object[]) [ \"Object3\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParameters(
                    new List<int>( new[] { 1 } ),
                    new List<object>( new object[] { "Object1" } ),
                    new List<int>( new[] { 2 } ),
                    new List<object>( new object[] { "Object2" } ),
                    new[] { 3 },
                    new object[] { "Object3" } ) );
        }

        #endregion TestMethodWithOneItemCollectionParameters

        #region TestMethodWithOneItemCollectionParametersAsync

        private const string testMethodWithOneItemCollectionParametersAsyncProfileName = profileNamePrefix + "TestMethodWithOneItemCollectionParametersAsync";

        private class AsyncTestClassForOneCollectionParameters : AsyncTestClassForCollections
        {
            [Cache( ProfileName = testMethodWithOneItemCollectionParametersAsyncProfileName )]
            public override Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParametersAsync( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public async Task TestMethodWithOneItemCollectionParametersAsync()
        {
            var testObject = new AsyncTestClassForOneCollectionParameters();

            await this.DoTestMethodAsync(
                testMethodWithOneItemCollectionParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForOneCollectionParameters.CachedInstatceMethodWithCollectionParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForOneCollectionParameters}, (IEnumerable<int>) [ 1 ], (IEnumerable<object>) [ \"Object1\" ], (List<int>) [ 2 ], (List<object>) [ \"Object2\" ], (int[]) [ 3 ], (object[]) [ \"Object3\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParametersAsync(
                    new List<int>( new[] { 1 } ),
                    new List<object>( new object[] { "Object1" } ),
                    new List<int>( new[] { 2 } ),
                    new List<object>( new object[] { "Object2" } ),
                    new[] { 3 },
                    new object[] { "Object3" } ) );
        }

        #endregion TestMethodWithOneItemCollectionParametersAsync

        #region TestMethodWithTwoItemCollectionParameters

        private const string testMethodWithTwoItemCollectionParametersProfileName = profileNamePrefix + "TestMethodWithTwoItemCollectionParameters";

        private class TestClassForTwoCollectionParameters : TestClassForCollections
        {
            [Cache( ProfileName = testMethodWithTwoItemCollectionParametersProfileName )]
            public override string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParameters( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public void TestMethodWithTwoItemCollectionParameters()
        {
            var testObject = new TestClassForTwoCollectionParameters();

            this.DoTestMethod(
                testMethodWithTwoItemCollectionParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForTwoCollectionParameters.CachedInstatceMethodWithCollectionParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForTwoCollectionParameters}, (IEnumerable<int>) [ 1, 2 ], (IEnumerable<object>) [ \"Object1\", \"Object2\" ], (List<int>) [ 3, 4 ], (List<object>) [ \"Object3\", \"Object4\" ], (int[]) [ 5, 6 ], (object[]) [ \"Object5\", \"Object6\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParameters(
                    new List<int>( new[] { 1, 2 } ),
                    new List<object>( new object[] { "Object1", "Object2" } ),
                    new List<int>( new[] { 3, 4 } ),
                    new List<object>( new object[] { "Object3", "Object4" } ),
                    new[] { 5, 6 },
                    new object[] { "Object5", "Object6" } ) );
        }

        #endregion TestMethodWithTwoItemCollectionParameters

        #region TestMethodWithTwoItemCollectionParametersAsync

        private const string testMethodWithTwoItemCollectionParametersAsyncProfileName = profileNamePrefix + "TestMethodWithTwoItemCollectionParametersAsync";

        private class AsyncTestClassForTwoCollectionParameters : AsyncTestClassForCollections
        {
            [Cache( ProfileName = testMethodWithTwoItemCollectionParametersAsyncProfileName )]
            public override Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParametersAsync( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public async Task TestMethodWithTwoItemCollectionParametersAsync()
        {
            var testObject = new AsyncTestClassForTwoCollectionParameters();

            await this.DoTestMethodAsync(
                testMethodWithTwoItemCollectionParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForTwoCollectionParameters.CachedInstatceMethodWithCollectionParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForTwoCollectionParameters}, (IEnumerable<int>) [ 1, 2 ], (IEnumerable<object>) [ \"Object1\", \"Object2\" ], (List<int>) [ 3, 4 ], (List<object>) [ \"Object3\", \"Object4\" ], (int[]) [ 5, 6 ], (object[]) [ \"Object5\", \"Object6\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParametersAsync(
                    new List<int>( new[] { 1, 2 } ),
                    new List<object>( new object[] { "Object1", "Object2" } ),
                    new List<int>( new[] { 3, 4 } ),
                    new List<object>( new object[] { "Object3", "Object4" } ),
                    new[] { 5, 6 },
                    new object[] { "Object5", "Object6" } ) );
        }

        #endregion TestMethodWithTwoItemCollectionParametersAsync

        #region TestMethodWithThreeItemCollectionParameters

        private const string testMethodWithThreeItemCollectionParametersProfileName = profileNamePrefix + "TestMethodWithThreeItemCollectionParameters";

        private class TestClassForThreeCollectionParameters : TestClassForCollections
        {
            [Cache( ProfileName = testMethodWithThreeItemCollectionParametersProfileName )]
            public override string CachedInstatceMethodWithCollectionParameters(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParameters( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public void TestMethodWithThreeItemCollectionParameters()
        {
            var testObject = new TestClassForThreeCollectionParameters();

            this.DoTestMethod(
                testMethodWithThreeItemCollectionParametersProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForThreeCollectionParameters.CachedInstatceMethodWithCollectionParameters(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.TestClassForThreeCollectionParameters}, (IEnumerable<int>) [ 1, 2, 3 ], (IEnumerable<object>) [ \"Object1\", \"Object2\", \"Object3\" ], (List<int>) [ 4, 5, 6 ], (List<object>) [ \"Object4\", \"Object5\", \"Object6\" ], (int[]) [ 7, 8, 9 ], (object[]) [ \"Object7\", \"Object8\", \"Object9\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParameters(
                    new List<int>( new[] { 1, 2, 3 } ),
                    new List<object>( new object[] { "Object1", "Object2", "Object3" } ),
                    new List<int>( new[] { 4, 5, 6 } ),
                    new List<object>( new object[] { "Object4", "Object5", "Object6" } ),
                    new[] { 7, 8, 9 },
                    new object[] { "Object7", "Object8", "Object9" } ) );
        }

        #endregion TestMethodWithThreeItemCollectionParameters

        #region TestMethodWithThreeItemCollectionParametersAsync

        private const string testMethodWithThreeItemCollectionParametersAsyncProfileName =
            profileNamePrefix + "TestMethodWithThreeItemCollectionParametersAsync";

        private class AsyncTestClassForThreeCollectionParameters : AsyncTestClassForCollections
        {
            [Cache( ProfileName = testMethodWithThreeItemCollectionParametersAsyncProfileName )]
            public override Task<string> CachedInstatceMethodWithCollectionParametersAsync(
                IEnumerable<int> intEnumerable,
                IEnumerable<object> objectEnumerable,
                List<int> intList,
                List<object> objectList,
                int[] intArray,
                object[] objectArray )
            {
                return base.CachedInstatceMethodWithCollectionParametersAsync( intEnumerable, objectEnumerable, intList, objectList, intArray, objectArray );
            }
        }

        [Fact]
        public async Task TestMethodWithThreeItemCollectionParametersAsync()
        {
            var testObject = new AsyncTestClassForThreeCollectionParameters();

            await this.DoTestMethodAsync(
                testMethodWithThreeItemCollectionParametersAsyncProfileName,
                "PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForThreeCollectionParameters.CachedInstatceMethodWithCollectionParametersAsync(this={PostSharp.Patterns.Caching.Tests.CacheKeyBuilderTests.AsyncTestClassForThreeCollectionParameters}, (IEnumerable<int>) [ 1, 2, 3 ], (IEnumerable<object>) [ \"Object1\", \"Object2\", \"Object3\" ], (List<int>) [ 4, 5, 6 ], (List<object>) [ \"Object4\", \"Object5\", \"Object6\" ], (int[]) [ 7, 8, 9 ], (object[]) [ \"Object7\", \"Object8\", \"Object9\" ])",
                () => testObject.CachedInstatceMethodWithCollectionParametersAsync(
                    new List<int>( new[] { 1, 2, 3 } ),
                    new List<object>( new object[] { "Object1", "Object2", "Object3" } ),
                    new List<int>( new[] { 4, 5, 6 } ),
                    new List<object>( new object[] { "Object4", "Object5", "Object6" } ),
                    new[] { 7, 8, 9 },
                    new object[] { "Object7", "Object8", "Object9" } ) );
        }

        #endregion TestMethodWithThreeItemCollectionParametersAsync
    }
}