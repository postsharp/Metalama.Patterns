// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Collections.Immutable;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Removes dependencies added when a <see cref="RedisCachingBackend"/> when items are expired or evicted from the cache.
/// At least one instance (ideally a single instance) of the <see cref="RedisCacheDependencyGarbageCollector"/> must be running whenever a
/// <see cref="RedisCachingBackend"/> instance that supports dependencies is running, otherwise the cache will use storage to store dependencies
/// that are no longer relevant, and will not be removed otherwise. If no <see cref="RedisCacheDependencyGarbageCollector"/> is running while
/// at least one dependency-enabled <see cref="RedisCachingBackend"/> instance is running, you must initiate full garbage collection
/// by calling the <see cref="PerformFullCollectionAsync(RedisCachingBackend,CancellationToken)"/> method.
/// </summary>
[PublicAPI] // Comments above indicate use case.
public sealed class RedisCacheDependencyGarbageCollector : ITestableCachingComponent
{
    private readonly FlashtraceSource _logger;
    private RedisKeyBuilder _keyBuilder = null!; // "Guaranteed" to be initialized via Init et al.

    internal RedisNotificationQueue NotificationQueue { get; set; } = null!; // "Guaranteed" to be initialized via Init et al.

    private readonly bool _ownsBackend;
    private readonly DependenciesRedisCachingBackend _backend;

    private RedisCacheDependencyGarbageCollector(
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration configuration,
        IServiceProvider? serviceProvider )
    {
        this.ServiceProvider = serviceProvider;
        this.Connection = connection;
        this.Database = this.Connection.GetDatabase( configuration.Database );
        this._keyBuilder = new RedisKeyBuilder( this.Database, configuration );
        this._backend = new DependenciesRedisCachingBackend( connection, this.Database, this._keyBuilder, configuration, serviceProvider );
        this._ownsBackend = true;
        this._logger = serviceProvider.GetFlashtraceSource( this.GetType(), FlashtraceRole.Caching );
    }

    private RedisCacheDependencyGarbageCollector( DependenciesRedisCachingBackend backend )
    {
        this.Connection = backend.Connection;
        this.Database = backend.Database;
        this._backend = backend;
        this._ownsBackend = false;
        this._logger = backend.ServiceProvider.GetFlashtraceSource( this.GetType(), FlashtraceRole.Caching );
    }

    /// <summary>
    /// Creates a new <see cref="RedisCacheDependencyGarbageCollector"/> given a Redis connection and a configuration object.
    /// </summary>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="configuration">A configuration object.</param>
    /// <returns>A <see cref="RedisCacheDependencyGarbageCollector"/> using <paramref name="connection"/> and <paramref name="configuration"/>.</returns>
    public static RedisCacheDependencyGarbageCollector Create(
        IConnectionMultiplexer connection,
        IServiceProvider? serviceProvider = null,
        RedisCachingBackendConfiguration? configuration = null )
    {
        configuration ??= new RedisCachingBackendConfiguration();
        var collector = new RedisCacheDependencyGarbageCollector( connection, configuration, serviceProvider );
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
                throw new ArgumentException( "The backend is not a Redis backend.", nameof(backend) );
        }
    }

    /// <summary>
    /// Creates a new <see cref="RedisCacheDependencyGarbageCollector"/> that uses an existing <see cref="DependenciesRedisCachingBackend"/> object.
    /// </summary>
    /// <param name="backend">An existing Redis <see cref="CachingBackend"/>, as returned by <see cref="RedisCachingBackend.Create"/>.</param>
    /// <returns>A <see cref="RedisCacheDependencyGarbageCollector"/> using <paramref name="backend"/>.</returns>
    public static RedisCacheDependencyGarbageCollector Create( CachingBackend backend )
    {
        var redisCachingBackend = FindRedisCachingBackend( backend );

        if ( !redisCachingBackend.SupportedFeatures.Dependencies )
        {
            throw new ArgumentException( "This backend does not support dependencies.", nameof(backend) );
        }

        var collector = new RedisCacheDependencyGarbageCollector( (DependenciesRedisCachingBackend) redisCachingBackend );
        collector.Init( redisCachingBackend.Configuration );

        return collector;
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="RedisCacheDependencyGarbageCollector"/> given a Redis connection and a configuration object.
    /// </summary>
    /// <param name="connection">A Redis connection.</param>
    /// <param name="configuration">A configuration object.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> returning a <see cref="RedisCacheDependencyGarbageCollector"/> that uses <paramref name="connection"/> and <paramref name="configuration"/>.</returns>
    public static Task<RedisCacheDependencyGarbageCollector> CreateAsync(
        IConnectionMultiplexer connection,
        RedisCachingBackendConfiguration configuration,
        IServiceProvider? serviceProvider,
        CancellationToken cancellationToken = default )
    {
        var collector = new RedisCacheDependencyGarbageCollector( connection, configuration, serviceProvider );

        return collector.InitAsync( configuration, cancellationToken );
    }

    /// <summary>
    /// Asynchronously creates a new <see cref="RedisCacheDependencyGarbageCollector"/> that uses an existing <see cref="RedisCachingBackend"/> object.
    /// </summary>
    /// <param name="backend">An existing <see cref="CachingBackend"/>, as returned by <see cref="RedisCachingBackend.Create"/>, that supports dependencies.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/> returning a <see cref="RedisCacheDependencyGarbageCollector"/> that uses <paramref name="backend"/>.</returns>
    public static Task<RedisCacheDependencyGarbageCollector> CreateAsync( CachingBackend backend, CancellationToken cancellationToken = default )
    {
        var redisCachingBackend = FindRedisCachingBackend( backend );

        if ( !redisCachingBackend.SupportedFeatures.Dependencies )
        {
            throw new ArgumentException( "This backend does not support dependencies.", nameof(backend) );
        }

        var collector = new RedisCacheDependencyGarbageCollector( (DependenciesRedisCachingBackend) backend );

        return collector.InitAsync( redisCachingBackend.Configuration, cancellationToken );
    }

    private void Init( RedisCachingBackendConfiguration configuration )
    {
        this.InitCommon( configuration );

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        this.NotificationQueue = RedisNotificationQueue.Create(
            this.ToString()!,
            this.Connection,
            ImmutableArray.Create( this._keyBuilder.NotificationChannel ),
            this.ProcessKeyspaceNotification,
            configuration.ConnectionTimeout,
            this.ServiceProvider );
    }

    private async Task<RedisCacheDependencyGarbageCollector> InitAsync(
        RedisCachingBackendConfiguration configuration,
        CancellationToken cancellationToken )
    {
        this.InitCommon( configuration );

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        this.NotificationQueue = await RedisNotificationQueue.CreateAsync(
            this.ToString()!,
            this.Connection,
            ImmutableArray.Create( this._keyBuilder.NotificationChannel ),
            this.ProcessKeyspaceNotification,
            configuration.ConnectionTimeout,
            this.ServiceProvider,
            cancellationToken );

        return this;
    }

    private void InitCommon( RedisCachingBackendConfiguration configuration ) => this._keyBuilder = new RedisKeyBuilder( this.Database, configuration );

    /// <summary>
    /// Gets the Redis <see cref="IDatabase"/> used by the current object.
    /// </summary>
    public IDatabase Database { get; }

    public IServiceProvider? ServiceProvider { get; }

    /// <summary>
    /// Gets the Redis <see cref="IConnectionMultiplexer"/> used by the current object.
    /// </summary>
    public IConnectionMultiplexer Connection { get; }

    private void ProcessKeyspaceNotification( RedisNotification notification )
    {
        string channelName = notification.Channel;

        var tokenizer = new StringTokenizer( channelName );

        // Was: `if ( tokenizer.GetNext() == null ) return;` - However, GetNext() never returns null, but does have side effects, so using discard.   
        _ = tokenizer.GetNext( ':' );

        var prefix = tokenizer.GetNext( ':' );

        if ( !prefix.Equals( this._keyBuilder.KeyPrefix.AsSpan(), StringComparison.Ordinal ) )
        {
            return;
        }

        var keyKind = tokenizer.GetNext( ':' );

        var itemKey = tokenizer.GetRemainder().ToString();

        switch ( keyKind )
        {
            case RedisKeyBuilder.DependenciesKindPrefix:
                // When a dependencies key is removed by Redis (for whatever reason not under our control),
                // we need to remove the main key as well.
                switch ( notification.Value )
                {
                    case "expired":
                    case "evicted":
                        if ( this.Database.KeyDelete( this._keyBuilder.GetValueKey( itemKey ) ) )
                        {
                            // ReSharper disable once StringLiteralTypo
                            this._logger.Warning.Write(
                                Formatted(
                                    "The dependencies key for item {Item} has been {State} but should not. The Redis server is probably misconfigured. " +
                                    "Only use volatile-* maxmemory policies.",
                                    itemKey,
                                    notification.Value ) );
                        }

                        // TODO: remove the main key from other dependencies.
                        break;
                }

                break;

            case RedisKeyBuilder.DependencyKindPrefix:
                // No idea of what to do with this situation.
                switch ( notification.Value )
                {
                    case "expired":
                    case "evicted":
                        // ReSharper disable once StringLiteralTypo
                        this._logger.Warning.Write(
                            Formatted(
                                "The dependency key {Key} has been {State} but should not. The Redis server is probably misconfigured. " +
                                "Only use volatile-* maxmemory policies.",
                                itemKey,
                                notification.Value ) );

                        break;
                }

                break;

            case RedisKeyBuilder.ValueKindPrefix:
                this._logger.Debug.IfEnabled?.Write( Formatted( "Enqueue processing of cache eviction." ) );
                this._backend.ExecuteNonBlockingTask( () => this.OnValueEvictedAsync( itemKey ) );

                break;

            default:
                this._logger.Debug.IfEnabled?.Write( Formatted( "Notification ignored." ) );

                break;
        }
    }

    private async Task OnValueEvictedAsync( string key )
    {
        string valueKey = this._keyBuilder.GetValueKey( key );
        string dependenciesKey = this._keyBuilder.GetDependenciesKey( key );

        for ( var attempt = 0; attempt < this._backend.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var valueKeyExistsTask = this.Database.KeyExistsAsync( valueKey );
            var dependenciesKeyExistsTask = this.Database.KeyExistsAsync( dependenciesKey );

            await Task.WhenAll( valueKeyExistsTask, dependenciesKeyExistsTask );

            // This condition repeats in the following transaction.
            // It is more expensive to set up the transaction and we know that just one client out of all clients
            // connected will be able to execute the transaction, so we stop all the other clients here.
            if (

                // The value has been set again in the meanwhile
                valueKeyExistsTask.Result

                // The garbage collection has been performed by another client already
                || !dependenciesKeyExistsTask.Result )
            {
                return;
            }

            var transaction = this.Database.CreateTransaction();
            transaction.AddCondition( Condition.KeyNotExists( valueKey ) );
            transaction.AddCondition( Condition.KeyExists( dependenciesKey ) );
            var dependencies = await this._backend.GetDependenciesAsync( key, transaction );
            this._backend.RemoveDependenciesTransaction( key, dependencies, transaction );
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

        if ( this._ownsBackend )
        {
            this._backend.Dispose();
        }
    }

    /// <summary>
    /// Asynchronously disposes the current object.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask DisposeAsync( CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await this.NotificationQueue.DisposeAsync( cancellationToken );

        cancellationToken.ThrowIfCancellationRequested();

        if ( this._ownsBackend )
        {
            await this._backend.DisposeAsync( cancellationToken );
        }
    }

    ValueTask IAsyncDisposable.DisposeAsync() => this.DisposeAsync();

    /// <summary>
    /// Performs a full garbage collection on all Redis servers. This operation enumerates and validates all keys in the database, and can possibly last several
    /// minutes and affect performance in production.
    /// </summary>
    /// <param name="backend">A Redis <see cref="CachingBackend"/> that supports dependencies.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public static Task PerformFullCollectionAsync( CachingBackend backend, CancellationToken cancellationToken = default )
    {
        if ( backend is not DependenciesRedisCachingBackend dependenciesRedisCachingBackend )
        {
            throw new ArgumentOutOfRangeException( nameof(backend), "The back-end is not a Redis backend supporting dependencies." );
        }

        return dependenciesRedisCachingBackend.CleanUpAsync( cancellationToken );
    }

    /// <summary>
    /// Performs a full garbage collection on a given Redis server. This operation enumerates and validates all keys in the database, and can possibly last several
    /// minutes and affect performance in production.
    /// </summary>
    /// <param name="backend">A <see cref="RedisCachingBackend"/> that supports dependencies.</param>
    /// <param name="server">The Redis server whose keys will be enumerated and validated.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public static Task PerformFullCollectionAsync(
        CachingBackend backend,
        IServer server,
        CancellationToken cancellationToken = default )
    {
        if ( backend is not DependenciesRedisCachingBackend dependenciesRedisCachingBackend )
        {
            throw new ArgumentOutOfRangeException( nameof(backend), "The back-end is not a Redis backend supporting dependencies." );
        }

        return dependenciesRedisCachingBackend.CleanUpAsync( server, cancellationToken );
    }

    internal int BackgroundTaskExceptions => this.NotificationQueue.BackgroundTaskExceptions;

    int ITestableCachingComponent.BackgroundTaskExceptions => this.BackgroundTaskExceptions;
}