// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[PublicAPI] // TODO: [Porting] Does CacheItemConfiguration need to be public? Might be a serialization artefact. Regardless, review visibility of setters.
[PSerializable]
public sealed class CacheItemConfiguration : ICacheItemConfiguration
{
    /// <inheritdoc />
    public bool? IsEnabled { get; set; }

    /// <inheritdoc />
    public string? ProfileName { get; set; }

    /// <inheritdoc />
    public bool? AutoReload { get; set; }

    /// <inheritdoc />
    public TimeSpan? AbsoluteExpiration { get; set; }

    /// <inheritdoc />
    public TimeSpan? SlidingExpiration { get; set; }

    /// <inheritdoc />
    public CacheItemPriority? Priority { get; set; }

    /// <inheritdoc />
    public bool? IgnoreThisParameter { get; set; }

    internal void ApplyFallback( ICacheItemConfiguration fallback )
    {
        this.AutoReload ??= fallback.AutoReload;
        this.AbsoluteExpiration ??= fallback.AbsoluteExpiration;
        this.SlidingExpiration ??= fallback.SlidingExpiration;
        this.Priority ??= fallback.Priority;
        this.ProfileName ??= fallback.ProfileName;
        this.IsEnabled ??= fallback.IsEnabled;
        this.IgnoreThisParameter ??= fallback.IgnoreThisParameter;
    }

    internal CacheItemConfiguration Clone() => (CacheItemConfiguration) this.MemberwiseClone();
}