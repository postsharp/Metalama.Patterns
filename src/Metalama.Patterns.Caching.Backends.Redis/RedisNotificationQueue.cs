// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal class RedisNotificationQueue : ITestableCachingComponent
{
    private LogSource logger;

    public ISubscriber Subscriber { get; }

#pragma warning disable CA2213 // This is properly disposed
    private readonly BlockingCollection<RedisNotification> notificationQueue = new();
    private readonly ManualResetEventSlim notificationProcessingLock = new( true );
#pragma warning restore CA2213

    private readonly Thread notificationProcessingThread;
    private readonly TaskCompletionSource<bool> notificationProcessingThreadCompleted = new();
    private readonly ImmutableArray<RedisChannel> channels;
    private readonly Action<RedisNotification> handler;
    private readonly TimeSpan connectionTimeout;
    private volatile TaskCompletionSource<bool> queueEmptyTask = new();
    private static volatile int notificationProcessingThreads;
    private readonly string id;
    private const int connectDelay = 10;
    private volatile int status;
    private readonly TaskCompletionSource<bool> disposeTask = new();
    private readonly StackTrace allocationStackTrace = new();

    private RedisNotificationQueue(
        string id,
        IConnectionMultiplexer connection,
        ImmutableArray<RedisChannel> channels,
        Action<RedisNotification> handler,
        TimeSpan connectionTimeout )
    {
        this.id = id;
        this.channels = channels;
        this.handler = handler;
        this.connectionTimeout = connectionTimeout;
        this.Subscriber = connection.GetSubscriber();

        this.notificationProcessingThread = new Thread( ProcessNotificationQueue )
        {
            Name = nameof(DependenciesRedisCachingBackend) + ".ProcessNotificationQueue"
        };
    }

    [SuppressMessage( "Microsoft.Design", "CA1031" )]
    ~RedisNotificationQueue()
    {
        try
        {
            this.logger.Error.Write(
                Formatted(
                    "The resource {ObjectId} has not been properly disposed. Allocation stack trace:\n{AllocStackTrace}",
                    this.id,
                    this.allocationStackTrace.ToString() ) );

            try
            {
                this.Dispose( false );
            }
            catch ( Exception e )
            {
                this.logger.Error.Write( Formatted( "Exception when finalizing the RedisNotificationQueue." ), e );
            }
        }
        catch { }
    }

    private void Init()
    {
        this.logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.GetType() );

        var startedAt = DateTime.Now;

        foreach ( var channel in this.channels )
        {
            this.Subscriber.Subscribe( channel, this.OnNotificationReceived );

            while ( !this.Subscriber.IsConnected( channel ) )
            {
                this.logger.Debug.EnabledOrNull?.Write(
                    Formatted( "Not connected to {Channel}. Waiting {Delay} ms and retrying.", channel, connectDelay ) );

                Thread.Sleep( connectDelay );

                if ( DateTime.Now > startedAt + this.connectionTimeout )
                {
                    throw new TimeoutException( "Could not connect to a Redis server in the connection timeout." );
                }
            }
        }

        this.Subscriber.Ping();

        this.notificationProcessingThread.Start( new WeakReference<RedisNotificationQueue>( this ) );
    }

    private async Task<RedisNotificationQueue> InitAsync( CancellationToken cancellationToken )
    {
        this.logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.GetType() );

        foreach ( var channel in this.channels )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.Subscriber.SubscribeAsync( channel, this.OnNotificationReceived );

            while ( !this.Subscriber.IsConnected( channel ) )
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.logger.Debug.EnabledOrNull?.Write(
                    Formatted( "Not connected to {Channel}. Waiting {Delay} ms and retrying.", channel, connectDelay ) );

                await Task.Delay( connectDelay, cancellationToken );
            }
        }

        await this.Subscriber.PingAsync();

        this.notificationProcessingThread.Start( new WeakReference<RedisNotificationQueue>( this ) );

        return this;
    }

    [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
    public static RedisNotificationQueue Create(
        string id,
        IConnectionMultiplexer connection,
        ImmutableArray<RedisChannel> channels,
        Action<RedisNotification> handler,
        TimeSpan connectionTimeout )
    {
        var queue = new RedisNotificationQueue( id, connection, channels, handler, connectionTimeout );
        queue.Init();

        return queue;
    }

    [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
    public static Task<RedisNotificationQueue> CreateAsync(
        string id,
        IConnectionMultiplexer connection,
        ImmutableArray<RedisChannel> channels,
        Action<RedisNotification> handler,
        TimeSpan connectionTimeout,
        CancellationToken cancellationToken )
    {
        var queue = new RedisNotificationQueue( id, connection, channels, handler, connectionTimeout );

        return queue.InitAsync( cancellationToken );
    }

    private void OnNotificationReceived( RedisChannel channel, RedisValue value )
    {
        this.logger.Debug.EnabledOrNull?.Write( Formatted( "Received notification {Value} on channel {Channel}", value, channel ) );

        try
        {
            if ( !this.notificationQueue.IsAddingCompleted )
            {
                this.notificationQueue.Add( new RedisNotification { Channel = channel, Value = value } );

                return;
            }
        }
        catch ( ObjectDisposedException ) { }

        this.logger.Debug.EnabledOrNull?.Write( Formatted( "The notification was not queued because the queue was already disposed." ) );
    }

    [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
    private static void ProcessNotificationQueue( object state )
    {
        WeakReference<RedisNotificationQueue> queueRef = (WeakReference<RedisNotificationQueue>) state;

        RedisNotificationQueue queue;

        if ( !queueRef.TryGetTarget( out queue ) )
        {
            return;
        }

        Interlocked.Increment( ref notificationProcessingThreads );

        try
        {
            var logger = queue.logger;
            var id = queue.id;
            var blockingCollection = queue.notificationQueue;

            logger.Debug.EnabledOrNull?.Write(
                Formatted( "The {ThreadName} thread for object {ObjectId} has started.", nameof(ProcessNotificationQueue), id ) );

            // Don't hold a strong reference during enumeration.
            // ReSharper disable RedundantAssignment
            queue = null;

            foreach ( var notification in blockingCollection.GetConsumingEnumerable() )
            {
                if ( !queueRef.TryGetTarget( out queue ) )
                {
                    return;
                }

                if ( queue.notificationQueue.IsAddingCompleted )
                {
                    break;
                }

                try
                {
                    queue.notificationProcessingLock.Wait();

                    using ( var activity =
                           logger.Default.OpenActivity(
                               Formatted( "Processing notification {Value} received on channel {Channel}", notification.Value, notification.Channel ) ) )
                    {
                        try
                        {
                            queue.handler( notification );

                            activity.SetSuccess();
                        }
                        catch ( Exception e )
                        {
                            queue.BackgroundTaskExceptions++;

                            activity.SetException( e );
                        }
                    }

                    if ( queue.notificationQueue.Count == 0 )
                    {
                        Interlocked.Exchange( ref queue.queueEmptyTask, new TaskCompletionSource<bool>() ).SetResult( true );
                    }
                }
                catch ( Exception e )
                {
                    // If we have any unhandled exception here, don't want to crash the process, but instead
                    // we want to continue processing other queue items.
                    logger.Error.Write( Formatted( "Exception while processing the notification {Notification}.", notification ), e );
                    queue.BackgroundTaskExceptions++;
                }

                // Don't hold a strong reference during enumeration.
                queue = null;
            }

            logger.Debug.EnabledOrNull?.Write(
                Formatted( "The {ThreadName} thread for object {ObjectId} has completed.", nameof(ProcessNotificationQueue), id ) );
        }
        finally
        {
            if ( queueRef.TryGetTarget( out queue ) )
            {
                queue.notificationProcessingThreadCompleted.TrySetResult( true );
            }

            Interlocked.Decrement( ref notificationProcessingThreads );
        }
    }

    internal static int NotificationProcessingThreads => notificationProcessingThreads;

    internal void SuspendProcessing()
    {
        this.notificationProcessingLock.Reset();
    }

    internal void ResumeProcessing()
    {
        this.notificationProcessingLock.Set();
    }

    internal Task WhenQueueEmpty()
    {
        // This is probably not thread safe, but this should be good enough for unit testing.
        Thread.MemoryBarrier();

        if ( this.status >= (int) Status.DisposingPhase2 || this.notificationQueue.Count == 0 )
        {
            return Task.FromResult( true );
        }
        else
        {
            return this.queueEmptyTask.Task;
        }
    }

    private bool TryChangeStatus( Status expectedStatus, Status newStatus )
    {
#pragma warning disable 420
        return Interlocked.CompareExchange( ref this.status, (int) newStatus, (int) expectedStatus ) == (int) expectedStatus;
#pragma warning restore 420
    }

    private void ChangeStatus( Status newStatus )
    {
        this.status = (int) newStatus;
        Thread.MemoryBarrier();
    }

    public void Dispose()
    {
        this.Dispose( true );
    }

    private void Dispose( bool disposing )
    {
        using ( var activity = this.logger.Default.OpenActivity( Formatted( "Disposing" ) ) )
        {
            try
            {
                if ( !this.TryChangeStatus( Status.Default, Status.DisposingPhase1 ) )
                {
                    activity.SetOutcome( LogLevel.Debug, Formatted( "The method was already called." ) );
                    this.disposeTask.Task.Wait();

                    return;
                }

                try
                {
                    if ( disposing )
                    {
                        this.Subscriber.UnsubscribeAll();
                    }
                    else
                    {
                        // Don't attempt to touch the Subscriber during finalization.
                        // Redis resources are being finalized in the wrong order.
                    }

                    // Messages are not enqueued from this point.

                    this.notificationProcessingLock.Set();
                    this.notificationQueue.CompleteAdding();

                    // All messages are processed at this point.

                    this.notificationProcessingThreadCompleted.Task.Wait();

                    this.ChangeStatus( Status.DisposingPhase2 );

                    this.notificationQueue.Dispose();
                    this.notificationProcessingLock.Dispose();

                    this.ChangeStatus( Status.Disposed );

                    activity.SetSuccess();
                    this.disposeTask.SetResult( true );
                }
                catch ( Exception e )
                {
                    this.disposeTask.SetException( e );

                    throw;
                }
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }

        if ( disposing )
        {
            GC.SuppressFinalize( this );
        }
    }

    public async Task DisposeAsync( CancellationToken cancellationToken = default )
    {
        using ( var activity = this.logger.Default.OpenActivity( Formatted( "Disposing" ) ) )
        {
            try
            {
                if ( !this.TryChangeStatus( Status.Default, Status.DisposingPhase1 ) )
                {
                    activity.SetOutcome( LogLevel.Debug, Formatted( "The method was already called." ) );
                    await this.disposeTask.Task;

                    return;
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.Subscriber.UnsubscribeAllAsync();

                    cancellationToken.ThrowIfCancellationRequested();

                    // Messages are not enqueued from this point.

                    this.notificationProcessingLock.Set();
                    this.notificationQueue.CompleteAdding();

                    using ( cancellationToken.Register( () => this.notificationProcessingThreadCompleted.SetCanceled() ) )
                    {
                        // All messages are processed at this point.
                        await this.notificationProcessingThreadCompleted.Task;
                    }

                    this.ChangeStatus( Status.DisposingPhase2 );

                    this.notificationQueue.Dispose();
                    this.notificationProcessingLock.Dispose();

                    this.ChangeStatus( Status.Disposed );

                    activity.SetSuccess();
                    this.disposeTask.SetResult( true );
                }
                catch ( Exception e )
                {
                    this.disposeTask.SetException( e );

                    throw;
                }
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }

        GC.SuppressFinalize( this );
    }

    public int BackgroundTaskExceptions { get; private set; }

    /// <summary>
    /// Gets the default connection timeout when creating a Redis backend. 
    /// </summary>
    /// <remarks>
    /// When you change this value, please also change the documentation for <see cref="RedisCacheInvalidatorOptions.ConnectionTimeout"/>
    /// and <see cref="RedisCachingBackendConfiguration.ConnectionTimeout"/>.
    /// </remarks>
    public static TimeSpan DefaultSubscriptionTimeout { get; } = TimeSpan.FromMinutes( 1 );

    public override string ToString()
    {
        return string.Format( CultureInfo.InvariantCulture, "{{RedisNotificationQueue {0}}}", this.id );
    }

    private enum Status
    {
        Default,
        DisposingPhase1,
        DisposingPhase2,
        Disposed
    }
}