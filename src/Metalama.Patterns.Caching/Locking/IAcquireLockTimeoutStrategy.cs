// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Locking;

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