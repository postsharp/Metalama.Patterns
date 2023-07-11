// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using System;

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests
{
    public static class CacheConfigurationAttributeTest
    {
        public static void CheckAfterCachedMethodCall(string calledMethod, TestingCacheBackend backend, ref string previousCacheKey,
                                                        string expectedProfileName)
        {
            if (backend.LastCachedKey == previousCacheKey)
            {
                throw new Exception($"{calledMethod}: backend.LastCachedKey '{backend.LastCachedKey}' == previousCacheKey '{previousCacheKey}'");
            }

            if (backend.LastCachedItem.Configuration.ProfileName != expectedProfileName)
            {
                throw new Exception(
                    $"{calledMethod}: backend.LastCachedItem.Configuration.ProfileName '{backend.LastCachedItem.Configuration.ProfileName}' != expectedProfileName '{expectedProfileName}'");
            }

            previousCacheKey = backend.LastCachedKey;
        }
    }

    public class LocalChildCachingClass : ReferencedChildCachingClass
    {
        public class LocalInnerCachingClassInChild
        {
            [Caching.Cache]
            public object GetValueLocalInnerChild()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueLocalChild()
        {
            return null;
        }
    }

    public class LocalChildCachingClassOverridden : ReferencedChildCachingClassOverridden
    {
        public class LocalInnerCachingClassInChildOverridden
        {
            [Caching.Cache]
            public object GetValueLocalInnerChild()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueLocalChild()
        {
            return null;
        }
    }
}