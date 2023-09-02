// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[RunTimeOrCompileTime]
[PublicAPI]
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

    public CacheItemConfiguration() { }

    protected CacheItemConfiguration( CacheItemConfiguration overrideValue, ICacheItemConfiguration fallbackValue )
    {
        this.AutoReload = overrideValue.AutoReload ?? fallbackValue.AutoReload;
        this.AbsoluteExpiration = overrideValue.AbsoluteExpiration ?? fallbackValue.AbsoluteExpiration;
        this.SlidingExpiration = overrideValue.SlidingExpiration ?? fallbackValue.SlidingExpiration;
        this.Priority = overrideValue.Priority ?? fallbackValue.Priority;
        this.ProfileName = overrideValue.ProfileName ?? fallbackValue.ProfileName;
        this.IsEnabled = overrideValue.IsEnabled ?? fallbackValue.IsEnabled;
    }

    public CacheItemConfiguration ApplyFallbackValues( ICacheItemConfiguration fallback ) => new( this, fallback );
}