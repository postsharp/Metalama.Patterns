﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Aspects;
using Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

[assembly: CachingConfiguration( ProfileName = TestValues.CacheConfigurationAttributeProfileNameA )]

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

public static class TestValues
{
    public const string DefaultProfileName = CachingProfile.DefaultName;
    public const string CacheConfigurationAttributeProfileNameA = "[A]";
    public const string CacheConfigurationAttributeProfileNameB = "[B]";
}

[CachingConfiguration( UseDependencyInjection = false )]
public class ReferencedParentCachingClass
{
    [CachingConfiguration( UseDependencyInjection = false )]
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

[CachingConfiguration( UseDependencyInjection = false )]
public class ReferencedChildCachingClass : ReferencedParentCachingClass
{
    [CachingConfiguration( UseDependencyInjection = false )]
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

[CachingConfiguration( ProfileName = TestValues.CacheConfigurationAttributeProfileNameB, UseDependencyInjection = false )]
public class ReferencedParentCachingClassOverridden
{
    [CachingConfiguration( UseDependencyInjection = false )]
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

[CachingConfiguration( UseDependencyInjection = false )]
public class ReferencedChildCachingClassOverridden : ReferencedParentCachingClassOverridden
{
    [CachingConfiguration( UseDependencyInjection = false )]
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