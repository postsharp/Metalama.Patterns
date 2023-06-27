// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation
{
    /// <summary>
    /// Arguments of the <see cref="CachingBackend.ItemRemoved"/> event.
    /// </summary>
    public sealed class CacheItemRemovedEventArgs : EventArgs
    {
        internal CacheItemRemovedEventArgs( string key, CacheItemRemovedReason removedReason, Guid sourceId )
        {
            if ( key == null )
            {
                throw new ArgumentNullException( nameof(key) );
            }

            this.Key = key;
            this.RemovedReason = removedReason;
            this.SourceId = sourceId;
        }

        /// <summary>
        /// Gets the key of the removed cached item.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the reason of the removal.
        /// </summary>
        public CacheItemRemovedReason RemovedReason { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> of the <see cref="CachingBackend"/> that caused the removal,
        /// or <see cref="Guid.Empty"/> if it cannot be determined or does not apply.
        /// </summary>
        public Guid SourceId { get; }
    }
}