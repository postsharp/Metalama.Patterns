// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

[assembly: CacheConfiguration( ProfileName = TestValues.CacheConfigurationAttributeProfileName1 )]

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

public static class TestValues
{
    public const string DefaultProfileName = CachingProfile.DefaultName;
    public const string CacheConfigurationAttributeProfileName1 = "[A]";
    public const string CacheConfigurationAttributeProfileName2 = "[B]";
}

public class ReferencedParentCachingClass
{
    public sealed class ReferencedInnerCachingClassInBase
    {
        [Cache]
        public object GetValueReferencedInnerBase()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueReferencedBase()
    {
        return null!;
    }
}

public class ReferencedChildCachingClass : ReferencedParentCachingClass
{
    public sealed class ReferencedInnerCachingClassInChild
    {
        [Cache]
        public object GetValueReferencedInnerChild()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueReferencedChild()
    {
        return null!;
    }
}

[CacheConfiguration( ProfileName = TestValues.CacheConfigurationAttributeProfileName2 )]
public class ReferencedParentCachingClassOverridden
{
    public sealed class ReferencedInnerCachingClassInBaseOverridden
    {
        [Cache]
        public object GetValueReferencedInnerBase()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueReferencedBase()
    {
        return null!;
    }
}

public class ReferencedChildCachingClassOverridden : ReferencedParentCachingClassOverridden
{
    public sealed class ReferencedInnerCachingClassInChildOverridden
    {
        [Cache]
        public object GetValueReferencedInnerChild()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueReferencedChild()
    {
        return null!;
    }
}