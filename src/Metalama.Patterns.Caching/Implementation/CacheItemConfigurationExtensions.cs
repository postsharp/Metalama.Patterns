// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Implementation;

internal static class CacheItemConfigurationExtensions
{
    public static ICacheItemConfiguration? WithoutAutoReload( this ICacheItemConfiguration? other )
        => other switch
        {
            null => null,
            { AutoReload: false } => other,
            _ => other.AsCacheItemConfiguration() with { AutoReload = false }
        };

    public static CacheItemConfiguration AsCacheItemConfiguration( [Required] this ICacheItemConfiguration other )
    {
        if ( other is CacheItemConfiguration cacheItemConfiguration )
        {
            return cacheItemConfiguration;
        }

        return new CacheItemConfiguration()
        {
            ProfileName = other.ProfileName,
            AutoReload = other.AutoReload,
            AbsoluteExpiration = other.AbsoluteExpiration,
            SlidingExpiration = other.SlidingExpiration,
            Priority = other.Priority,
            IgnoreThisParameter = other.IgnoreThisParameter
        };
    }
}