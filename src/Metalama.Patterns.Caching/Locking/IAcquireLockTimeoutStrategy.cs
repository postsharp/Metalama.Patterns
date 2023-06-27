// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Caching.Locking
{
    /// <summary>
    /// Represents the behavior when the caching aspect cannot acquire a lock because of a timeout.
    /// </summary>
    public interface IAcquireLockTimeoutStrategy
    {
        /// <summary>
        /// Method invoked when the caching aspect cannot acquire a lock because of a timeout. If this method returns without an exception,
        /// the caching aspect will invoke the cached method without acquiring the lock.
        /// </summary>
        /// <param name="key">Cache key (name of the lock).</param>
        void OnTimeout( string key );
    }
}
