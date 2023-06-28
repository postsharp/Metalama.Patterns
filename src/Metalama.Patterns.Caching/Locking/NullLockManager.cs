// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching.Locking;

/// <summary>
/// An implementation of <see cref="ILockManager"/> which does not acquire any lock.
/// </summary>
[PublicAPI]
public class NullLockManager : ILockManager
{
    /// <inheritdoc />
    public ILockHandle GetLock( string key ) => LockHandle.Instance;

    private class LockHandle : ILockHandle
    {
        public static readonly LockHandle Instance = new();
        private static readonly Task<bool> _doneTask = Task.FromResult( true );

        public Task ReleaseAsync() => _doneTask;

        public bool Acquire( TimeSpan timeout, CancellationToken cancellationToken ) => true;

        public Task<bool> AcquireAsync( TimeSpan timeout, CancellationToken cancellationToken ) => _doneTask;

        public void Release() { }

        public void Dispose() { }
    }
}