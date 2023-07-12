// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests;

#pragma warning disable CA2201

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable UnusedMethodReturnValue.Global
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

public sealed class LocalChildCachingClass : ReferencedChildCachingClass
{
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

public sealed class LocalChildCachingClassOverridden : ReferencedChildCachingClassOverridden
{
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