// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

#if NETCOREAPP3_0_OR_GREATER

public static class AsyncEnumerableHelper
{
    /// <summary>
    /// If <paramref name="task"/> is already completed, returns the result of the task; otherwise, returns an
    /// instance of <see cref="IAsyncEnumerable{T}"/> which awaits the <paramref name="task"/> in
    /// <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> then creates an enumerator from the result of the task.
    /// </summary>
    public static IAsyncEnumerable<T> GetAsyncEnumerable<T>( Task<IAsyncEnumerable<T>> task )
    {
        if ( task == null )
        {
            throw new ArgumentNullException( nameof( task ) );
        }

        if ( task.IsCompleted )
        {
            return task.Result;
        }

        return new Enumerable<T>( task );
    }

    private sealed class Enumerable<T> : IAsyncEnumerable<T>
    {
        private readonly Task<IAsyncEnumerable<T>> _task;

        public Enumerable( Task<IAsyncEnumerable<T>> task )
        {
            this._task = task;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator( CancellationToken cancellationToken = default )
        {
            return new Enumerator( this._task, cancellationToken );
        }

        private sealed class Enumerator : IAsyncEnumerator<T>
        {
            private readonly CancellationToken _cancellationToken;
            private readonly Task<IAsyncEnumerable<T>> _task;
            private IAsyncEnumerator<T>? _enumerator;

            public Enumerator( Task<IAsyncEnumerable<T>> task, CancellationToken cancellationToken )
            {
                this._task = task;
                this._cancellationToken = cancellationToken;
            }

            public T Current => this._enumerator == null ? throw new InvalidOperationException() : this._enumerator.Current;

            public async ValueTask DisposeAsync()
            {
                if ( this._enumerator != null )
                {
                    await this._enumerator.DisposeAsync();
                }

                this._task.Dispose();
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                this._enumerator ??= (await this._task).GetAsyncEnumerator( this._cancellationToken );
                return await this._enumerator.MoveNextAsync();
            }
        }
    }
}

#endif