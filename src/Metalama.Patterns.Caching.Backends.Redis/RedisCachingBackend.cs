// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Serializers;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// A <see cref="CachingBackend"/> for Redis, based on the <c>StackExchange.Redis</c> client.
/// </summary>
[PublicAPI]
internal class RedisCachingBackend : CachingBackend
{
    private const string _itemRemovedEvent = "item-removed";

    private readonly Func<ICachingSerializer> _createSerializerFunc;
    private readonly ConcurrentStack<ICachingSerializer> _serializerPool = new();
    private readonly bool _ownsConnection;
    private readonly BackgroundTaskScheduler _backgroundTaskScheduler;
    private int _backgroundTaskExceptions;

#pragma warning disable SA1401
    private protected readonly RedisKeyBuilder _keyBuilder;
#pragma warning restore SA1401

    private RedisNotificationQueue? _notificationQueue;

    /// <summary>
    /// Gets <see cref="_notificationQueue"/> if not null, otherwise throws <see cref="CachingAssertionFailedException"/>.
    /// </summary>
    private RedisNotificationQueue NotificationQueue
        => this._notificationQueue ?? throw new CachingAssertionFailedException( nameof(this._notificationQueue) + " has not been initialized." );

    /// <summary>
    /// Gets the Redis database used by the current <see cref="RedisCachingBackend"/>.
    /// </summary>
    public IDatabase Database { get; }

    /// <summary>
    /// Gets the Redis connection used by the current <see cref="RedisCachingBackend"/>.
    /// </summary>
    public IConnectionMultiplexer Connection { get; }

    /// <summary>
    /// Gets the configuration of the current <see cref="RedisCachingBackend"/>.
    /// </summary>
    public new RedisCachingBackendConfiguration Configuration => (RedisCachingBackendConfiguration) base.Configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCachingBackend"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection.</param>
    /// <param name="configuration">Configuration.</param>
    /// <param name="serviceProvider"></param>
    internal RedisCachingBackend( IConnectionMultiplexer connection, RedisCachingBackendConfiguration configuration, IServiceProvider? serviceProvider ) : base(
        configuration,
        serviceProvider )
    {
        this.Connection = connection;
        this._ownsConnection = configuration.OwnsConnection;
        this.Database = this.Connection.GetDatabase( configuration.Database );

        this._keyBuilder = new RedisKeyBuilder( this.Database, configuration );

        this._createSerializerFunc = configuration.CreateSerializer ?? (() => new JsonCachingFormatter());
        this._backgroundTaskScheduler = new BackgroundTaskScheduler( serviceProvider );
    }

    internal RedisCachingBackend(
        IConnectionMultiplexer connection,
        IDatabase database,
        RedisKeyBuilder keyBuilder,
        RedisCachingBackendConfiguration configuration,
        IServiceProvider? serviceProvider ) : base( configuration, serviceProvider )
    {
        this.Connection = connection;
        this.Database = database;
        this._keyBuilder = keyBuilder;
        this._ownsConnection = false;
        this._backgroundTaskScheduler = new BackgroundTaskScheduler( serviceProvider );
        this._createSerializerFunc = this.Configuration.CreateSerializer ?? (() => new JsonCachingFormatter());
    }

    protected override void Initialize()
    {
        this._notificationQueue = RedisNotificationQueue.Create(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this._keyBuilder.EventsChannel, this._keyBuilder.NotificationChannel ),
            this.ProcessNotification,
            this.Configuration.ConnectionTimeout,
            this.ServiceProvider );

        base.Initialize();
    }

    protected override async Task InitializeAsync( CancellationToken cancellationToken = default )
    {
        this._notificationQueue = await RedisNotificationQueue.CreateAsync(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create(
                this._keyBuilder.EventsChannel,
                this._keyBuilder.NotificationChannel ),
            this.ProcessNotification,
            this.Configuration.ConnectionTimeout,
            this.ServiceProvider,
            cancellationToken );

        await base.InitializeAsync( cancellationToken );
    }

    /// <inheritdoc />
    protected override CachingBackendFeatures CreateFeatures() => new RedisCachingBackendFeatures( this );

    private void ProcessNotification( RedisNotification notification )
    {
        if ( notification.Channel == this._keyBuilder.EventsChannel )
        {
            this.ProcessEvent( notification );
        }
        else
        {
            this.ProcessKeyspaceNotification( notification );
        }
    }

    private void ProcessKeyspaceNotification( RedisNotification notification )
    {
        string channelName = notification.Channel;

        if ( !this._keyBuilder.TryParseKeyspaceNotification( channelName, out var keyKind, out var itemKey ) )
        {
            return;
        }

        if ( !keyKind.Equals( RedisKeyBuilder.ValueKindPrefix.AsSpan(), StringComparison.Ordinal ) )
        {
            return;
        }

        CacheItemRemovedReason reason;

        switch ( notification.Value )
        {
            case "expired":
                reason = CacheItemRemovedReason.Expired;

                break;

            case "evicted":
                reason = CacheItemRemovedReason.Evicted;

                break;

            default:
                return;
        }

        this.OnItemRemoved( itemKey.ToString(), reason, Guid.Empty );
    }

    private void ProcessEvent( RedisNotification notification )
    {
        var tokenizer = new StringTokenizer( notification.Value );
        var kind = tokenizer.GetNext( ':' );
        var sourceIdStr = tokenizer.GetNext( ':' );
        var key = tokenizer.GetRemainder();

        if ( kind.IsEmpty || sourceIdStr.IsEmpty || key.IsEmpty )
        {
            this.Source.Warning.Write( Formatted( "Cannot parse the event '{Event}'. Skipping it.", notification.Value ) );

            return;
        }

#if NET6_0_OR_GREATER
        if ( !Guid.TryParse( sourceIdStr, out var sourceId ) )
#else
        if ( !Guid.TryParse( sourceIdStr.ToString(), out var sourceId ) )
#endif
        {
            this.Source.Warning.Write( Formatted( "Cannot parse the SourceId '{SourceId}' into a Guid. Skipping the event.", sourceIdStr.ToString() ) );

            return;
        }

        if ( !this.ProcessEvent( kind.ToString(), key.ToString(), sourceId ) )
        {
            this.Source.Warning.Write( Formatted( "Don't know how to process the event kind {Kind}.", kind.ToString() ) );
        }
    }

    /// <summary>
    /// Processes an event that was received on the events channel.
    /// </summary>
    /// <param name="kind">Kind of event.</param>
    /// <param name="key">Key of the item (value key or dependency key, typically).</param>
    /// <param name="sourceId"><see cref="Guid"/> of the <see cref="RedisCachingBackend"/> that sent the event.</param>
    /// <returns></returns>
    protected virtual bool ProcessEvent( string kind, string key, Guid sourceId )
    {
        switch ( kind )
        {
            case _itemRemovedEvent:
                this.OnItemRemoved( key, CacheItemRemovedReason.Removed, sourceId );

                return true;

            default:
                this.Source.Debug.Write( Formatted( "Event {Kind} ignored.", kind ) );

                break;
        }

        return false;
    }

    /// <summary>
    /// Asynchronously sends of event.
    /// </summary>
    /// <param name="kind">Kind of event.</param>
    /// <param name="key">Key of the item (value key or dependency key, typically).</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected Task SendEventAsync( string kind, string key )
    {
        var value = kind + ":" + this.Id + ":" + key;
        this.Source.Debug.Write( Formatted( "Publishing message \"{Message}\" to {Channel}.", value, this._keyBuilder.EventsChannel ) );

        return this.NotificationQueue.Subscriber.PublishAsync( this._keyBuilder.EventsChannel, value );
    }

    /// <summary>
    /// Sends of event.
    /// </summary>
    /// <param name="kind">Kind of event.</param>
    /// <param name="key">Key of the item (value key or dependency key, typically).</param>
    protected void SendEvent( string kind, string key )
    {
        var value = kind + ":" + this.Id + ":" + key;
        this.Source.Debug.Write( Formatted( "Publishing message \"{Message}\" to {Channel}.", value, this._keyBuilder.EventsChannel ) );

        this.NotificationQueue.Subscriber.Publish( this._keyBuilder.EventsChannel, value );
    }

    /// <exclude />
    protected virtual async Task DeleteItemAsync( string key )
    {
        await this.Database.KeyDeleteAsync( this._keyBuilder.GetValueKey( key ) );
    }

    /// <exclude />
    protected virtual void DeleteItem( string key )
    {
        this.Database.KeyDelete( this._keyBuilder.GetValueKey( key ) );
    }

    private TimeSpan? CreateExpiry( CacheItem policy )
    {
        TimeSpan? ttl = this.Configuration.DefaultExpiration;

        if ( policy.Configuration != null )
        {
            if ( policy.Configuration.AbsoluteExpiration.HasValue )
            {
                ttl = policy.Configuration.AbsoluteExpiration;
            }
            else if ( policy.Configuration.SlidingExpiration.HasValue )
            {
                ttl = policy.Configuration.SlidingExpiration.Value;
            }
            else if ( policy.Configuration.Priority.GetValueOrDefault() == CacheItemPriority.NotRemovable )
            {
                ttl = null;
            }
        }

        return ttl;
    }

    private byte[] Serialize( object? item )
    {
        if ( !this._serializerPool.TryPop( out var serializer ) )
        {
            serializer = this._createSerializerFunc();
        }

        var bytes = serializer.Serialize( item );

        // No try/finally. We don't want to reuse the serializer if there is an exception.

        this._serializerPool.Push( serializer );

        return bytes;
    }

    private object? Deserialize( byte[] bytes )
    {
        if ( !this._serializerPool.TryPop( out var serializer ) )
        {
            serializer = this._createSerializerFunc();
        }

        object? item;

        try
        {
            item = serializer.Deserialize( bytes );
        }
        catch ( Exception e )
        {
            throw new InvalidCacheItemException( "Failed to deserialize a cache item: " + e.Message, e );
        }

        // No try/finally. We don't want to reuse the serializer if there is an exception.

        this._serializerPool.Push( serializer );

        return item;
    }

    /// <summary>
    /// Creates the value that will be serialized and stored in the cache.
    /// </summary>
    /// <param name="item">The source <see cref="CacheItem"/>.</param>
    /// <returns>The <see cref="object"/> that should be serialized or stored in the cache. Typically
    /// <see cref="CacheItem.Value"/> or a <see cref="RedisCacheValue"/>.
    /// </returns>
    private static object? CreateCacheValue( CacheItem item )
    {
        if ( item.Configuration?.SlidingExpiration == null )
        {
            return item.Value;
        }
        else
        {
            return new RedisCacheValue( item.Value, item.Configuration.SlidingExpiration.Value );
        }
    }

    internal RedisValue CreateRedisValue( CacheItem item )
    {
        var value = CreateCacheValue( item );

        return this.Serialize( value );
    }

    /// <inheritdoc />
    protected override void SetItemCore( string key, CacheItem item )
    {
        // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
        var value = this.CreateRedisValue( item );
        var valueKey = this._keyBuilder.GetValueKey( key );

        var expiry = this.CreateExpiry( item );

        this.Database.StringSet( valueKey, value, expiry );
    }

    /// <inheritdoc />
    protected override async ValueTask SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
        var value = this.CreateRedisValue( item );

        var valueKey = this._keyBuilder.GetValueKey( key );

        var expiry = this.CreateExpiry( item );

        await this.Database.StringSetAsync( valueKey, value, expiry );
    }

    /// <inheritdoc />
    protected override bool ContainsItemCore( string key )
    {
        return this.Database.KeyExists( this._keyBuilder.GetValueKey( key ) );
    }

    /// <inheritdoc />
    protected override async ValueTask<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        return await this.Database.KeyExistsAsync( this._keyBuilder.GetValueKey( key ) );
    }

    /// <exclude />
    protected object? GetCacheValue( RedisKey valueKey, RedisValue value )
    {
        var cacheValue = this.Deserialize( value );

        if ( cacheValue is RedisCacheValue withSlidingExpiration )
        {
            this.ExecuteNonBlockingTask( () => this.Database.KeyExpireAsync( valueKey, withSlidingExpiration.SlidingExpiration ) );
            cacheValue = withSlidingExpiration.Value;
        }

        return cacheValue;
    }

    /// <exclude />
    protected override CacheValue? GetItemCore( string key, bool includeDependencies )
    {
        var valueKey = this._keyBuilder.GetValueKey( key );
        var serializedValue = this.Database.StringGet( valueKey );

        if ( !serializedValue.HasValue )
        {
            return null;
        }

        var cacheValue = this.GetCacheValue( valueKey, serializedValue );

        return new CacheValue( cacheValue );
    }

    /// <exclude />
    protected override async ValueTask<CacheValue?> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
    {
        var valueKey = this._keyBuilder.GetValueKey( key );
        var serializedValue = await this.Database.StringGetAsync( valueKey );

        if ( !serializedValue.HasValue )
        {
            return null;
        }

        var cacheValue = this.GetCacheValue( valueKey, serializedValue );

        return new CacheValue( cacheValue );
    }

    /// <inheritdoc />
    protected override bool ContainsDependencyCore( string key )
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    protected override void InvalidateDependencyCore( string key )
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    protected override async ValueTask RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        await this.DeleteItemAsync( key );
        await this.SendEventAsync( _itemRemovedEvent, key );
    }

    /// <inheritdoc />
    protected override void RemoveItemCore( string key )
    {
        this.DeleteItem( key );
        this.SendEvent( _itemRemovedEvent, key );
    }

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        // Do not dispose Redis-related resources during finalization: it blocks the finalizer thread and
        // causes timeouts, and things are probably being disposed in the wrong order anyway.

        if ( disposing )
        {
            this._notificationQueue?.Dispose();
        }

        this._backgroundTaskScheduler.Dispose();

        base.DisposeCore( disposing );

        if ( disposing )
        {
            if ( this._ownsConnection )
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }

            GC.SuppressFinalize( this );
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore( CancellationToken cancellationToken )
    {
        if ( this._notificationQueue != null )
        {
            await this._notificationQueue.DisposeAsync( cancellationToken );
        }

        await this._backgroundTaskScheduler.DisposeAsync( cancellationToken );

        await base.DisposeAsyncCore( cancellationToken );

        if ( this._ownsConnection )
        {
            await this.Connection.CloseAsync();
            this.Connection.Dispose();
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="RedisCachingBackend"/> class.
    /// Destructor.
    /// </summary>
    ~RedisCachingBackend()
    {
        try
        {
            this.Dispose( false );
        }
        catch ( Exception e )
        {
            this.Source.Error.Write( Formatted( "Exception when finalizing the RedisNotificationQueue." ), e );
            this._backgroundTaskExceptions++;
        }
    }

    /// <param name="options"></param>
    /// <inheritdoc />
    protected override void ClearCore( ClearCacheOptions options )
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    protected override ValueTask ClearAsyncCore( ClearCacheOptions options, CancellationToken cancellationToken )
    {
        throw new NotSupportedException();
    }

    // Change the visibility of the method.
    internal void ExecuteNonBlockingTask( Func<Task> task )
    {
        this._backgroundTaskScheduler.EnqueueBackgroundTask( task );
    }

    /// <inheritdoc />
    public override async Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken )
    {
        await base.WhenBackgroundTasksCompleted( cancellationToken );

        if ( this._notificationQueue != null )
        {
            await this._notificationQueue.WhenQueueEmpty();
        }
    }

    protected override int BackgroundTaskExceptions
        => base.BackgroundTaskExceptions + this.NotificationQueue.BackgroundTaskExceptions + this._backgroundTaskExceptions;

    /// <summary>
    /// Features of <see cref="RedisCachingBackend"/>.
    /// </summary>
    internal class RedisCachingBackendFeatures : CachingBackendFeatures
    {
        private readonly RedisCachingBackend _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCachingBackendFeatures"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="RedisCachingBackend"/>.</param>
        public RedisCachingBackendFeatures( RedisCachingBackend parent )
        {
            this._parent = parent;
        }

        /// <inheritdoc />
        public override bool Events => this._parent._notificationQueue != null;

        /// <inheritdoc />
        public override bool Clear => false;

        /// <inheritdoc />
        public override bool Dependencies => false;

        /// <inheritdoc />
        public override bool ContainsDependency => false;
    }
}