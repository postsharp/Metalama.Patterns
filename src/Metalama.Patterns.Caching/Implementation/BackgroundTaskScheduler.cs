// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] Used by Redis backend. Making public for now.
[ExplicitCrossPackageInternal]
public sealed class BackgroundTaskScheduler
{
    private volatile int backgroundTaskExceptions;
    private static volatile int allBackgroundTaskExceptions;
    private volatile int backgroundTaskCount;
    private readonly AwaitableEvent backgroundTasksFinishedEvent = new( EventResetMode.ManualReset, true );
    private bool backgroundTasksForbidden;
    private static readonly LogSource logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource();
    private readonly bool sequential;

#if DEBUG
    private volatile int nextTaskId;
    private readonly ConcurrentDictionary<int, PendingTask> pendingBackgroundTasks = new();
#endif

    public int BackgroundTaskExceptions => this.backgroundTaskExceptions;

    private volatile Task lastTask = Task.CompletedTask;

    private readonly object sync = new();

    public BackgroundTaskScheduler() : this( false ) { }

    public BackgroundTaskScheduler( bool sequential )
    {
        this.sequential = sequential;
    }

    /// <summary>
    /// Forbids the use of the <see cref="EnqueueBackgroundTask(Func{Task})"/> method. This method is used for debugging purposes only.
    /// </summary>
    public void StopAcceptingBackgroundTasks()
    {
        this.backgroundTasksForbidden = true;
    }

    /// <summary>
    /// Enqueues a background task.
    /// </summary>
    /// <param name="task">A function creating a <see cref="Task"/>.</param>
    public void EnqueueBackgroundTask( Func<Task> task )
    {
        if ( this.backgroundTasksForbidden )
        {
            throw new InvalidOperationException(
                string.Format( CultureInfo.InvariantCulture, "The current {0} can no longer accept background tasks.", nameof(CachingBackend) ) );
        }

        lock ( this.backgroundTasksFinishedEvent )
        {
            this.backgroundTaskCount++;

            if ( this.backgroundTaskCount == 1 )
            {
                this.backgroundTasksFinishedEvent.Reset();
            }
        }

#if DEBUG
        var taskId = Interlocked.Increment( ref this.nextTaskId );
        var pendingTask = new PendingTask { StackTrace = new StackTrace(), Id = taskId };

        this.pendingBackgroundTasks.TryAdd( taskId, pendingTask );
#endif

        if ( this.sequential )
        {
            lock ( this.sync )
            {
                var previousTask = this.lastTask;

                var createdTask = Task.Run(
                    () => this.RunTask(
                        task,
                        previousTask
#if DEBUG
                       ,
                        pendingTask
#endif
                    ) );

                this.lastTask = createdTask;
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
        Task lastTask
#if DEBUG
       ,
        PendingTask pendingTask
#endif
    )
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
            logger.Error.Write( FormattedMessageBuilder.Formatted( "{ExceptionType} when executing a background task.", e.GetType().Name ), e );

#if DEBUG
            logger.Debug.EnabledOrNull?.Write(
                FormattedMessageBuilder.Formatted( "Stack trace that created the failing task: {StackTrace}", pendingTask.StackTrace ) );
#endif

            Interlocked.Increment( ref this.backgroundTaskExceptions );
            Interlocked.Increment( ref allBackgroundTaskExceptions );
        }
        finally
        {
            lock ( this.backgroundTasksFinishedEvent )
            {
                this.backgroundTaskCount--;

                if ( this.backgroundTaskCount == 0 )
                {
                    this.backgroundTasksFinishedEvent.Set();
                }
            }

#if DEBUG
            PendingTask removedTask;
            this.pendingBackgroundTasks.TryRemove( pendingTask.Id, out removedTask );
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
        await this.backgroundTasksFinishedEvent.WaitAsync( CancellationToken.None );
    }

    internal static int AllBackgroundTaskExceptions
    {
        get { return allBackgroundTaskExceptions; }
    }

    public void Dispose()
    {
        this.StopAcceptingBackgroundTasks();
        this.backgroundTasksFinishedEvent.Wait();
    }

    public Task DisposeAsync( CancellationToken cancellationToken )
    {
        this.StopAcceptingBackgroundTasks();

        return this.WhenBackgroundTasksCompleted( cancellationToken );
    }

#if DEBUG
    private class PendingTask
    {
        public StackTrace StackTrace;
        public Task Task;
        public int Id;
    }
#endif
}