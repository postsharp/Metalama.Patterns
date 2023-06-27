// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Backends
{
    /// <summary>
    /// Meant to be used by caching backends. It's a <see cref="CacheValue"/> with an extra object that functions as a lock. Do not use this if you're not
    /// implementing a <see cref="CachingBackend"/>.
    /// </summary>
    public class MemoryCacheValue : CacheValue
    {
        /// <summary>
        /// Initializes a new <see cref="MemoryCacheValue"/>.
        /// </summary>
        /// <param name="value">The cached value.</param>
        /// <param name="dependencies">The list of dependencies (or <c>null</c> if there are no dependencies).</param>
        /// <param name="sync">A mutex that's locked when we manipulate this item's dependencies.</param>
        public MemoryCacheValue( object value, IImmutableList<string> dependencies, object sync ) : base( value, dependencies )
        {
            this.Sync = sync;
        }

        /// <summary>
        /// Gets or sets the mutex that protects this cache key.
        /// </summary>
        public object Sync { get; set; }
        
    }
}
