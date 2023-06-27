// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Locking;

/// <summary>
/// An implementation of <see cref="ILockManager"/> which does not acquire any lock.
/// </summary>
public class NullLockManager : ILockManager
{
    /// <inheritdoc />
    public ILockHandle GetLock( string key )
    {
        return LockHandle.Instance;
    }

    private class LockHandle : ILockHandle
    {
        public static readonly LockHandle Instance = new();
        private static readonly Task<bool> doneTask = Task.FromResult( true );

        public Task ReleaseAsync()
        {
            return doneTask;
        }

        public bool Acquire( TimeSpan timeout, CancellationToken cancellationToken )
        {
            return true;
        }

        public Task<bool> AcquireAsync( TimeSpan timeout, CancellationToken cancellationToken )
        {
            return doneTask;
        }

        public void Release() { }

        public void Dispose() { }
    }
}