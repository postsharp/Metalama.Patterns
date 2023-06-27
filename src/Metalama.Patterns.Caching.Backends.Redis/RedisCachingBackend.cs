// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Constraints;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Serializers;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Utilities;
using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Backends.Redis
{
    /// <summary>
    /// A <see cref="CachingBackend"/> for Redis, based on the <c>StackExchange.Redis</c> client.
    /// </summary>
    public class RedisCachingBackend : CachingBackend
    {
        private readonly Func<ISerializer> createSerializerFunc;
        private readonly ConcurrentStack<ISerializer> serializerPool = new ConcurrentStack<ISerializer>();
        private readonly bool ownsConnection;
        private readonly BackgroundTaskScheduler backgroundTaskScheduler = new BackgroundTaskScheduler();

        #region Events

        private const string itemRemovedEvent = "item-removed";

        #pragma warning disable CA2213 // We're doing DisposeAsync.
        private RedisNotificationQueue notificationQueue;
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
            this.ownsConnection = configuration.OwnsConnection;
            this.Configuration = configuration;
            this.Database = this.Connection.GetDatabase( configuration.Database  );

            this.KeyBuilder = new RedisKeyBuilder( this.Database, configuration );

            this.createSerializerFunc = configuration.CreateSerializer ?? (() => new BinarySerializer());
        }

        /// <summary>
        /// Initializes the current <see cref="RedisCachingBackend"/>.
        /// </summary>
        internal void Init()
        {
            this.notificationQueue = RedisNotificationQueue.Create( this.ToString(), this.Connection,
                                                                    ImmutableArray.Create( this.KeyBuilder.EventsChannel, this.KeyBuilder.NotificationChannel ),
                                                                    this.ProcessNotification, this.Configuration.ConnectionTimeout );
        }

        /// <summary>
        /// Asynchronously initializes the current <see cref="RedisCachingBackend"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        internal async Task InitAsync(CancellationToken cancellationToken)
        {
            this.notificationQueue = await RedisNotificationQueue.CreateAsync( this.ToString(), this.Connection,
                                                                               ImmutableArray.Create( this.KeyBuilder.EventsChannel,
                                                                                                      this.KeyBuilder.NotificationChannel ),
                                                                               this.ProcessNotification, this.Configuration.ConnectionTimeout, cancellationToken );
        }

        /// <summary>
        /// Creates a new <see cref="RedisCachingBackend"/>.
        /// </summary>
        /// <param name="connection">A Redis connection.</param>
        /// <param name="configuration">Configuration of the new back-end.</param>
        /// <returns>A <see cref="RedisCachingBackend"/>, <see cref="DependenciesRedisCachingBackend"/>, or a <see cref="TwoLayerCachingBackendEnhancer"/>,
        /// according to the properties of the <paramref name="configuration"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static CachingBackend Create( [Required] IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration)
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
            if (configuration.IsLocallyCached)
            {
                enhancer = new TwoLayerCachingBackendEnhancer(new NonBlockingCachingBackendEnhancer(  backend) );
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
        public static async Task<CachingBackend> CreateAsync(IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
        {
            configuration.Freeze();

            RedisCachingBackend backend;
            if (configuration.SupportsDependencies)
            {
                backend = new DependenciesRedisCachingBackend(connection, configuration);
            }
            else
            {
                backend = new RedisCachingBackend(connection, configuration);
            }

            await backend.InitAsync(cancellationToken);

            CachingBackend enhancer;
            if (configuration.IsLocallyCached)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                enhancer = new TwoLayerCachingBackendEnhancer(new NonBlockingCachingBackendEnhancer( backend) );
#pragma warning restore CA2000 // Dispose objects before losing scope
            }
            else
            {
                enhancer = backend;
            }

            return enhancer;
        }


        internal RedisCachingBackend( IConnectionMultiplexer connection, IDatabase database, RedisKeyBuilder keyBuilder,
                                            RedisCachingBackendConfiguration configuration )
        {
            this.Connection = connection;
            this.Database = database;
            this.KeyBuilder = keyBuilder;
            this.Configuration = configuration ?? new RedisCachingBackendConfiguration();
            this.ownsConnection = false;
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
                return;

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
            StringTokenizer tokenizer = new StringTokenizer( notification.Value );
            string kind = tokenizer.GetNext();
            string sourceIdStr = tokenizer.GetNext();
            string key = tokenizer.GetRest();

            if ( kind == null || sourceIdStr == null || key == null )
            {
                this.Logger.Write( LogLevel.Warning, "Cannot parse the event '{Event}'. Skipping it.", notification.Value );
                return;
            }

            Guid sourceId;
            if ( !Guid.TryParse( sourceIdStr, out sourceId ) )
            {
                this.Logger.Write( LogLevel.Warning, "Cannot parse the SourceId '{SourceId}' into a Guid. Skipping the event.", sourceIdStr);
                return;
            }

            if ( !this.ProcessEvent( kind, key, sourceId ) )
            {
                this.Logger.Write( LogLevel.Warning, "Don't know how to process the event kind {Kind}.", kind );
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
                case itemRemovedEvent:
                    this.OnItemRemoved( key, CacheItemRemovedReason.Removed, sourceId );
                    return true;

                default:
                    this.Logger.Write( LogLevel.Debug, "Event {Kind} ignored.", kind );
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
            string value = kind + ":" + this.Id + ":" + key;
            this.Logger.Write( LogLevel.Debug, "Publishing message \"{Message}\" to {Channel}.", value, this.KeyBuilder.EventsChannel );
            
            return this.notificationQueue.Subscriber.PublishAsync( this.KeyBuilder.EventsChannel, value );
        }

        /// <summary>
        /// Sends of event.
        /// </summary>
        /// <param name="kind">Kind of event.</param>
        /// <param name="key">Key of the item (value key or dependency key, typically).</param>
        protected void SendEvent( string kind, string key )
        {
            string value = kind + ":" + this.Id + ":" + key;
            this.Logger.Write( LogLevel.Debug, "Publishing message \"{Message}\" to {Channel}.", value, this.KeyBuilder.EventsChannel );

            this.notificationQueue.Subscriber.Publish( this.KeyBuilder.EventsChannel, value );
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

            if ( !this.serializerPool.TryPop( out serializer ) )
            {
                serializer = this.createSerializerFunc();
            }

            byte[] bytes = serializer.Serialize( item );

            // No try/finally. We don't want to reuse the serializer if there is an exception.

            this.serializerPool.Push( serializer );

            return bytes;
        }

        private object Deserialize( byte[] bytes )
        {
            ISerializer serializer;

            if ( !this.serializerPool.TryPop( out serializer ) )
            {
                serializer = this.createSerializerFunc();
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

            this.serializerPool.Push( serializer );

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
            object value = CreateCacheValue( item );

            return this.Serialize( value );
        }

        /// <inheritdoc />
        protected override void SetItemCore( string key, CacheItem item )
        {
            // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
            RedisValue value = this.CreateRedisValue( item );
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            TimeSpan? expiry = this.CreateExpiry( item );

            this.Database.StringSet( valueKey, value, expiry );
        }


        /// <inheritdoc />
        protected override async Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
        {
            // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
            RedisValue value = this.CreateRedisValue( item );
            
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            TimeSpan? expiry = this.CreateExpiry( item );

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
            object cacheValue = this.Deserialize( value );


            if (cacheValue is RedisCacheValue withSlidingExpiration)
            {
                this.ExecuteNonBlockingTask(() => this.Database.KeyExpireAsync(valueKey, withSlidingExpiration.SlidingExpiration));
                cacheValue = withSlidingExpiration.Value;
            }

            return cacheValue;
        }

        /// <exclude />
        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );
            RedisValue serializedValue = this.Database.StringGet( valueKey );

            if ( !serializedValue.HasValue )
            {
                return null;
            }

            object cacheValue = this.GetCacheValue( valueKey, serializedValue );

            return new CacheValue( cacheValue );
        }

        /// <exclude />
        protected override async Task<CacheValue> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
        {
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );
            RedisValue serializedValue = await this.Database.StringGetAsync( valueKey );

            if ( !serializedValue.HasValue )
            {
                return null;
            }

            object cacheValue = this.GetCacheValue( valueKey, serializedValue );

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
            await this.SendEventAsync( itemRemovedEvent, key );
        }

        /// <inheritdoc />
        protected override void RemoveItemCore( string key )
        {
            this.DeleteItem( key );
            this.SendEvent( itemRemovedEvent, key );
        }

        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            // Do not dispose Redis-related resources during finalization: it blocks the finalizer thread and
            // causes timeouts, and things are probably being disposed in the wrong order anyway.

            if ( disposing )
            {
                this.notificationQueue?.Dispose();
            }

            this.backgroundTaskScheduler.Dispose();

            base.DisposeCore(disposing);

            if ( disposing )
            {
                if ( this.ownsConnection )
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

            if ( this.notificationQueue != null )
            {
                await this.notificationQueue.DisposeAsync( cancellationToken );
            }

            await this.backgroundTaskScheduler.DisposeAsync( cancellationToken );

            await base.DisposeAsyncCore( cancellationToken );


            if ( this.ownsConnection )
            {
                await this.Connection.CloseAsync();
                this.Connection.Dispose();
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031")]
        #pragma warning disable CA1063 // Implement IDisposable Correctly
        ~RedisCachingBackend()
        {
            try
            {
                this.Dispose( false );
            }
            catch ( Exception e )
            {
                this.Logger.WriteException( LogLevel.Error, e, "Exception when finalizing the RedisNotificationQueue." );
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
            this.backgroundTaskScheduler.EnqueueBackgroundTask( task );
        }

        /// <inheritdoc />
        public override async Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken )
        {
            await base.WhenBackgroundTasksCompleted( cancellationToken );
            if ( this.notificationQueue != null )
            {
                await this.notificationQueue.WhenQueueEmpty();
            }
        }

        internal override int BackgroundTaskExceptions => base.BackgroundTaskExceptions + this.notificationQueue.BackgroundTaskExceptions;

        /// <summary>
        /// Features of <see cref="RedisCachingBackend"/>.
        /// </summary>
        internal class RedisCachingBackendFeatures : CachingBackendFeatures
        {
            private readonly RedisCachingBackend parent;

            /// <summary>
            /// Initializes a new <see cref="RedisCachingBackendFeatures"/>.
            /// </summary>
            /// <param name="parent">The parent <see cref="RedisCachingBackend"/>.</param>
            public RedisCachingBackendFeatures( [Required] RedisCachingBackend parent )
            {
                this.parent = parent;
            }

            /// <inheritdoc />
            public override bool Events => this.parent.notificationQueue != null;

            /// <inheritdoc />
            public override bool Clear => false;

            /// <inheritdoc />
            public override bool Dependencies => false;

            /// <inheritdoc />
            public override bool ContainsDependency => false;
        }
    }
}
