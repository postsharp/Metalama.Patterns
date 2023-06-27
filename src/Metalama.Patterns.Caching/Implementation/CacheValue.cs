// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Immutable;

namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Return value of the <see cref="CachingBackend.GetItem(string, bool)"/> method, i.e. represents an item retrieved from the cache (items being stored in the cache are represented by the <see cref="CacheItem"/> class).
    /// </summary>
    public class CacheValue
    {
        /// <summary>
        /// Gets the cached value.
        /// </summary>
        public object Value { get; private set; }

        // We need to store dependencies to be able to automatically add dependencies to the calling context
        // when we're accessing a cached method calling another cached method.

        /// <summary>
        /// Gets the list of dependencies of the cache. 
        /// </summary>
        public IImmutableList<string> Dependencies { get; }


        /// <summary>
        /// Initializes a new <see cref="CacheValue"/>.
        /// </summary>
        /// <param name="value">The cached value.</param>
        /// <param name="dependencies">The list of dependencies (or <c>null</c> if there are no dependencies or dependencies were not requested).</param>
        public CacheValue( object value, IImmutableList<string> dependencies = null )
        {
            this.Value = value;
            this.Dependencies = dependencies;
        }

        /// <summary>
        /// Returns a new <see cref="CacheValue"/> with different <see cref="Value"/> but identical <see cref="Dependencies"/>.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>A new <see cref="CacheValue"/> with different <see cref="Value"/> but identical <see cref="Dependencies"/>.</returns>
        public CacheValue WithValue( object value )
        {
            CacheValue clone = (CacheValue) this.MemberwiseClone();
            clone.Value = value;
            return clone;
        }
    }
}