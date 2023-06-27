// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Serializers;
using Metalama.Patterns.Contracts;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// A <see cref="CachingBackend"/> for Redis, based on the <c>StackExchange.Redis</c> client.
/// </summary>
public class RedisCachingBackend : CachingBackend
{
    private readonly Func<ISerializer> _createSerializerFunc;
    private readonly ConcurrentStack<ISerializer> _serializerPool = new();
    private readonly bool _ownsConnection;
    private readonly BackgroundTaskScheduler _backgroundTaskScheduler = new();

    #region Events

    private const string _itemRemovedEvent = "item-removed";

#pragma warning disable CA2213 // We're doing DisposeAsync.
    private RedisNotificationQueue _notificationQueue;
#pragma warning restore CA2213

    #endregion

    [Protected]
    internal readonly RedisKeyBuilder KeyBuilder;

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
    public RedisCachingBackendConfiguration Configuration { get; }

    /// <summary>
    /// Initializes a new <see cref="RedisCachingBackend"/>.
    /// </summary>
    /// <param name="connection">The Redis connection.</param>
    /// <param name="configuration">Configuration.</param>
    internal RedisCachingBackend( [Required] IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration )
    {
        this.Connection = connection;
        this._ownsConnection = configuration.OwnsConnection;
        this.Configuration = configuration;
        this.Database = this.Connection.GetDatabase( configuration.Database );

        this.KeyBuilder = new RedisKeyBuilder( this.Database, configuration );

        this._createSerializerFunc = configuration.CreateSerializer ?? (() => new BinarySerializer());
    }

    /// <summary>
    /// Initializes the current <see cref="RedisCachingBackend"/>.
    /// </summary>
    internal void Init()
    {
        this._notificationQueue = RedisNotificationQueue.Create(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create( this.KeyBuilder.EventsChannel, this.KeyBuilder.NotificationChannel ),
            this.ProcessNotification,
            this.Configuration.ConnectionTimeout );
    }

    /// <summary>
    /// Asynchronously initializes the current <see cref="RedisCachingBackend"/>.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    internal async Task InitAsync( CancellationToken cancellationToken )
    {
        this._notificationQueue = await RedisNotificationQueue.CreateAsync(
            this.ToString(),
            this.Connection,
            ImmutableArray.Create(
                this.KeyBuilder.EventsChannel,
                this.KeyBuilder.NotificationChannel ),
            this.ProcessNotification,
            this.Configuration.ConnectionTimeout,
            cancellationToken );
    }

    /// <summary>
    /// Creates a new <see cref="RedisCachingBackend"/>.
    /// </summary>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="configuration">Configuration of the new back-end.</param>
    /// <returns>A <see cref="RedisCachingBackend"/>, <see cref="DependenciesRedisCachingBackend"/>, or a <see cref="TwoLayerCachingBackendEnhancer"/>,
    /// according to the properties of the <paramref name="configuration"/>.</returns>
    public static CachingBackend Create( [Required] IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration )
    {
        // #20775 Caching: two-layered cache should modify the key to avoid conflicts when toggling the option
        if ( configuration.IsLocallyCached )
        {
            if ( configuration.IsFrozen )
            {
                configuration = configuration.Clone();
            }

            if ( configuration.KeyPrefix != null )
            {
                configuration.KeyPrefix += "_L2";
            }
            else
            {
                configuration.KeyPrefix = "L2";
            }
        }

        configuration.Freeze();

        RedisCachingBackend backend;

        if ( configuration.SupportsDependencies )
        {
            backend = new DependenciesRedisCachingBackend( connection, configuration );
        }
        else
        {
            backend = new RedisCachingBackend( connection, configuration );
        }

        backend.Init();

        CachingBackend enhancer;

        if ( configuration.IsLocallyCached )
        {
            enhancer = new TwoLayerCachingBackendEnhancer( new NonBlockingCachingBackendEnhancer( backend ) );
        }
        else
        {
            enhancer = backend;
        }

        return enhancer;
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="RedisCachingBackend"/>.
    /// </summary>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="configuration">Configuration of the new back-end.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A task returning a <see cref="RedisCachingBackend"/>, <see cref="DependenciesRedisCachingBackend"/>, or a <see cref="TwoLayerCachingBackendEnhancer"/>,
    /// according to the properties of the <paramref name="configuration"/>.</returns>
    public static async Task<CachingBackend> CreateAsync(
        [Required] IConnectionMultiplexer connection,
        [Required] RedisCachingBackendConfiguration configuration,
        CancellationToken cancellationToken = default )
    {
        configuration.Freeze();

        RedisCachingBackend backend;

        if ( configuration.SupportsDependencies )
        {
            backend = new DependenciesRedisCachingBackend( connection, configuration );
        }
        else
        {
            backend = new RedisCachingBackend( connection, configuration );
        }

        await backend.InitAsync( cancellationToken );

        CachingBackend enhancer;

        if ( configuration.IsLocallyCached )
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            enhancer = new TwoLayerCachingBackendEnhancer( new NonBlockingCachingBackendEnhancer( backend ) );
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
        else
        {
            enhancer = backend;
        }

        return enhancer;
    }

    internal RedisCachingBackend(
        IConnectionMultiplexer connection,
        IDatabase database,
        RedisKeyBuilder keyBuilder,
        RedisCachingBackendConfiguration configuration )
    {
        this.Connection = connection;
        this.Database = database;
        this.KeyBuilder = keyBuilder;
        this.Configuration = configuration ?? new RedisCachingBackendConfiguration();
        this._ownsConnection = false;
    }

    /// <inheritdoc />
    protected override CachingBackendFeatures CreateFeatures() => new RedisCachingBackendFeatures( this );

    #region Events

    private void ProcessNotification( RedisNotification notification )
    {
        if ( notification.Channel == this.KeyBuilder.EventsChannel )
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

        string keyKind;
        string itemKey;

        if ( !this.KeyBuilder.TryParseKeyspaceNotification( channelName, out keyKind, out itemKey ) )
        {
            return;
        }

        if ( keyKind != RedisKeyBuilder.ValueKindPrefix )
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

        this.OnItemRemoved( itemKey, reason, Guid.Empty );
    }

    private void ProcessEvent( RedisNotification notification )
    {
        var tokenizer = new StringTokenizer( notification.Value );
        var kind = tokenizer.GetNext();
        var sourceIdStr = tokenizer.GetNext();
        var key = tokenizer.GetRest();

        if ( kind == null || sourceIdStr == null || key == null )
        {
            this.LogSource.Warning.Write( Formatted( "Cannot parse the event '{Event}'. Skipping it.", notification.Value ) );

            return;
        }

        Guid sourceId;

        if ( !Guid.TryParse( sourceIdStr, out sourceId ) )
        {
            this.LogSource.Warning.Write( Formatted( "Cannot parse the SourceId '{SourceId}' into a Guid. Skipping the event.", sourceIdStr ) );

            return;
        }

        if ( !this.ProcessEvent( kind, key, sourceId ) )
        {
            this.LogSource.Warning.Write( Formatted( "Don't know how to process the event kind {Kind}.", kind ) );
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
                this.LogSource.Debug.Write( Formatted( "Event {Kind} ignored.", kind ) );

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
        this.LogSource.Debug.Write( Formatted( "Publishing message \"{Message}\" to {Channel}.", value, this.KeyBuilder.EventsChannel ) );

        return this._notificationQueue.Subscriber.PublishAsync( this.KeyBuilder.EventsChannel, value );
    }

    /// <summary>
    /// Sends of event.
    /// </summary>
    /// <param name="kind">Kind of event.</param>
    /// <param name="key">Key of the item (value key or dependency key, typically).</param>
    protected void SendEvent( string kind, string key )
    {
        var value = kind + ":" + this.Id + ":" + key;
        this.LogSource.Debug.Write( Formatted( "Publishing message \"{Message}\" to {Channel}.", value, this.KeyBuilder.EventsChannel ) );

        this._notificationQueue.Subscriber.Publish( this.KeyBuilder.EventsChannel, value );
    }

    #endregion

    /// <exclude />
    protected virtual async Task DeleteItemAsync( string key )
    {
        await this.Database.KeyDeleteAsync( this.KeyBuilder.GetValueKey( key ) );
    }

    /// <exclude />
    protected virtual void DeleteItem( string key )
    {
        this.Database.KeyDelete( this.KeyBuilder.GetValueKey( key ) );
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

    private byte[] Serialize( object item )
    {
        ISerializer serializer;

        if ( !this._serializerPool.TryPop( out serializer ) )
        {
            serializer = this._createSerializerFunc();
        }

        var bytes = serializer.Serialize( item );

        // No try/finally. We don't want to reuse the serializer if there is an exception.

        this._serializerPool.Push( serializer );

        return bytes;
    }

    private object Deserialize( byte[] bytes )
    {
        ISerializer serializer;

        if ( !this._serializerPool.TryPop( out serializer ) )
        {
            serializer = this._createSerializerFunc();
        }

        object item;

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
    private static object CreateCacheValue( CacheItem item )
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
        var valueKey = this.KeyBuilder.GetValueKey( key );

        var expiry = this.CreateExpiry( item );

        this.Database.StringSet( valueKey, value, expiry );
    }

    /// <inheritdoc />
    protected override async Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
        var value = this.CreateRedisValue( item );

        var valueKey = this.KeyBuilder.GetValueKey( key );

        var expiry = this.CreateExpiry( item );

        await this.Database.StringSetAsync( valueKey, value, expiry );
    }

    /// <inheritdoc />
    protected override bool ContainsItemCore( string key )
    {
        return this.Database.KeyExists( this.KeyBuilder.GetValueKey( key ) );
    }

    /// <inheritdoc />
    protected override Task<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        return this.Database.KeyExistsAsync( this.KeyBuilder.GetValueKey( key ) );
    }

    /// <exclude />
    protected object GetCacheValue( RedisKey valueKey, RedisValue value )
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
        var valueKey = this.KeyBuilder.GetValueKey( key );
        var serializedValue = this.Database.StringGet( valueKey );

        if ( !serializedValue.HasValue )
        {
            return null;
        }

        var cacheValue = this.GetCacheValue( valueKey, serializedValue );

        return new CacheValue( cacheValue );
    }

    /// <exclude />
    protected override async Task<CacheValue?> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
    {
        var valueKey = this.KeyBuilder.GetValueKey( key );
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
    protected override async Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
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
    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
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
    /// Destructor.
    /// </summary>
    [SuppressMessage( "Microsoft.Design", "CA1031" )]
#pragma warning disable CA1063 // Implement IDisposable Correctly
    ~RedisCachingBackend()
    {
        try
        {
            this.Dispose( false );
        }
        catch ( Exception e )
        {
            this.LogSource.Error.Write( Formatted( "Exception when finalizing the RedisNotificationQueue." ), e );
        }
    }

    /// <inheritdoc />
    protected override void ClearCore()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    protected override Task ClearAsyncCore( CancellationToken cancellationToken )
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

    protected override int BackgroundTaskExceptions => base.BackgroundTaskExceptions + this._notificationQueue.BackgroundTaskExceptions;

    /// <summary>
    /// Features of <see cref="RedisCachingBackend"/>.
    /// </summary>
    internal class RedisCachingBackendFeatures : CachingBackendFeatures
    {
        private readonly RedisCachingBackend _parent;

        /// <summary>
        /// Initializes a new <see cref="RedisCachingBackendFeatures"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="RedisCachingBackend"/>.</param>
        public RedisCachingBackendFeatures( [Required] RedisCachingBackend parent )
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