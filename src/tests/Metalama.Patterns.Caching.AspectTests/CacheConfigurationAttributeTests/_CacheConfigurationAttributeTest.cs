// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Aspects;
using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

#pragma warning disable CA2201

public static class CacheConfigurationAttributeTest
{
    public static void CheckAfterCachedMethodCall(
        string calledMethod,
        TestingCacheBackend backend,
        ref string? previousCacheKey,
        string expectedProfileName )
    {
        if ( backend.LastCachedKey == previousCacheKey )
        {
            throw new Exception( $"{calledMethod}: backend.LastCachedKey '{backend.LastCachedKey}' == previousCacheKey '{previousCacheKey}'" );
        }

        if ( backend.LastCachedItem?.Configuration?.ProfileName != expectedProfileName )
        {
            throw new Exception(
                $"{calledMethod}: backend.LastCachedItem.Configuration.ProfileName '{backend.LastCachedItem?.Configuration?.ProfileName}' != expectedProfileName '{expectedProfileName}'" );
        }

        previousCacheKey = backend.LastCachedKey;
    }
}

[CachingConfiguration( UseDependencyInjection = false )]
public sealed class LocalChildCachingClass : ReferencedChildCachingClass
{
    [CachingConfiguration( UseDependencyInjection = false )]
    public sealed class LocalInnerCachingClassInChild
    {
        [Cache]
        public object GetValueLocalInnerChild()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueLocalChild()
    {
        return null!;
    }
}

[CachingConfiguration( UseDependencyInjection = false )]
public sealed class LocalChildCachingClassOverridden : ReferencedChildCachingClassOverridden
{
    [CachingConfiguration( UseDependencyInjection = false )]
    public sealed class LocalInnerCachingClassInChildOverridden
    {
        [Cache]
        public object GetValueLocalInnerChild()
        {
            return null!;
        }
    }

    [Cache]
    public object GetValueLocalChild()
    {
        return null!;
    }
}