// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Caching.Locking
{
    /// <summary>
    /// Provides instances of named locks. 
    /// </summary>
    /// <remarks>
    /// Two implementations are provided: <see cref="NullLockManager"/> and <see cref="LocalLockManager"/>.
    /// To change the lock manager, set the <see cref="CachingProfile.LockManager"/> property.
    /// </remarks>
    public interface ILockManager
    {
        /// <summary>
        /// Gets a handle to a named lock. This method must return immediately. Waiting, if any, must be done in the <see cref="ILockHandle.Acquire"/> method.
        /// </summary>
        /// <param name="key">The name of the lock.</param>
        /// <returns>A handle to the lock named <paramref name="key"/>.</returns>
        ILockHandle GetLock( string key );
    }
}
