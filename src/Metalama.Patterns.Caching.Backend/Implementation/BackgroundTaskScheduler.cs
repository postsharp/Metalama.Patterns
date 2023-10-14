// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Messages;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

#if DEBUG
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#endif

namespace Metalama.Patterns.Caching.Implementation;

[PublicAPI]
public sealed class BackgroundTaskScheduler : IDisposable, IAsyncDisposable
{
    private static volatile int _allBackgroundTaskExceptions;

    private readonly FlashtraceSource _logger;
    private readonly AwaitableEvent _backgroundTasksFinishedEvent = new( EventResetMode.ManualReset, true );
    private readonly CancellationTokenSource _disposeCancellationTokenSource = new();
    private readonly bool _sequential;
    private readonly object _sync = new();
    private readonly ICachingExceptionObserver? _exceptionObserver;

#if DEBUG
    private readonly ConcurrentDictionary<int, PendingTask> _pendingBackgroundTasks = new();
#endif

    private volatile int _backgroundTaskExceptions;
    private volatile int _backgroundTaskCount;
    private bool _backgroundTasksForbidden;

#if DEBUG
    private volatile int _nextTaskId;
#endif

    // A CancellationToken triggered when the Dispose method is cancelled, i.e. when the caller no longer wants to wait.
    private CancellationToken DisposeCancellationToken => this._disposeCancellationTokenSource.Token;

    public int BackgroundTaskExceptions => this._backgroundTaskExceptions;

    private volatile Task _lastTask = Task.CompletedTask;

    public BackgroundTaskScheduler( IServiceProvider? serviceProvider, bool sequential = false )
    {
        this._sequential = sequential;
        this._exceptionObserver = serviceProvider?.GetService<ICachingExceptionObserver>();
        this._logger = serviceProvider.GetFlashtraceSource( this.GetType(), FlashtraceRole.Caching );
    }

    /// <summary>
    /// Forbids the use of the <see cref="EnqueueBackgroundTask(System.Func{System.Threading.CancellationToken,System.Threading.Tasks.ValueTask})"/> method. This method is used for debugging purposes only.
    /// </summary>
    public void StopAcceptingBackgroundTasks() => this._backgroundTasksForbidden = true;

    public void EnqueueBackgroundTask( Func<CancellationToken, ValueTask> task ) => this.EnqueueBackgroundTask( ct => task( ct ).AsTask() );

    /// <summary>
    /// Enqueues a background task.
    /// </summary>
    /// <param name="task">A function creating a <see cref="Task"/>.</param>
    public void EnqueueBackgroundTask( Func<CancellationToken, Task> task )
    {
        if ( this._backgroundTasksForbidden )
        {
            throw new InvalidOperationException(
                string.Format( CultureInfo.InvariantCulture, "The current {0} can no longer accept background tasks.", nameof(CachingBackend) ) );
        }

        lock ( this._backgroundTasksFinishedEvent )
        {
            // ReSharper disable once NonAtomicCompoundOperator : [Porting] Won't fix, too risky.
            this._backgroundTaskCount++;

            if ( this._backgroundTaskCount == 1 )
            {
                this._backgroundTasksFinishedEvent.Reset();
            }
        }

#if DEBUG
        var taskId = Interlocked.Increment( ref this._nextTaskId );
        var pendingTask = new PendingTask { StackTrace = new StackTrace(), Id = taskId };

        this._pendingBackgroundTasks.TryAdd( taskId, pendingTask );
#endif

        if ( this._sequential )
        {
            lock ( this._sync )
            {
                var previousTask = this._lastTask;

// Suppress warnings to allow adding extra parameter if DEBUG
#pragma warning disable SA1113
#pragma warning disable SA1115
#pragma warning disable SA1111

                var createdTask = Task.Run(
                    () => this.RunTask(
                        () => task( this.DisposeCancellationToken ),
                        previousTask
#if DEBUG
                       ,
                        pendingTask
#endif
                    ),
                    this.DisposeCancellationToken );

                this._lastTask = createdTask;
            }
        }
        else
        {
            Task.Run(
                () => this.RunTask(
                    () => task( this.DisposeCancellationToken ),
                    null
#if DEBUG
                   ,
                    pendingTask
#endif
                ),
                this.DisposeCancellationToken );
        }
    }

    private async Task RunTask(
        Func<Task> task,
        Task? lastTask
#if DEBUG
       ,
        PendingTask pendingTask
#endif
    )
#pragma warning restore SA1115
#pragma warning restore SA1113
#pragma warning restore SA1111
    {
        if ( lastTask != null )
        {
            try
            {
                await lastTask;
            }
            catch ( Exception e )
            {
                if ( this.OnBackgroundTaskException( e ) )
                {
                    throw;
                }
            }
        }

        try
        {
            var t = task();
#if DEBUG
            pendingTask.Task = t;
#endif
            await t;
        }
        catch ( Exception e )
        {
            if ( this.OnBackgroundTaskException( e ) )
            {
                throw;
            }

#if DEBUG
            this._logger.Debug.IfEnabled?.Write(
                FormattedMessageBuilder.Formatted( "Stack trace that created the failing task: {StackTrace}", pendingTask.StackTrace ) );
#endif
        }
        finally
        {
            lock ( this._backgroundTasksFinishedEvent )
            {
                // ReSharper disable once NonAtomicCompoundOperator : [Porting] Won't fix, too risky.
                this._backgroundTaskCount--;

                if ( this._backgroundTaskCount == 0 )
                {
                    this._backgroundTasksFinishedEvent.Set();
                }
            }

#if DEBUG
            this._pendingBackgroundTasks.TryRemove( pendingTask.Id, out _ );
#endif
        }
    }

    private bool OnBackgroundTaskException( Exception e )
    {
        if ( this._exceptionObserver.OnException( e, true ) )
        {
            return true;
        }

        this._logger.Error.Write( FormattedMessageBuilder.Formatted( "{ExceptionType} when executing a background task.", e.GetType().Name ), e );
        Interlocked.Increment( ref this._backgroundTaskExceptions );
        Interlocked.Increment( ref _allBackgroundTaskExceptions );

        return false;
    }

    /// <summary>
    /// Returns a <see cref="Task"/> that completes when all enqueued background tasks complete.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="DisposeCancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that completes when all enqueued background tasks complete.</returns>
    public async Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // AwaitableEvent does not support CancellationToken.
        await this._backgroundTasksFinishedEvent.WaitAsync( CancellationToken.None );
    }

    public static int AllBackgroundTaskExceptions => _allBackgroundTaskExceptions;

    public void Dispose() => this.Dispose( default );

    public void Dispose( CancellationToken cancellationToken )
    {
        using ( cancellationToken.Register( this._disposeCancellationTokenSource.Cancel ) )
        {
            this.StopAcceptingBackgroundTasks();
            this._backgroundTasksFinishedEvent.Wait( cancellationToken );
        }
    }

    public Task DisposeAsync( CancellationToken cancellationToken = default )
    {
        using ( cancellationToken.Register( this._disposeCancellationTokenSource.Cancel ) )
        {
            this.StopAcceptingBackgroundTasks();

            return this.WhenBackgroundTasksCompleted( cancellationToken );
        }
    }

    // ReSharper disable once MethodSupportsCancellation
    ValueTask IAsyncDisposable.DisposeAsync() => new( this.DisposeAsync() );

#if DEBUG
    [SuppressMessage( "StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Class is for diagnostic purposes." )]
    private class PendingTask
    {
        public StackTrace? StackTrace;

        // ReSharper disable once NotAccessedField.Local
        public Task? Task;
        public int Id;
    }
#endif
}