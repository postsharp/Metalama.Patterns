// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Backends;

/// <summary>
/// The object stored in the remote class.
/// </summary>
[Serializable]
[DataContract]
internal sealed class LayeredCacheValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredCacheValue"/> class.
    /// </summary>
    /// <param name="item">The original <see cref="CacheItem"/>.</param>
    public LayeredCacheValue( CacheItem item )
    {
        this.Value = item.Value;
        this.SlidingExpiration = item.Configuration?.SlidingExpiration;
        this.Priority = item.Configuration?.Priority;

        if ( item.Configuration?.AbsoluteExpiration != null )
        {
            this.AbsoluteExpiration = DateTime.UtcNow + item.Configuration?.AbsoluteExpiration.Value;
        }
    }

    /// <summary>
    /// Gets or sets the timestamp of the cache item.
    /// </summary>
    [DataMember]
    public long Timestamp { get; set; } = LayeredCachingBackendEnhancer.GetTimestamp();

    /// <summary>
    /// Gets or sets the cached value.
    /// </summary>
    [DataMember]
    public object? Value { get; set; }

    /// <summary>
    /// Gets or sets the absolute expiration of the cache item.
    /// </summary>
    [DataMember]
    public DateTime? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration of the cache item.
    /// </summary>
    [DataMember]
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// Gets or sets the cache item priority.
    /// </summary>
    [DataMember]
    public CacheItemPriority? Priority { get; set; }

    internal CacheItemConfiguration ToCacheItemConfiguration()
    {
        var configuration = new CacheItemConfiguration
        {
            Priority = this.Priority,
            SlidingExpiration = this.SlidingExpiration,
            AbsoluteExpiration = this.AbsoluteExpiration != null ? DateTime.UtcNow - this.AbsoluteExpiration.Value : null
        };

        return configuration;
    }
}