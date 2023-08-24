// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Messages;
using JetBrains.Annotations;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] BackgroundTaskScheduler was [ExplicitCrossPackageInternal]. Used by Redis backend so making public for now.
[PublicAPI]
public sealed class BackgroundTaskScheduler : IDisposable, IAsyncDisposable
{
    private static readonly LogSource _logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource();

    private readonly AwaitableEvent _backgroundTasksFinishedEvent = new( EventResetMode.ManualReset, true );
    private readonly bool _sequential;
    private readonly object _sync = new();

#if DEBUG
    private readonly ConcurrentDictionary<int, PendingTask> _pendingBackgroundTasks = new();
#endif

    private volatile int _backgroundTaskExceptions;
    private static volatile int _allBackgroundTaskExceptions;
    private volatile int _backgroundTaskCount;
    private bool _backgroundTasksForbidden;

#if DEBUG
    private volatile int _nextTaskId;
#endif

    public int BackgroundTaskExceptions => this._backgroundTaskExceptions;

    private volatile Task _lastTask = Task.CompletedTask;

    public BackgroundTaskScheduler() : this( false ) { }

    public BackgroundTaskScheduler( bool sequential )
    {
        this._sequential = sequential;
    }

    /// <summary>
    /// Forbids the use of the <see cref="EnqueueBackgroundTask(Func{Task})"/> method. This method is used for debugging purposes only.
    /// </summary>
    public void StopAcceptingBackgroundTasks() => this._backgroundTasksForbidden = true;

    public void EnqueueBackgroundTask( Func<ValueTask> task ) => this.EnqueueBackgroundTask( () => task().AsTask() );

    /// <summary>
    /// Enqueues a background task.
    /// </summary>
    /// <param name="task">A function creating a <see cref="Task"/>.</param>
    public void EnqueueBackgroundTask( Func<Task> task )
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
                        task,
                        previousTask
#if DEBUG
                       ,
                        pendingTask
#endif
                    ) );

                this._lastTask = createdTask;
            }
        }
        else
        {
            Task.Run(
                () => this.RunTask(
                    task,
                    null
#if DEBUG
                   ,
                    pendingTask
#endif
                ) );
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
            await lastTask;
        }

        try
        {
            var t = task();
#if DEBUG
            pendingTask.Task = t;
#endif
            await t;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch ( Exception e )
#pragma warning restore CA1031 // Do not catch general exception types
        {
            _logger.Error.Write( FormattedMessageBuilder.Formatted( "{ExceptionType} when executing a background task.", e.GetType().Name ), e );

#if DEBUG
            _logger.Debug.EnabledOrNull?.Write(
                FormattedMessageBuilder.Formatted( "Stack trace that created the failing task: {StackTrace}", pendingTask.StackTrace ) );
#endif

            Interlocked.Increment( ref this._backgroundTaskExceptions );
            Interlocked.Increment( ref _allBackgroundTaskExceptions );
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

    /// <summary>
    /// Returns a <see cref="Task"/> that completes when all enqueued background tasks complete.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that completes when all enqueued background tasks complete.</returns>
    public async Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        // AwaitableEvent does not support CancellationToken.
        await this._backgroundTasksFinishedEvent.WaitAsync( CancellationToken.None );
    }

    internal static int AllBackgroundTaskExceptions => _allBackgroundTaskExceptions;

    public void Dispose()
    {
        this.StopAcceptingBackgroundTasks();
        this._backgroundTasksFinishedEvent.Wait();
    }

    public Task DisposeAsync( CancellationToken cancellationToken = default )
    {
        this.StopAcceptingBackgroundTasks();

        return this.WhenBackgroundTasksCompleted( cancellationToken );
    }

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