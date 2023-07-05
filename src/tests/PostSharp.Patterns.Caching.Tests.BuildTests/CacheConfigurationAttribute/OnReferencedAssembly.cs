// @Include(_CacheConfigurationAttributeTest.cs)

using System;
using System.Diagnostics;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Caching;

namespace PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute
{
    namespace OnReferencedAssembly
    {

        public class Program
        {
            public static void Main()
            {
                TestingCacheBackend backend =
                    TestProfileConfigurationFactory.InitializeTestWithTestingBackend( TestValues.cacheConfigurationAttributeProfileName1 );
                TestProfileConfigurationFactory.CreateProfile( TestValues.cacheConfigurationAttributeProfileName1 );
                TestProfileConfigurationFactory.CreateProfile( TestValues.cacheConfigurationAttributeProfileName2 );

                LocalChildCachingClass cachingClass = new LocalChildCachingClass();

                LocalChildCachingClassOverridden cachingClassOverridden = new LocalChildCachingClassOverridden();

                ReferencedParentCachingClass.ReferencedInnerCachingClassInBase referencedInnerCachingClassInBase
                    = new ReferencedParentCachingClass.ReferencedInnerCachingClassInBase();
                ReferencedChildCachingClass.ReferencedInnerCachingClassInChild referencedInnerCachingClassInChild
                    = new ReferencedChildCachingClass.ReferencedInnerCachingClassInChild();
                LocalChildCachingClass.LocalInnerCachingClassInChild localInnerCachingClassInChild
                    = new LocalChildCachingClass.LocalInnerCachingClassInChild();

                ReferencedParentCachingClassOverridden.ReferencedInnerCachingClassInBaseOverridden referencedInnerCachingClassInBaseOverridden
                    = new ReferencedParentCachingClassOverridden.ReferencedInnerCachingClassInBaseOverridden();
                ReferencedChildCachingClassOverridden.ReferencedInnerCachingClassInChildOverridden referencedInnerCachingClassInChildOverridden
                    = new ReferencedChildCachingClassOverridden.ReferencedInnerCachingClassInChildOverridden();
                LocalChildCachingClassOverridden.LocalInnerCachingClassInChildOverridden localInnerCachingClassInChildOverridden
                    = new LocalChildCachingClassOverridden.LocalInnerCachingClassInChildOverridden();

                try
                {
                    string previousCachedKey = null;

                    if ( backend.LastCachedKey != null )
                    {
                        throw new Exception( "backend.LastCachedKey != null" );
                    }

                    if ( backend.LastCachedItem != null )
                    {
                        throw new Exception( "backend.LastCachedItem != null" );
                    }

                    cachingClass.GetValueReferencedBase();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "referenced base", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    cachingClass.GetValueReferencedChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "referenced child", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    cachingClass.GetValueLocalChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "local child", backend, ref previousCachedKey,
                        TestValues.defaultProfileName );

                    cachingClassOverridden.GetValueReferencedBase();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden referenced base", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName2 );

                    cachingClassOverridden.GetValueReferencedChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden referenced child", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName2 );

                    cachingClassOverridden.GetValueLocalChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden local child", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName2 );

                    referencedInnerCachingClassInBase.GetValueReferencedInnerBase();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "referenced inner base", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    referencedInnerCachingClassInChild.GetValueReferencedInnerChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "referenced inner child", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    localInnerCachingClassInChild.GetValueLocalInnerChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "local inner child", backend, ref previousCachedKey,
                        TestValues.defaultProfileName );

                    referencedInnerCachingClassInBaseOverridden.GetValueReferencedInnerBase();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden referenced inner base", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    referencedInnerCachingClassInChildOverridden.GetValueReferencedInnerChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden referenced inner child", backend, ref previousCachedKey,
                        TestValues.cacheConfigurationAttributeProfileName1 );

                    localInnerCachingClassInChildOverridden.GetValueLocalInnerChild();
                    CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                        "overridden local inner child", backend, ref previousCachedKey,
                        TestValues.defaultProfileName );
                }
                finally
                {
                    TestProfileConfigurationFactory.DisposeTest();
                }
            }
        }
    }
}
