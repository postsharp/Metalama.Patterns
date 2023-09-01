// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[RunTimeOrCompileTime]
public record CacheItemConfiguration : ICacheItemConfiguration
{
    /// <inheritdoc />
    public bool? IsEnabled { get; init; }

    /// <inheritdoc />
    public string? ProfileName { get; init; }

    /// <inheritdoc />
    public bool? AutoReload { get; init; }

    /// <inheritdoc />
    public TimeSpan? AbsoluteExpiration { get; init; }

    /// <inheritdoc />
    public TimeSpan? SlidingExpiration { get; init; }

    /// <inheritdoc />
    public CacheItemPriority? Priority { get; init; }

    public virtual CacheItemConfiguration ApplyFallbackValues( ICacheItemConfiguration fallback )
    {
        return new CacheItemConfiguration
        {
            AutoReload = this.AutoReload ?? fallback.AutoReload,
            AbsoluteExpiration = this.AbsoluteExpiration ?? fallback.AbsoluteExpiration,
            SlidingExpiration = this.SlidingExpiration ?? fallback.SlidingExpiration,
            Priority = this.Priority ?? fallback.Priority,
            ProfileName = this.ProfileName ?? fallback.ProfileName,
            IsEnabled = this.IsEnabled ?? fallback.IsEnabled
        };
    }
}