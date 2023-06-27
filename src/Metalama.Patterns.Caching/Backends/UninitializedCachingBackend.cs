// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Globalization;

namespace Metalama.Patterns.Caching.Backends
{
    /// <summary>
    /// A <see cref="CachingBackend"/> that throws an exception when it's used. This is the active default backend until you see <see cref="CachingServices.DefaultBackend"/> to something else.
    /// </summary>
    public sealed class UninitializedCachingBackend : CachingBackend
    {
        private static void Throw()
        {
            throw new CachingException(string.Format(CultureInfo.InvariantCulture,
                "The caching back-end has not been initialized. Set the {0}.{1} property before accessing a cached method.",
                nameof(CachingServices),
                nameof(CachingServices.DefaultBackend)
                ));
        }

        /// <inheritdoc />
        protected override void ClearCore()
        {
            Throw();
        }

        /// <inheritdoc />
        protected override bool ContainsDependencyCore(string key)
        {
            Throw();
            return false;
        }

        /// <inheritdoc />
        protected override bool ContainsItemCore(string key)
        {
            Throw();
            return false;
        }

        /// <inheritdoc />
        protected override CacheValue GetItemCore(string key, bool includeDependencies)
        {
            Throw();
            return null;
        }

        /// <inheritdoc />
        protected override void InvalidateDependencyCore(string key)
        {
            Throw();
        }

        /// <inheritdoc />
        protected override void RemoveItemCore(string key)
        {
            Throw();
        }

        /// <inheritdoc />
        protected override void SetItemCore(string key, CacheItem item)
        {
            Throw();
        }
    }
}
