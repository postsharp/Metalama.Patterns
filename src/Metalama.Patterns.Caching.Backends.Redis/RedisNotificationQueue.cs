// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal sealed class RedisNotificationQueue : ITestableCachingComponent
{
    private const int _connectDelay = 10;

    private LogSource _logger = null!; // "Guaranteed" to be initialized via Init et al.

    public ISubscriber Subscriber { get; }

#pragma warning disable CA2213 // This is properly disposed
    private readonly BlockingCollection<RedisNotification> _notificationQueue = new();
    private readonly ManualResetEventSlim _notificationProcessingLock = new( true );
#pragma warning restore CA2213
    private readonly TaskCompletionSource<bool> _disposeTask = new();
    private readonly StackTrace _allocationStackTrace = new();

    private readonly Thread _notificationProcessingThread;
    private readonly TaskCompletionSource<bool> _notificationProcessingThreadCompleted = new();
    private readonly ImmutableArray<RedisChannel> _channels;
    private readonly Action<RedisNotification> _handler;
    private readonly TimeSpan _connectionTimeout;
    private volatile TaskCompletionSource<bool> _queueEmptyTask = new();
    private static volatile int _notificationProcessingThreads;
    private readonly string _id;
    private volatile int _status;

    private RedisNotificationQueue(
        string id,
        IConnectionMultiplexer connection,
        ImmutableArray<RedisChannel> channels,
        Action<RedisNotification> handler,
        TimeSpan connectionTimeout )
    {
        this._id = id;
        this._channels = channels;
        this._handler = handler;
        this._connectionTimeout = connectionTimeout;
        this.Subscriber = connection.GetSubscriber();

        this._notificationProcessingThread = new Thread( ProcessNotificationQueue )
        {
            Name = nameof(DependenciesRedisCachingBackend) + ".ProcessNotificationQueue"
        };
    }

    ~RedisNotificationQueue()
    {
        try
        {
            this._logger.Error.Write(
                Formatted(
                    "The resource {ObjectId} has not been properly disposed. Allocation stack trace:\n{AllocStackTrace}",
                    this._id,
                    this._allocationStackTrace.ToString() ) );

            try
            {
                this.Dispose( false );
            }
            catch ( Exception e )
            {
                this._logger.Error.Write( Formatted( "Exception when finalizing the RedisNotificationQueue." ), e );
                this.BackgroundTaskExceptions++;
            }
        }
        catch
        {
            // ignored
        }
    }

    private void Init()
    {
        this._logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.GetType() );

        var startedAt = DateTime.Now;

        foreach ( var channel in this._channels )
        {
            this.Subscriber.Subscribe( channel, this.OnNotificationReceived );

            while ( !this.Subscriber.IsConnected( channel ) )
            {
                this._logger.Debug.EnabledOrNull?.Write( Formatted( "Not connected to {Channel}. Waiting {Delay} ms and retrying.", channel, _connectDelay ) );

                Thread.Sleep( _connectDelay );

                if ( DateTime.Now > startedAt + this._connectionTimeout )
                {
                    throw new TimeoutException( "Could not connect to a Redis server in the connection timeout." );
                }
            }
        }

        this.Subscriber.Ping();

        this._notificationProcessingThread.Start( new WeakReference<RedisNotificationQueue>( this ) );
    }

    private async Task<RedisNotificationQueue> InitAsync( CancellationToken cancellationToken )
    {
        this._logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.GetType() );

        foreach ( var channel in this._channels )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.Subscriber.SubscribeAsync( channel, this.OnNotificationReceived );

            while ( !this.Subscriber.IsConnected( channel ) )
            {
                cancellationToken.ThrowIfCancellationRequested();

                this._logger.Debug.EnabledOrNull?.Write( Formatted( "Not connected to {Channel}. Waiting {Delay} ms and retrying.", channel, _connectDelay ) );

                await Task.Delay( _connectDelay, cancellationToken );
            }
        }

        await this.Subscriber.PingAsync();

        this._notificationProcessingThread.Start( new WeakReference<RedisNotificationQueue>( this ) );

        return this;
    }

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
        this._logger.Debug.EnabledOrNull?.Write( Formatted( "Received notification {Value} on channel {Channel}", value, channel ) );

        try
        {
            if ( !this._notificationQueue.IsAddingCompleted )
            {
                this._notificationQueue.Add( new RedisNotification { Channel = channel, Value = value } );

                return;
            }
        }
        catch ( ObjectDisposedException ) { }

        this._logger.Debug.EnabledOrNull?.Write( Formatted( "The notification was not queued because the queue was already disposed." ) );
    }

    private static void ProcessNotificationQueue( object? state )
    {
        if ( state is not WeakReference<RedisNotificationQueue> queueRef )
        {
            throw new RedisCachingBackendAssertionFailedException( "null was not expected." );
        }

        if ( !queueRef.TryGetTarget( out var queue ) )
        {
            return;
        }

        Interlocked.Increment( ref _notificationProcessingThreads );

        try
        {
            var logger = queue._logger;
            var id = queue._id;
            var blockingCollection = queue._notificationQueue;

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

                if ( queue._notificationQueue.IsAddingCompleted )
                {
                    break;
                }

                try
                {
                    queue._notificationProcessingLock.Wait();

                    using ( var activity =
                           logger.Default.OpenActivity(
                               Formatted( "Processing notification {Value} received on channel {Channel}", notification.Value, notification.Channel ) ) )
                    {
                        try
                        {
                            queue._handler( notification );

                            activity.SetSuccess();
                        }
                        catch ( Exception e )
                        {
                            queue.BackgroundTaskExceptions++;

                            activity.SetException( e );
                        }
                    }

                    if ( queue._notificationQueue.Count == 0 )
                    {
                        Interlocked.Exchange( ref queue._queueEmptyTask, new TaskCompletionSource<bool>() ).SetResult( true );
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
                queue._notificationProcessingThreadCompleted.TrySetResult( true );
            }

            Interlocked.Decrement( ref _notificationProcessingThreads );
        }
    }

    internal static int NotificationProcessingThreads => _notificationProcessingThreads;

    internal void SuspendProcessing()
    {
        this._notificationProcessingLock.Reset();
    }

    internal void ResumeProcessing()
    {
        this._notificationProcessingLock.Set();
    }

    internal Task WhenQueueEmpty()
    {
        // This is probably not thread safe, but this should be good enough for unit testing.
        Thread.MemoryBarrier();

        if ( this._status >= (int) Status.DisposingPhase2 || this._notificationQueue.Count == 0 )
        {
            return Task.FromResult( true );
        }
        else
        {
            return this._queueEmptyTask.Task;
        }
    }

    private bool TryChangeStatus( Status expectedStatus, Status newStatus )
    {
#pragma warning disable 420
        return Interlocked.CompareExchange( ref this._status, (int) newStatus, (int) expectedStatus ) == (int) expectedStatus;
#pragma warning restore 420
    }

    private void ChangeStatus( Status newStatus )
    {
        this._status = (int) newStatus;
        Thread.MemoryBarrier();
    }

    public void Dispose()
    {
        this.Dispose( true );
    }

    private void Dispose( bool disposing )
    {
        using ( var activity = this._logger.Default.OpenActivity( Formatted( "Disposing" ) ) )
        {
            try
            {
                if ( !this.TryChangeStatus( Status.Default, Status.DisposingPhase1 ) )
                {
                    activity.SetOutcome( LogLevel.Debug, Formatted( "The method was already called." ) );
                    this._disposeTask.Task.Wait();

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

                    this._notificationProcessingLock.Set();
                    this._notificationQueue.CompleteAdding();

                    // All messages are processed at this point.

                    this._notificationProcessingThreadCompleted.Task.Wait();

                    this.ChangeStatus( Status.DisposingPhase2 );

                    this._notificationQueue.Dispose();
                    this._notificationProcessingLock.Dispose();

                    this.ChangeStatus( Status.Disposed );

                    activity.SetSuccess();
                    this._disposeTask.SetResult( true );
                }
                catch ( Exception e )
                {
                    this._disposeTask.SetException( e );

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
        using ( var activity = this._logger.Default.OpenActivity( Formatted( "Disposing" ) ) )
        {
            try
            {
                if ( !this.TryChangeStatus( Status.Default, Status.DisposingPhase1 ) )
                {
                    activity.SetOutcome( LogLevel.Debug, Formatted( "The method was already called." ) );
                    await this._disposeTask.Task;

                    return;
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.Subscriber.UnsubscribeAllAsync();

                    cancellationToken.ThrowIfCancellationRequested();

                    // Messages are not enqueued from this point.

                    this._notificationProcessingLock.Set();
                    this._notificationQueue.CompleteAdding();

                    // ReSharper disable once MethodSupportsCancellation
                    // ReSharper disable once BadPreprocessorIndent
#pragma warning disable SA1137
#if NETCOREAPP
                    await
#endif
                        using ( cancellationToken.Register( () => this._notificationProcessingThreadCompleted.SetCanceled() ) )
                    {
                        // All messages are processed at this point.
                        await this._notificationProcessingThreadCompleted.Task;
                    }
#pragma warning restore SA1137

                    this.ChangeStatus( Status.DisposingPhase2 );

                    this._notificationQueue.Dispose();
                    this._notificationProcessingLock.Dispose();

                    this.ChangeStatus( Status.Disposed );

                    activity.SetSuccess();
                    this._disposeTask.SetResult( true );
                }
                catch ( Exception e )
                {
                    this._disposeTask.SetException( e );

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
        return string.Format( CultureInfo.InvariantCulture, "{{RedisNotificationQueue {0}}}", this._id );
    }

    private enum Status
    {
        Default,
        DisposingPhase1,
        DisposingPhase2,
        Disposed
    }
}