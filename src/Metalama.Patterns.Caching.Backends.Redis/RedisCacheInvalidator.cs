// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// An implementation of <see cref="CacheInvalidator"/>  that uses Redis publish/subscribe channels to invalidate several
/// instances of local caches.
/// </summary>
public class RedisCacheInvalidator : CacheInvalidator
{
    private readonly bool ownsConnection;
    private readonly RedisChannel channel;
    private readonly TimeSpan connectionTimeout;

    internal RedisNotificationQueue NotificationQueue { get; private set; }

    /// <summary>
    /// Gets the Redis <see cref="IConnectionMultiplexer"/> used by the current <see cref="RedisCacheInvalidator"/>.
    /// </summary>
    public IConnectionMultiplexer Connection { get; }

    private RedisCacheInvalidator(
        CachingBackend underlyingBackend,
        IConnectionMultiplexer connection,
        RedisCacheInvalidatorOptions options ) : base( underlyingBackend, options )
    {
        this.Connection = connection;
        this.ownsConnection = options.OwnsConnection;
        this.connectionTimeout = options.ConnectionTimeout;
        this.channel = new RedisChannel( options.ChannelName, RedisChannel.PatternMode.Literal );
    }

    private void Init()
    {
        this.NotificationQueue = RedisNotificationQueue.Create(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this.channel ),
            this.HandleMessage,
            this.connectionTimeout );
    }

    private async Task<RedisCacheInvalidator> InitAsync( CancellationToken cancellationToken )
    {
        this.NotificationQueue = await RedisNotificationQueue.CreateAsync(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this.channel ),
            this.HandleMessage,
            this.connectionTimeout,
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
    [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
    public static RedisCacheInvalidator Create(
        [Required] CachingBackend backend,
        [Required] IConnectionMultiplexer connection,
        [Required] RedisCacheInvalidatorOptions options )
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
    [SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
    public static Task<RedisCacheInvalidator> CreateAsync(
        [Required] CachingBackend backend,
        [Required] IConnectionMultiplexer connection,
        [Required] RedisCacheInvalidatorOptions options,
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
        await this.NotificationQueue.Subscriber.PublishAsync( this.channel, message );
    }

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );

        this.NotificationQueue.Dispose();

        if ( this.ownsConnection )
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }
    }

    /// <inheritdoc />
    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken );

        await this.NotificationQueue.DisposeAsync( cancellationToken );

        if ( this.ownsConnection )
        {
            await this.Connection.CloseAsync();
            this.Connection.Dispose();
        }
    }
}