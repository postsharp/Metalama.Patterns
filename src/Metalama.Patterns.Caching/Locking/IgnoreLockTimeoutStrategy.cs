// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Locking;

/// <summary>
/// An implementation of <see cref="IAcquireLockTimeoutStrategy"/> that ignores the lock
/// and proceeds with the execution of the method.
/// </summary>
public sealed class IgnoreLockTimeoutStrategy : IAcquireLockTimeoutStrategy
{
    public void OnTimeout( string key ) { }
}