// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests
{
    public static class TestValues
    {
        public const string defaultProfileName = CachingProfile.DefaultName;
        public const string cacheConfigurationAttributeProfileName1 = "namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests.CacheConfigurationAttribute1";
        public const string cacheConfigurationAttributeProfileName2 = "namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests.CacheConfigurationAttribute2";
    }

    public class ReferencedParentCachingClass
    {
        public class ReferencedInnerCachingClassInBase
        {
            [Caching.Cache]
            public object GetValueReferencedInnerBase()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueReferencedBase()
        {
            return null;
        }
    }

    public class ReferencedChildCachingClass : ReferencedParentCachingClass
    {
        public class ReferencedInnerCachingClassInChild
        {
            [Caching.Cache]
            public object GetValueReferencedInnerChild()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueReferencedChild()
        {
            return null;
        }
    }

    [Caching.CacheConfiguration( ProfileName = TestValues.cacheConfigurationAttributeProfileName2 )]
    public class ReferencedParentCachingClassOverridden
    {
        public class ReferencedInnerCachingClassInBaseOverridden
        {
            [Caching.Cache]
            public object GetValueReferencedInnerBase()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueReferencedBase()
        {
            return null;
        }
    }

    public class ReferencedChildCachingClassOverridden : ReferencedParentCachingClassOverridden
    {
        public class ReferencedInnerCachingClassInChildOverridden
        {
            [Caching.Cache]
            public object GetValueReferencedInnerChild()
            {
                return null;
            }
        }

        [Caching.Cache]
        public object GetValueReferencedChild()
        {
            return null;
        }
    }
}
