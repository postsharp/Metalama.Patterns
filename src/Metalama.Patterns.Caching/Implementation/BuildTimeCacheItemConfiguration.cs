// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a cached method at build time.
/// </summary>
/// <param name="ProfileName"><inheritdoc/></param>
/// <param name="AutoReload"><inheritdoc/></param>
/// <param name="AbsoluteExpiration"><inheritdoc/></param>
/// <param name="SlidingExpiration"><inheritdoc/></param>
/// <param name="Priority"><inheritdoc/></param>
/// <param name="IgnoreThisParameter"></param>
public sealed record BuildTimeCacheItemConfiguration(
    string? ProfileName = null,
    bool? AutoReload = null,
    TimeSpan? AbsoluteExpiration = null,
    TimeSpan? SlidingExpiration = null,
    CacheItemPriority? Priority = null,
    bool? IgnoreThisParameter = null )
    : IBuildTimeCacheItemConfiguration;
