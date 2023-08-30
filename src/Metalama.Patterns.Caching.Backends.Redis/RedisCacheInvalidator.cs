// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// An implementation of <see cref="CacheInvalidator"/>  that uses Redis publish/subscribe channels to invalidate several
/// instances of local caches.
/// </summary>
[PublicAPI]
public sealed class RedisCacheInvalidator : CacheInvalidator
{
    private readonly bool _ownsConnection;
    private readonly RedisChannel _channel;
    private readonly TimeSpan _connectionTimeout;

    private RedisNotificationQueue NotificationQueue { get; set; } = null!; // "Guaranteed" to be initialized via Init et al.

    /// <summary>
    /// Gets the Redis <see cref="IConnectionMultiplexer"/> used by the current <see cref="RedisCacheInvalidator"/>.
    /// </summary>
    private IConnectionMultiplexer Connection { get; }

    private RedisCacheInvalidator(
        CachingBackend underlyingBackend,
        IConnectionMultiplexer connection,
        RedisCacheInvalidatorOptions options ) : base( underlyingBackend, options )
    {
        this.Connection = connection;
        this._ownsConnection = options.OwnsConnection;
        this._connectionTimeout = options.ConnectionTimeout;
        this._channel = new RedisChannel( options.ChannelName, RedisChannel.PatternMode.Literal );
    }

    private void Init()
    {
        this.NotificationQueue = RedisNotificationQueue.Create(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this._channel ),
            this.HandleMessage,
            this._connectionTimeout,
            this.Configuration.ServiceProvider );
    }

    private async Task<RedisCacheInvalidator> InitAsync( CancellationToken cancellationToken )
    {
        this.NotificationQueue = await RedisNotificationQueue.CreateAsync(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this._channel ),
            this.HandleMessage,
            this._connectionTimeout,
            this.Configuration.ServiceProvider,
            cancellationToken );

        return this;
    }

    /// <summary>
    /// Creates a new <see cref="RedisCacheInvalidator"/>.
    /// </summary>
    /// <param name="backend">A local (typically in-memory) caching back-end.</param>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="options">Options.</param>
    /// <returns>A new <see cref="RedisCacheInvalidator"/>.</returns>
    public static RedisCacheInvalidator Create(
        CachingBackend backend,
        IConnectionMultiplexer connection,
        RedisCacheInvalidatorOptions options )
    {
        var invalidator = new RedisCacheInvalidator( backend, connection, options );
        invalidator.Init();

        return invalidator;
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="RedisCacheInvalidator"/>.
    /// </summary>
    /// <param name="backend">A local (typically in-memory) caching back-end.</param>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="options">Options.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> returning a new <see cref="RedisCacheInvalidator"/>.</returns>
    public static Task<RedisCacheInvalidator> CreateAsync(
        CachingBackend backend,
        IConnectionMultiplexer connection,
        RedisCacheInvalidatorOptions options,
        CancellationToken cancellationToken = default )
    {
        var invalidator = new RedisCacheInvalidator( backend, connection, options );

        return invalidator.InitAsync( cancellationToken );
    }

    private void HandleMessage( RedisNotification notification )
    {
        this.OnMessageReceived( notification.Value );
    }

    /// <inheritdoc />
    protected override async Task SendMessageAsync( string message, CancellationToken cancellationToken )
    {
        await this.NotificationQueue.Subscriber.PublishAsync( this._channel, message );
    }

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );

        this.NotificationQueue.Dispose();

        if ( this._ownsConnection )
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken );

        await this.NotificationQueue.DisposeAsync( cancellationToken );

        if ( this._ownsConnection )
        {
            await this.Connection.CloseAsync();
            this.Connection.Dispose();
        }
    }
}