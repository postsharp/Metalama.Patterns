// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Implementation;

public static class CacheItemConfigurationExtensions
{
    public static CacheItemConfiguration CloneAsCacheItemConfiguration( [Required] this IRunTimeCacheItemConfiguration other )
    {
        if ( other is CacheItemConfiguration cacheItemConfiguration )
        {
            return cacheItemConfiguration.Clone();
        }

        return new CacheItemConfiguration()
        {
            IsEnabled = other.IsEnabled,
            ProfileName = other.ProfileName,
            AutoReload = other.AutoReload,
            AbsoluteExpiration = other.AbsoluteExpiration,
            SlidingExpiration = other.SlidingExpiration,
            Priority = other.Priority,
            IgnoreThisParameter = other.IgnoreThisParameter
        };
    }
}