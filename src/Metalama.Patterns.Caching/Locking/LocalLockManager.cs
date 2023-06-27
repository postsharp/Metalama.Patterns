// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Concurrent;

namespace Metalama.Patterns.Caching.Locking
{
    /// <summary>
    /// An implementation of <see cref="ILockManager"/> in which every instance of the <see cref="LocalLockManager"/>
    /// has its own set of named locks that are not shared in any way with other instances. The <see cref="LocalLockManager"/> can
    /// be used to synchronize the execution of methods in the current process and <see cref="AppDomain"/>.
    /// </summary>
    public sealed class LocalLockManager : ILockManager
    {
        private readonly ConcurrentDictionary<string, Lock> locks = new( StringComparer.OrdinalIgnoreCase );

        /// <inheritdoc />
        public ILockHandle GetLock( string key )
        {
            var @lock = this.locks.AddOrUpdate( key, k => new Lock( this, k ), ( k, l ) => l.AddReference() );
#if DEBUG
            if ( @lock.References <= 0 )
            {
                throw new MetalamaPatternsCachingAssertionFailedException();
            }
#endif
            return new LockHandle( @lock );
        }

        private class LockHandle : ILockHandle
        {
            private static readonly Task doneTask = Task.FromResult( true );

            private readonly Lock @lock;
            private bool disposed;
            private bool acquired;

            public LockHandle( Lock @lock )
            {
                this.@lock = @lock;
            }

            public bool Acquire( TimeSpan timeout, CancellationToken cancellationToken )
            {
                if ( this.acquired )
                {
                    throw new InvalidOperationException();
                }

                this.acquired = this.@lock.Wait( timeout, cancellationToken );

                return this.acquired;
            }

            public async Task<bool> AcquireAsync( TimeSpan timeout, CancellationToken cancellationToken )
            {
                if ( this.acquired )
                {
                    throw new InvalidOperationException();
                }

                this.acquired = await this.@lock.WaitAsync( timeout, cancellationToken );

                return this.acquired;
            }

            public void Release()
            {
                if ( this.acquired )
                {
                    this.@lock.Release();
                    this.acquired = false;
                }
            }

            public Task ReleaseAsync()
            {
                this.Release();

                return doneTask;
            }

            public void Dispose()
            {
                if ( this.acquired )
                {
                    throw new InvalidOperationException( "The lock is still acquired." );
                }

                if ( !this.disposed )
                {
                    this.@lock.RemoveReference();
                    this.disposed = true;

#if DEBUG
                    GC.SuppressFinalize( this );
#endif
                }
            }

#if DEBUG
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
#pragma warning disable CA1821 // Remove empty Finalizers
            ~LockHandle()

            {
                throw new MetalamaPatternsCachingAssertionFailedException( "The Dispose method has not been invoked." );
            }
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning restore CA1821 // Remove empty Finalizers
#endif
        }

        private class Lock : SemaphoreSlim
        {
            private readonly LocalLockManager parent;
            private readonly string key;

            public int References { get; private set; } = 1;

            // This locks prevents a data race between AddReference and RemoveReference. 
            // It enforces the following invariant: this.References == 0 and 'this' is not present in in this.parent.lock.
#pragma warning disable IDE0044 // Add readonly modifier (this is a mutable struct type)
            private SpinLock spinLock;
#pragma warning restore IDE0044 // Add readonly modifier

            public Lock( LocalLockManager parent, string key ) : base( 1 )
            {
                this.parent = parent;
                this.key = key;
            }

            public Lock AddReference()
            {
                var lockTaken = false;

                try
                {
                    this.spinLock.Enter( ref lockTaken );
                    this.References++;
                }
                finally
                {
                    if ( lockTaken )
                    {
                        this.spinLock.Exit( true );
                    }
                }

                return this;
            }

            public void RemoveReference()
            {
                var lockTaken = false;

                try
                {
                    this.spinLock.Enter( ref lockTaken );

                    this.References--;

                    if ( this.References == 0 )
                    {
                        Lock removedLock;
#pragma warning disable CA2000 // Dispose objects before losing scope
                        if ( !this.parent.locks.TryRemove( this.key, out removedLock ) || removedLock != this )
#pragma warning restore CA2000 // Dispose objects before losing scope
                        {
                            throw new MetalamaPatternsCachingAssertionFailedException( "Data race." );
                        }
                    }
                }
                finally
                {
                    if ( lockTaken )
                    {
                        this.spinLock.Exit( true );
                    }
                }
            }
        }
    }
}