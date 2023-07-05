using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching;

namespace PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute
{
    public static class TestValues
    {
        public const string defaultProfileName = CachingProfile.DefaultName;
        public const string cacheConfigurationAttributeProfileName1 = "PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute1";
        public const string cacheConfigurationAttributeProfileName2 = "PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute2";
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
