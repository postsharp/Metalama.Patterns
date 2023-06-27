// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Enumerates the priorities of a <see cref="CacheItem"/>.
    /// </summary>
    public enum CacheItemPriority
    {
        /// <summary>
        /// Default priority means "Default" for <c>System.Runtime.Caching.MemoryCache</c> and it means "Normal" for <c>Microsoft.Extensions.Caching.Memory.IMemoryCache</c>. 
        /// </summary>
        Default,

        /// <summary>
        /// Never removed, unless explicitly required through invalidation methods.
        /// </summary>
        NotRemovable,
        
        /// <summary>
        /// This cache item is removed earlier if the cache needs to be compacted. For <see cref="MemoryCachingBackend"/>, this is the same as <see cref="Default"/>.
        /// </summary>
        Low,

        /// <summary>
        /// This cache item is removed later if the cache needs to be compacted. For <see cref="MemoryCachingBackend"/>, this is the same as <see cref="Default"/>.
        /// </summary>
        High
    }
}