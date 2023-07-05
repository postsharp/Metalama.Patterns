// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Diagnostics;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Caching;

namespace PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute
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
