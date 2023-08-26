// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if TEST_OPTIONS
// @Include(_CacheConfigurationAttributeTest.cs)
// @RemoveOutputCode
// @MainMethod(TestMain)
#endif

using Metalama.Patterns.Caching.TestHelpers;

// ReSharper disable once CheckNamespace
namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests.OnReferencedAssembly;

#pragma warning disable CA2201

public class Program
{
    public static void TestMain()
    {
        Console.WriteLine( "Test started." );

        var backend =
            TestProfileConfigurationFactory.InitializeTestWithTestingBackend( TestValues.CacheConfigurationAttributeProfileName1, null );

        TestProfileConfigurationFactory.CreateProfile( TestValues.CacheConfigurationAttributeProfileName1 );
        TestProfileConfigurationFactory.CreateProfile( TestValues.CacheConfigurationAttributeProfileName2 );

        var cachingClass = new LocalChildCachingClass();

        var cachingClassOverridden = new LocalChildCachingClassOverridden();

        var referencedInnerCachingClassInBase
            = new ReferencedParentCachingClass.ReferencedInnerCachingClassInBase();

        var referencedInnerCachingClassInChild
            = new ReferencedChildCachingClass.ReferencedInnerCachingClassInChild();

        var localInnerCachingClassInChild
            = new LocalChildCachingClass.LocalInnerCachingClassInChild();

        var referencedInnerCachingClassInBaseOverridden
            = new ReferencedParentCachingClassOverridden.ReferencedInnerCachingClassInBaseOverridden();

        var referencedInnerCachingClassInChildOverridden
            = new ReferencedChildCachingClassOverridden.ReferencedInnerCachingClassInChildOverridden();

        var localInnerCachingClassInChildOverridden
            = new LocalChildCachingClassOverridden.LocalInnerCachingClassInChildOverridden();

        try
        {
            string? previousCachedKey = null;

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
                "referenced base",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            cachingClass.GetValueReferencedChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "referenced child",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            cachingClass.GetValueLocalChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "local child",
                backend,
                ref previousCachedKey,
                TestValues.DefaultProfileName );

            cachingClassOverridden.GetValueReferencedBase();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden referenced base",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName2 );

            cachingClassOverridden.GetValueReferencedChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden referenced child",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName2 );

            cachingClassOverridden.GetValueLocalChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden local child",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName2 );

            referencedInnerCachingClassInBase.GetValueReferencedInnerBase();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "referenced inner base",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            referencedInnerCachingClassInChild.GetValueReferencedInnerChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "referenced inner child",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            localInnerCachingClassInChild.GetValueLocalInnerChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "local inner child",
                backend,
                ref previousCachedKey,
                TestValues.DefaultProfileName );

            referencedInnerCachingClassInBaseOverridden.GetValueReferencedInnerBase();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden referenced inner base",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            referencedInnerCachingClassInChildOverridden.GetValueReferencedInnerChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden referenced inner child",
                backend,
                ref previousCachedKey,
                TestValues.CacheConfigurationAttributeProfileName1 );

            localInnerCachingClassInChildOverridden.GetValueLocalInnerChild();

            CacheConfigurationAttributeTest.CheckAfterCachedMethodCall(
                "overridden local inner child",
                backend,
                ref previousCachedKey,
                TestValues.DefaultProfileName );

            Console.WriteLine( "Test completed." );
        }
        finally
        {
            TestProfileConfigurationFactory.DisposeTest();
        }
    }
}