// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.RunTime;

namespace Metalama.Patterns.Caching.Implementation;

#if NETCOREAPP3_0_OR_GREATER

public static class AsyncEnumerableHelper
{
    /// <summary>
    /// If <paramref name="task"/> is already completed, returns the result of the task; otherwise, returns an
    /// instance of <see cref="IAsyncEnumerable{T}"/> which awaits the <paramref name="task"/> in
    /// <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> then creates and delegates to an enumerator from the result of the task.
    /// </summary>
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>( this Task<IAsyncEnumerable<T>> task )
    {
        if ( task == null )
        {
            throw new ArgumentNullException( nameof(task) );
        }

        if ( task.IsCompleted )
        {
            return task.Result;
        }

        return new EnumerableFromTask<T>( task );
    }

    /// <summary>
    /// If <paramref name="task"/> is already completed, returns the result of the task; otherwise, returns an
    /// instance of <see cref="IAsyncEnumerable{T}"/> which awaits the <paramref name="task"/> in
    /// <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> then creates and delegates to an enumerator from the result of the task.
    /// </summary>
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>( this ValueTask<IAsyncEnumerable<T>> task )
    {
        if ( task == null )
        {
            throw new ArgumentNullException( nameof(task) );
        }

        if ( task.IsCompleted )
        {
            return task.Result;
        }

        return new EnumerableFromValueTask<T>( task );
    }

    private sealed class EnumerableFromTask<T> : IAsyncEnumerable<T>
    {
        private readonly Task<IAsyncEnumerable<T>> _task;

        public EnumerableFromTask( Task<IAsyncEnumerable<T>> task )
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

    private sealed class EnumerableFromValueTask<T> : IAsyncEnumerable<T>
    {
        private readonly ValueTask<IAsyncEnumerable<T>> _task;

        public EnumerableFromValueTask( ValueTask<IAsyncEnumerable<T>> task )
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
            private readonly ValueTask<IAsyncEnumerable<T>> _task;
            private IAsyncEnumerator<T>? _enumerator;

            public Enumerator( ValueTask<IAsyncEnumerable<T>> task, CancellationToken cancellationToken )
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
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                this._enumerator ??= (await this._task).GetAsyncEnumerator( this._cancellationToken );

                return await this._enumerator.MoveNextAsync();
            }
        }
    }

    /// <summary>
    /// If <paramref name="task"/> is already completed, returns the result of the task; otherwise, returns an
    /// instance of <see cref="IAsyncEnumerator{T}"/> which awaits the <paramref name="task"/> in
    /// <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> then delegates to the enumerator from the result of the task.
    /// </summary>
    public static IAsyncEnumerator<T> AsAsyncEnumerator<T>( this Task<IAsyncEnumerator<T>> task )
    {
        if ( task == null )
        {
            throw new ArgumentNullException( nameof(task) );
        }

        if ( task.IsCompleted )
        {
            return task.Result;
        }

        return new EnumeratorFromTask<T>( task );
    }

    /// <summary>
    /// If <paramref name="task"/> is already completed, returns the result of the task; otherwise, returns an
    /// instance of <see cref="IAsyncEnumerator{T}"/> which awaits the <paramref name="task"/> in
    /// <see cref="IAsyncEnumerator{T}.MoveNextAsync"/> then delegates to the enumerator from the result of the task.
    /// </summary>
    public static IAsyncEnumerator<T> AsAsyncEnumerator<T>( this ValueTask<IAsyncEnumerator<T>> task )
    {
        if ( task == null )
        {
            throw new ArgumentNullException( nameof(task) );
        }

        if ( task.IsCompleted )
        {
            return task.Result;
        }

        return new EnumeratorFromValueTask<T>( task );
    }

    private sealed class EnumeratorFromTask<T> : IAsyncEnumerator<T>
    {
        private readonly Task<IAsyncEnumerator<T>> _task;
        private IAsyncEnumerator<T>? _enumerator;

        public EnumeratorFromTask( Task<IAsyncEnumerator<T>> task )
        {
            this._task = task;
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
            this._enumerator ??= await this._task;

            return await this._enumerator.MoveNextAsync();
        }
    }

    private sealed class EnumeratorFromValueTask<T> : IAsyncEnumerator<T>
    {
        private readonly ValueTask<IAsyncEnumerator<T>> _task;
        private IAsyncEnumerator<T>? _enumerator;

        public EnumeratorFromValueTask( ValueTask<IAsyncEnumerator<T>> task )
        {
            this._task = task;
        }

        public T Current => this._enumerator == null ? throw new InvalidOperationException() : this._enumerator.Current;

        public async ValueTask DisposeAsync()
        {
            if ( this._enumerator != null )
            {
                await this._enumerator.DisposeAsync();
            }
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            this._enumerator ??= await this._task;

            return await this._enumerator.MoveNextAsync();
        }
    }

    // TODO: !!! [Porting] Consider moving this to Metalama.Framework.RunTime/RunTimeAspectHelper, on which it is based:

    /// <summary>
    /// Evaluates an <see cref="IAsyncEnumerator{T}"/>, stores the result into an <see cref="AsyncEnumerableList{T}"/> and returns the list.
    ///  The intended side effect of this method is to completely evaluate the input enumerator.
    /// </summary>
    /// <param name="enumerator">An enumerator.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An <see cref="AsyncEnumerableList{T}"/> made from the items of <paramref name="enumerator"/>.</returns>
    public static async ValueTask<AsyncEnumerableList<T>> BufferToListAsync<T>(
        this IAsyncEnumerator<T> enumerator,
        CancellationToken cancellationToken
            = default )
    {
        // TODO: [Porting] Reinstate this optimization. Requires framework change.
#if false
        // Reinstate the following as the second line of <summary>:
        // If the enumerable is already an enumerator of a <see cref="AsyncEnumerableList{T}"/>, returns the input enumerator.
        // And use this for <returns>:
        // <returns>An <see cref="AsyncEnumerableList{T}"/> made from the items of <paramref name="enumerator"/>, or the parent <see cref="AsyncEnumerableList{T}"/> object
        // if <paramref name="enumerator"/> object itself it is already an <see cref="AsyncEnumerableList{T}"/> enumerator.</returns>

        if ( enumerator is AsyncEnumerableList<T>.AsyncEnumerator typedEnumerator )
        {
            return typedEnumerator.Parent; // TODO: Would need to add Parent property to AsyncEnumerableList<T>.AsyncEnumerator
        }
        else
#endif
        {
            var list = new AsyncEnumerableList<T>();

            try
            {
                while ( await enumerator.MoveNextAsync() )
                {
                    list.Add( enumerator.Current );

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return list;
        }
    }
}

#endif