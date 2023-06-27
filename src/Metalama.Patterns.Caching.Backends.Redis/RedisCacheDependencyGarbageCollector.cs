// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Utilities;
using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder;
using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Backends.Redis
{
    /// <summary>
    /// Removes dependencies added when a <see cref="RedisCachingBackend"/> when items are expired or evicted from the cache.
    /// At least one instance (ideally a single instance) of the <see cref="RedisCacheDependencyGarbageCollector"/> must be running whenever a
    /// <see cref="RedisCachingBackend"/> instance that supports dependencies is running, otherwise the cache will use storage to store dependencies
    /// that are no longer relevant, and will not be removed otherwise. If no <see cref="RedisCacheDependencyGarbageCollector"/> is running while
    /// at least one dependency-enabled <see cref="RedisCachingBackend"/> instance is running, you must initiate full garbage collection
    /// by calling the <see cref="PerformFullCollectionAsync(RedisCachingBackend,CancellationToken)"/> method.
    /// </summary>
    public sealed class RedisCacheDependencyGarbageCollector : ITestableCachingComponent
    {
        private RedisKeyBuilder keyBuilder;
        private readonly LogSource logger = LogSourceFactory.ForRole3( LoggingRoles.Caching ).GetLogSource( typeof(RedisCacheDependencyGarbageCollector) );
        internal RedisNotificationQueue NotificationQueue { get; private set; }

        private readonly bool ownsBackend;
        private readonly DependenciesRedisCachingBackend backend;

        private RedisCacheDependencyGarbageCollector( IConnectionMultiplexer connection, RedisCachingBackendConfiguration configuration )
        {
            this.Connection = connection;
            this.Database = this.Connection.GetDatabase( configuration?.Database ?? -1 );
            this.keyBuilder = new RedisKeyBuilder( this.Database, configuration );
            this.backend = new DependenciesRedisCachingBackend( connection, this.Database, this.keyBuilder, configuration );
            this.ownsBackend = true;

        }

        private RedisCacheDependencyGarbageCollector(DependenciesRedisCachingBackend backend) 
        {
            this.Connection = backend.Connection;
            this.Database = backend.Database;
            this.backend = backend;
            this.ownsBackend = false;
        }
        

        /// <summary>
        /// Creates a new <see cref="RedisCacheDependencyGarbageCollector"/> given a Redis connection and a configuration object.
        /// </summary>
        /// <param name="connection">A Redis connection.</param>
        /// <param name="configuration">A configuration object.</param>
        /// <returns>A <see cref="RedisCacheDependencyGarbageCollector"/> using <paramref name="connection"/> and <paramref name="configuration"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static RedisCacheDependencyGarbageCollector Create([Required] IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration)
        {
            RedisCacheDependencyGarbageCollector collector = new RedisCacheDependencyGarbageCollector( connection, configuration );
            collector.Init( configuration );
            return collector;
        }

        private static RedisCachingBackend FindRedisCachingBackend( CachingBackend backend )
        {
            switch ( backend )
            {
                case RedisCachingBackend redisCachingBackend:
                    return redisCachingBackend;
                case CachingBackendEnhancer enhancer:
                    return FindRedisCachingBackend( enhancer.UnderlyingBackend );
                default:
                    throw new ArgumentException("The backend is not a Redis backend.", nameof( backend ));
            }
        }

        /// <summary>
        /// Creates a new <see cref="RedisCacheDependencyGarbageCollector"/> that uses an existing <see cref="DependenciesRedisCachingBackend"/> object.
        /// </summary>
        /// <param name="backend">An existing Redis <see cref="CachingBackend"/>, as returned by <see cref="RedisCachingBackend.Create"/>.</param>
        /// <returns>A <see cref="RedisCacheDependencyGarbageCollector"/> using <paramref name="backend"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static RedisCacheDependencyGarbageCollector Create([Required] CachingBackend backend)
        {
            RedisCachingBackend redisCachingBackend = FindRedisCachingBackend( backend );


            if ( !redisCachingBackend.SupportedFeatures.Dependencies )
            {
                throw new ArgumentException("This backend does not support dependencies.", nameof(backend));
            }

            RedisCacheDependencyGarbageCollector collector = new RedisCacheDependencyGarbageCollector( (DependenciesRedisCachingBackend)redisCachingBackend);
            collector.Init(redisCachingBackend.Configuration );
            return collector;
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="RedisCacheDependencyGarbageCollector"/> given a Redis connection and a configuration object.
        /// </summary>
        /// <param name="connection">A Redis connection.</param>
        /// <param name="configuration">A configuration object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> returning a <see cref="RedisCacheDependencyGarbageCollector"/> that uses <paramref name="connection"/> and <paramref name="configuration"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static Task<RedisCacheDependencyGarbageCollector> CreateAsync([Required] IConnectionMultiplexer connection, RedisCachingBackendConfiguration configuration, CancellationToken cancellationToken = default)
        {
            RedisCacheDependencyGarbageCollector collector = new RedisCacheDependencyGarbageCollector( connection, configuration );
            return collector.InitAsync( configuration, cancellationToken );
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="RedisCacheDependencyGarbageCollector"/> that uses an existing <see cref="RedisCachingBackend"/> object.
        /// </summary>
        /// <param name="backend">An existing <see cref="CachingBackend"/>, as returned by <see cref="RedisCachingBackend.Create"/>, that supports dependencies.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> returning a <see cref="RedisCacheDependencyGarbageCollector"/> that uses <paramref name="backend"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static Task<RedisCacheDependencyGarbageCollector> CreateAsync([Required] CachingBackend backend, CancellationToken cancellationToken = default)
        {
            RedisCachingBackend redisCachingBackend = FindRedisCachingBackend(backend);


            if (!redisCachingBackend.SupportedFeatures.Dependencies)
            {
                throw new ArgumentException("This backend does not support dependencies.", nameof(backend));
            }

            RedisCacheDependencyGarbageCollector collector = new RedisCacheDependencyGarbageCollector( (DependenciesRedisCachingBackend) backend );
            return collector.InitAsync(redisCachingBackend.Configuration, cancellationToken );
        }


        private void Init( RedisCachingBackendConfiguration configuration )
        {
            this.InitCommon( configuration );

            this.NotificationQueue = RedisNotificationQueue.Create( this.ToString(), this.Connection,
                                                                    ImmutableArray.Create( this.keyBuilder.NotificationChannel ),
                                                                    this.ProcessKeyspaceNotification, configuration.ConnectionTimeout );

        }

        private async Task<RedisCacheDependencyGarbageCollector> InitAsync(RedisCachingBackendConfiguration configuration, CancellationToken cancellationToken)
        {
            this.InitCommon( configuration );

            this.NotificationQueue = await RedisNotificationQueue.CreateAsync(this.ToString(), this.Connection, ImmutableArray.Create(this.keyBuilder.NotificationChannel), this.ProcessKeyspaceNotification, configuration.ConnectionTimeout, cancellationToken);


            return this;
        }

        private void InitCommon( RedisCachingBackendConfiguration configuration )
        {
            this.keyBuilder = new RedisKeyBuilder( this.Database, configuration );
        }

        /// <summary>
        /// Gets the Redis <see cref="IDatabase"/> used by the current object.
        /// </summary>
        public IDatabase Database { get; }

        /// <summary>
        /// Gets the Redis <see cref="IConnectionMultiplexer"/> used by the current object.
        /// </summary>
        public IConnectionMultiplexer Connection { get; }


        private void ProcessKeyspaceNotification(RedisNotification notification)
        {
            string channelName = notification.Channel;

            StringTokenizer tokenizer = new StringTokenizer(channelName);

            if (tokenizer.GetNext() == null)
                return;

            string prefix = tokenizer.GetNext();
            if (prefix != this.keyBuilder.KeyPrefix)
                return;

            string keyKind = tokenizer.GetNext();

            string itemKey = tokenizer.GetRest();

            switch (keyKind)
            {
                case RedisKeyBuilder.DependenciesKindPrefix:
                    // When a dependencies key is removed by Redis (for whatever reason not under our control),
                    // we need to remove the main key as well.
                    switch (notification.Value)
                    {
                        case "expired":
                        case "evicted":
                            if (this.Database.KeyDelete(this.keyBuilder.GetValueKey(itemKey)))
                            {
                                this.logger.Warning.Write( 
                                                   Formatted("The dependencies key for item {Item} has been {State} but should not. The Redis server is probably misconfigured. " +
                                                   "Only use volatile-* maxmemory policies.", itemKey, notification.Value ));

                            }
                            // TODO: remove the main key from other dependencies.
                            break;
                    }
                    break;

                case RedisKeyBuilder.DependencyKindPrefix:
                    // No idea of what to do with this situation.
                    switch (notification.Value)
                    {
                        case "expired":
                        case "evicted":
                            this.logger.Warning.Write( Formatted(  
                                               "The dependency key {Key} has been {State} but should not. The Redis server is probably misconfigured. " +
                                               "Only use volatile-* maxmemory policies.", itemKey, notification.Value ) );
                            break;
                    }
                    break;

                case RedisKeyBuilder.ValueKindPrefix:
                    this.logger.Debug.EnabledOrNull?.Write( Formatted(  "Enqueue processing of cache eviction." ) );
                    this.backend.ExecuteNonBlockingTask(() => this.OnValueEvictedAsync(itemKey));
                    break;

                default:
                    this.logger.Debug.EnabledOrNull?.Write( Formatted( "Notification ignored." ) );
                    break;
            }
        }

        private async Task OnValueEvictedAsync( string key )
        {
            string valueKey = this.keyBuilder.GetValueKey( key );
            string dependenciesKey = this.keyBuilder.GetDependenciesKey( key );

            for ( int attempt = 0; attempt < this.backend.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                Task<bool> valueKeyExistsTask = this.Database.KeyExistsAsync( valueKey );
                Task<bool> dependenciesKeyExistsTask = this.Database.KeyExistsAsync( dependenciesKey );

                await Task.WhenAll( valueKeyExistsTask, dependenciesKeyExistsTask );

                // This condition repeats in the following transaction.
                // It is more expensive to set up the transaction and we know that just one client out of all clients
                // connected will be able to execute the transaction, so we stop all the other clients here.
                if (
                    // The value has been set again in the meanwhile
                    valueKeyExistsTask.Result
                    // The garabage collection has been performed by another client already
                    || !dependenciesKeyExistsTask.Result )
                {
                    return;
                }

                ITransaction transaction = this.Database.CreateTransaction();
                transaction.AddCondition( Condition.KeyNotExists( valueKey ) );
                transaction.AddCondition( Condition.KeyExists( dependenciesKey ) );
                string[] dependencies = await this.backend.GetDependenciesAsync( key, transaction );
                this.backend.RemoveDependenciesTransaction( key, dependencies, transaction );
#pragma warning disable 4014
                transaction.KeyDeleteAsync( dependenciesKey );
#pragma warning restore 4014
                if ( await transaction.ExecuteAsync() )
                {
                    return;
                }
            }

            throw new CachingException( DependenciesRedisCachingBackend.TooManyTransactionAttemptsErrorMessage );
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            this.NotificationQueue.Dispose();

            if ( this.ownsBackend )
            {
                this.backend.Dispose();
            }

        }

        /// <summary>
        /// Asynchronously disposes the current object.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task DisposeAsync( CancellationToken cancellationToken = default )
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.NotificationQueue.DisposeAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if ( this.ownsBackend )
            {
                await this.backend.DisposeAsync(cancellationToken);
            }


        }

        /// <summary>
        /// Performs a full garbage collection on all Redis servers. This operation enumerates and validates all keys in the database, and can possibly last several
        /// minutes and affect performance in production.
        /// </summary>
        /// <param name="backend">A <see cref="RedisCachingBackend"/> that supports dependencies.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task PerformFullCollectionAsync( [Required] RedisCachingBackend backend, CancellationToken cancellationToken = default )
        {
            return ((DependenciesRedisCachingBackend) backend).CleanUpAsync( cancellationToken );
        }

        /// <summary>
        /// Performs a full garbage collection on a given Redis server. This operation enumerates and validates all keys in the database, and can possibly last several
        /// minutes and affect performance in production.
        /// </summary>
        /// <param name="backend">A <see cref="RedisCachingBackend"/> that supports dependencies.</param>
        /// <param name="server">The Redis server whose keys will be enumerated and validated.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>

        public static Task PerformFullCollectionAsync( [Required] RedisCachingBackend backend, [Required] IServer server, CancellationToken cancellationToken = default )
        {
            return ((DependenciesRedisCachingBackend) backend).CleanUpAsync( server, cancellationToken );
        }

        internal int BackgroundTaskExceptions => this.NotificationQueue.BackgroundTaskExceptions;

        int ITestableCachingComponent.BackgroundTaskExceptions => this.BackgroundTaskExceptions;

   
    }
}
