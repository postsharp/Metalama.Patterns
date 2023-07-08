// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class CacheConfigurationAttributeTests
    {
        private const string profileNamePrefix = "Caching.Tests.CacheConfigurationAttributeTests_";

        private const string testCachingAttributeProfileName = profileNamePrefix + "TestCachingAttribute";

        [CacheConfiguration( ProfileName = testCachingAttributeProfileName )]
        private class BaseCachingClass
        {
            public class InnerCachingClassInBase
            {
                [Cache]
                public object GetValueInnerBase()
                {
                    return null;
                }

                [Cache]
                public async Task<object> GetValueInnerBaseAsync()
                {
                    await Task.Yield();

                    return null;
                }
            }

            [Cache]
            public object GetValueBase()
            {
                return null;
            }

            [Cache]
            public async Task<object> GetValueBaseAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        private class ChildCachingClass : BaseCachingClass
        {
            public class InnerCachingClassInChild
            {
                [Cache]
                public object GetValueInnerChild()
                {
                    return null;
                }

                [Cache]
                public async Task<object> GetValueInnerChildAsync()
                {
                    await Task.Yield();

                    return null;
                }
            }

            [Cache]
            public object GetValueChild()
            {
                return null;
            }

            [Cache]
            public async Task<object> GetValueChildAsync()
            {
                await Task.Yield();

                return null;
            }
        }

        private static void DoCachingAttributeTest( Func<object> getValueAction, bool defaultProfile )
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCachingAttributeProfileName );

            TestProfileConfigurationFactory.CreateProfile( testCachingAttributeProfileName );

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                getValueAction.Invoke();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal(
                    defaultProfile ? CachingProfile.DefaultName : testCachingAttributeProfileName,
                    backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        private static async Task DoCachingAttributeTestAsync( Func<Task<object>> getValueAction, bool defaultProfile )
        {
            TestingCacheBackend backend =
                TestProfileConfigurationFactory.InitializeTestWithTestingBackend( testCachingAttributeProfileName );

            TestProfileConfigurationFactory.CreateProfile( testCachingAttributeProfileName );

            try
            {
                Assert.Null( backend.LastCachedKey );
                Assert.Null( backend.LastCachedItem );

                await getValueAction.Invoke();

                Assert.NotNull( backend.LastCachedKey );
                Assert.NotNull( backend.LastCachedItem );

                Assert.Equal(
                    defaultProfile ? CachingProfile.DefaultName : testCachingAttributeProfileName,
                    backend.LastCachedItem.Configuration.ProfileName );
            }
            finally
            {
                await TestProfileConfigurationFactory.DisposeTestAsync();
            }
        }

        [Fact]
        public void TestCachingAttributeBase()
        {
            var cachingClass = new ChildCachingClass();
            DoCachingAttributeTest( cachingClass.GetValueBase, false );
        }

        [Fact]
        public async Task TestCachingAttributeBaseAsync()
        {
            var cachingClass = new ChildCachingClass();
            await DoCachingAttributeTestAsync( cachingClass.GetValueBaseAsync, false );
        }

        [Fact]
        public void TestCachingAttributeChild()
        {
            var cachingClass = new ChildCachingClass();
            DoCachingAttributeTest( cachingClass.GetValueChild, false );
        }

        [Fact]
        public async Task TestCachingAttributeChildAsync()
        {
            var cachingClass = new ChildCachingClass();
            await DoCachingAttributeTestAsync( cachingClass.GetValueChildAsync, false );
        }

        [Fact]
        public void TestCachingAttributeInnerInBase()
        {
            var cachingClass = new BaseCachingClass.InnerCachingClassInBase();
            DoCachingAttributeTest( cachingClass.GetValueInnerBase, true );
        }

        [Fact]
        public async Task TestCachingAttributeInnerInBaseAsync()
        {
            var cachingClass = new BaseCachingClass.InnerCachingClassInBase();
            await DoCachingAttributeTestAsync( cachingClass.GetValueInnerBaseAsync, true );
        }

        [Fact]
        public void TestCachingAttributeInnerInBaseChild()
        {
            var cachingClass = new ChildCachingClass.InnerCachingClassInChild();
            DoCachingAttributeTest( cachingClass.GetValueInnerChild, true );
        }

        [Fact]
        public async Task TestCachingAttributeInnerInBaseChildAsync()
        {
            var cachingClass = new ChildCachingClass.InnerCachingClassInChild();
            await DoCachingAttributeTestAsync( cachingClass.GetValueInnerChildAsync, true );
        }
    }
}