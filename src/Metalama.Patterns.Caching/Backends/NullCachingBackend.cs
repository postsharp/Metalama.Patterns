// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends
{
    /// <summary>
    /// An implementation of <see cref="CachingBackend"/> that does not cache at all.
    /// </summary>
    public class NullCachingBackend : CachingBackend
    {
        /// <inheritdoc />
        protected override void SetItemCore( string key, CacheItem item )
        {
        }

        /// <inheritdoc />
        protected override bool ContainsItemCore( string key )
        {
            return false;
        }

        /// <inheritdoc />
        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            return null;
        }

        /// <inheritdoc />
        protected override void InvalidateDependencyCore( string key )
        {
        }

        /// <inheritdoc />
        protected override bool ContainsDependencyCore( string key )
        {
            return false;
        }

        /// <inheritdoc />
        protected override void ClearCore()
        {

        }

        /// <inheritdoc />
        protected override void RemoveItemCore( string key )
        {
        }
    }
}
