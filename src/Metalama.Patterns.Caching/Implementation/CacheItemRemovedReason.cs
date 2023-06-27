// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Enumerates the reasons why an item can be removed from the cache.
    /// </summary>
    public enum CacheItemRemovedReason
    {
        /// <summary>
        /// Directly removed from the cache, by calling the <see cref="CachingBackend.RemoveItem(string)"/>, or an invalidation method that invalidates
        /// a cached method directly (not indirectly through dependencies).
        /// </summary>
        Removed,

        /// <summary>
        /// Removed because of <see cref="CacheItemConfiguration.AbsoluteExpiration"/> or <see cref="CacheItemConfiguration.SlidingExpiration"/>.
        /// </summary>
        Expired,

        /// <summary>
        /// Evicted to make space for newer cache items.
        /// </summary>
        Evicted,

        /// <summary>
        /// Indirectly invalidated (through dependencies).
        /// </summary>
        Invalidated,

        /// <summary>
        /// Other (or any reason that cannot be determined).
        /// </summary>
        Other
    }

}
