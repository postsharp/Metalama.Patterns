// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace PostSharp.Patterns.Caching.Locking
{
    /// <summary>
    /// Allows to acquire and release a named lock returned by <see cref="ILockManager.GetLock"/>.
    /// </summary>
    public interface ILockHandle : IDisposable
    {
        /// <summary>
        /// Synchronously acquires the lock bound to the current handle.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns><c>true</c> if the lock was acquired, or <c>false</c> if the operation has timed out before the lock could be acquired.</returns>
        bool Acquire( TimeSpan timeout, CancellationToken cancellationToken );

        /// <summary>
        /// Asynchronously acquires the lock bound to the current handle.
        /// </summary>
        /// <param name="timeout">Timeout.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns><c>true</c> if the lock was acquired, or <c>false</c> if the operation has timed out before the lock could be acquired.</returns>
        Task<bool> AcquireAsync( TimeSpan timeout, CancellationToken cancellationToken );

        /// <summary>
        /// Synchronously releases the lock bound to the current handle.
        /// </summary>
        void Release();

        /// <summary>
        /// Asynchronously releases the lock bound to the current handle.
        /// </summary>
        Task ReleaseAsync();

    }
}
