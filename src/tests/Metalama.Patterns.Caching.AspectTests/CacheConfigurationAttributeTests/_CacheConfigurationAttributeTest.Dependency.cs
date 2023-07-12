// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

[assembly: CacheConfiguration( ProfileName = TestValues.cacheConfigurationAttributeProfileName1 )]

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests
{
    public static class TestValues
    {
        public const string defaultProfileName = CachingProfile.DefaultName;
        public const string cacheConfigurationAttributeProfileName1 = "[A]";
        public const string cacheConfigurationAttributeProfileName2 = "[B]";
    }

    public class ReferencedParentCachingClass
    {
        public class ReferencedInnerCachingClassInBase
        {
            [Cache]
            public object GetValueReferencedInnerBase()
            {
                return null;
            }
        }

        [Cache]
        public object GetValueReferencedBase()
        {
            return null;
        }
    }

    public class ReferencedChildCachingClass : ReferencedParentCachingClass
    {
        public class ReferencedInnerCachingClassInChild
        {
            [Cache]
            public object GetValueReferencedInnerChild()
            {
                return null;
            }
        }

        [Cache]
        public object GetValueReferencedChild()
        {
            return null;
        }
    }

    [CacheConfiguration( ProfileName = TestValues.cacheConfigurationAttributeProfileName2 )]
    public class ReferencedParentCachingClassOverridden
    {
        public class ReferencedInnerCachingClassInBaseOverridden
        {
            [Cache]
            public object GetValueReferencedInnerBase()
            {
                return null;
            }
        }

        [Cache]
        public object GetValueReferencedBase()
        {
            return null;
        }
    }

    public class ReferencedChildCachingClassOverridden : ReferencedParentCachingClassOverridden
    {
        public class ReferencedInnerCachingClassInChildOverridden
        {
            [Cache]
            public object GetValueReferencedInnerChild()
            {
                return null;
            }
        }

        [Cache]
        public object GetValueReferencedChild()
        {
            return null;
        }
    }
}