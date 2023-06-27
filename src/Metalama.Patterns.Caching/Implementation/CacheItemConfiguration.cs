// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[PSerializable]
public sealed class CacheItemConfiguration : ICacheItemConfiguration
{
    /// <inheritdoc />
    public bool? IsEnabled { get; set; }

    /// <inheritdoc />
    public string ProfileName { get; set; }

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
        if ( this.AutoReload == null )
        {
            this.AutoReload = fallback.AutoReload;
        }

        if ( this.AbsoluteExpiration == null )
        {
            this.AbsoluteExpiration = fallback.AbsoluteExpiration;
        }

        if ( this.SlidingExpiration == null )
        {
            this.SlidingExpiration = fallback.SlidingExpiration;
        }

        if ( this.Priority == null )
        {
            this.Priority = fallback.Priority;
        }

        if ( this.ProfileName == null )
        {
            this.ProfileName = fallback.ProfileName;
        }

        if ( this.IsEnabled == null )
        {
            this.IsEnabled = fallback.IsEnabled;
        }

        if ( this.IgnoreThisParameter == null )
        {
            this.IgnoreThisParameter = fallback.IgnoreThisParameter;
        }
    }

    internal CacheItemConfiguration Clone() => (CacheItemConfiguration) this.MemberwiseClone();
}