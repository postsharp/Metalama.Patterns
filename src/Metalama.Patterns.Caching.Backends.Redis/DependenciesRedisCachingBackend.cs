// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal sealed class DependenciesRedisCachingBackend : RedisCachingBackend
{
    private const char _dependenciesSeparator = '\n';
    private const int _itemVersionIndex = 0;
    private const int _itemValueIndex = 1;
    private const int _itemVersionInDependenciesIndex = 0;
    private const int _firstDependencyIndex = 1;
    private const string _noDependencyVersion = "-";
    private const string _tooManyGetItemAttemptsErrorMessage = "Too many get-item attempts.";
    internal const string TooManyTransactionAttemptsErrorMessage = "Too many transaction attempts.";

    private const string _dependencyInvalidatedEvent = "dependency";
    private const string _itemInvalidatedEvent = "item-invalidated";

    private static readonly string _dependenciesSeparatorString = new( _dependenciesSeparator, 1 );

    internal DependenciesRedisCachingBackend( [Required] IConnectionMultiplexer connection, [Required] RedisCachingBackendConfiguration configuration )
        : base( connection, configuration ) { }

    internal DependenciesRedisCachingBackend(
        IConnectionMultiplexer connection,
        IDatabase database,
        RedisKeyBuilder keyBuilder,
        RedisCachingBackendConfiguration configuration )
        : base( connection, database, keyBuilder, configuration ) { }

    protected override CachingBackendFeatures CreateFeatures() => new DependenciesRedisCachingBackendFeatures( this );

    private string[]? GetDependencies( string key, ITransaction? transaction = null )
    {
        var dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
        string dependencies = this.Database.StringGet( dependenciesKey );

        transaction?.AddCondition(
            dependencies == null 
                ? Condition.KeyNotExists( dependenciesKey ) 
                : Condition.StringEqual( dependenciesKey, dependencies ) );

        return dependencies?.Split( _dependenciesSeparator );
    }

    internal async Task<string[]?> GetDependenciesAsync( string key, ITransaction? transaction = null )
    {
        var dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
        string dependencies = await this.Database.StringGetAsync( dependenciesKey );

        transaction?.AddCondition(
            dependencies == null 
                ? Condition.KeyNotExists( dependenciesKey ) 
                : Condition.StringEqual( dependenciesKey, dependencies ) );

        return dependencies?.Split( _dependenciesSeparator );
    }

    protected override bool ProcessEvent( string kind, string key, Guid sourceId )
    {
        if ( base.ProcessEvent( kind, key, sourceId ) )
        {
            return true;
        }

        switch ( kind )
        {
            case _dependencyInvalidatedEvent:
                this.OnDependencyInvalidated( key, sourceId );

                return true;

            case _itemInvalidatedEvent:
                this.OnItemRemoved( key, CacheItemRemovedReason.Invalidated, sourceId );

                return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override async Task DeleteItemAsync( string key )
    {
        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var transaction = this.Database.CreateTransaction();
            var dependencies = await this.GetDependenciesAsync( key, transaction );
            this.DeleteItemTransaction( key, dependencies, transaction );

            if ( await transaction.ExecuteAsync() )
            {
                return;
            }
            else
            {
                this.LogSource.Debug.Write( Formatted( "Transaction DeleteItem failed. Retrying." ) );
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    private void LogRetryTransaction( [CallerMemberName] string? memberName = null )
    {
        this.LogSource.Debug.Write( Formatted( "Transaction {Method} failed. Retrying.", memberName ) );
    }

    /// <inheritdoc />
    protected override void DeleteItem( string key )
    {
        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var transaction = this.Database.CreateTransaction();
            var dependencies = this.GetDependencies( key, transaction );
            this.DeleteItemTransaction( key, dependencies, transaction );

            if ( transaction.Execute() )
            {
                return;
            }
            else
            {
                this.LogRetryTransaction();
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    private void DeleteItemTransaction( string key, string[]? dependencies, ITransaction transaction )
    {
        var dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
        var valueKey = this.KeyBuilder.GetValueKey( key );

        if ( dependencies is { Length: > _firstDependencyIndex } )
        {
            this.RemoveDependenciesTransaction( key, dependencies, transaction );
        }

#pragma warning disable 4014

        this.LogSource.Debug.Write( Formatted( "KeyDelete({Key})", dependenciesKey ) );
        transaction.KeyDeleteAsync( dependenciesKey );
        this.LogSource.Debug.Write( Formatted( "KeyDelete({Key})", valueKey ) );
        transaction.KeyDeleteAsync( valueKey );

#pragma warning restore 4014
    }

    internal void RemoveDependenciesTransaction( string key, string[]? dependencies, ITransaction transaction )
    {
        if ( dependencies != null )
        {
            for ( var i = _firstDependencyIndex; i < dependencies.Length; i++ )
            {
                this.RemoveDependencyAsync( key, dependencies[i], transaction );
            }
        }
    }

    private Task RemoveDependencyAsync( string valueKey, string dependency, IDatabaseAsync database )
    {
        var dependencyKey = this.KeyBuilder.GetDependencyKey( dependency );
        this.LogSource.Debug.Write( Formatted( "SetRemove({DependencyKey}, {ValueKey})", dependencyKey, valueKey ) );

        return database.SetRemoveAsync( dependencyKey, valueKey );
    }

    private static TimeSpan? CreateExpiry( CacheItem policy )
    {
        // TODO: make this configurable
        TimeSpan? ttl = TimeSpan.FromDays( 1 );

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

    /// <inheritdoc />
    protected override void SetItemCore( string key, CacheItem item )
    {
        var valueKey = this.KeyBuilder.GetValueKey( key );

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var transaction = this.Database.CreateTransaction();

            var version = this.Database.ListGetByIndex( valueKey, _itemVersionIndex );

            if ( version.HasValue )
            {
                // There may be a race between two threads trying to set the same key with different values.
                // By design (i.e., because we don't support distributed locks), it does not matter which thread wins.
                // However, we want to prevent corrupted entries, i.e. we want to ensure that the value/dependency/dependencies
                // keys are all consistent. This is why we're using the item versioning thing.

                this.LogSource.Debug.Write(
                    Formatted(
                        "Version of existing item is {Version}. Deleting the key {Key} under the condition that the version remains equal.",
                        version,
                        valueKey ) );

                transaction.AddCondition( Condition.ListIndexEqual( valueKey, _itemVersionIndex, version ) );
                transaction.KeyDeleteAsync( valueKey );

                if ( version != _noDependencyVersion )
                {
                    var dependencies = this.GetDependencies( key, transaction );
                    this.RemoveDependenciesTransaction( key, dependencies, transaction );
                }
            }
            else
            {
                transaction.AddCondition( Condition.KeyNotExists( valueKey ) );
            }

            this.SetItemTransaction( key, item, transaction );

            if ( transaction.Execute() )
            {
                return;
            }
            else
            {
                this.LogRetryTransaction();
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    /// <inheritdoc />
    protected override async Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        var valueKey = this.KeyBuilder.GetValueKey( key );

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var transaction = this.Database.CreateTransaction();
#pragma warning disable 4014

            var version = await this.Database.ListGetByIndexAsync( valueKey, _itemVersionIndex );

            if ( version.HasValue )
            {
                // There may be a race between two threads trying to set the same key with different values.
                // By design (i.e., because we don't support distributed locks), it does not matter which thread wins.
                // However, we want to prevent corrupted entries, i.e. we want to ensure that the value/dependency/dependencies
                // keys are all consistent. This is why we're using the item versioning thing.

                this.LogSource.Debug.Write(
                    Formatted(
                        "Version of existing item is {Version}. Deleting the key {Key} under the condition that the version remains equal.",
                        version,
                        valueKey ) );

                transaction.AddCondition( Condition.ListIndexEqual( valueKey, _itemVersionIndex, version ) );
#pragma warning disable 4014
                transaction.KeyDeleteAsync( valueKey );
#pragma warning restore 4014

                if ( version != _noDependencyVersion )
                {
                    var dependencies = this.GetDependencies( key, transaction );
                    this.RemoveDependenciesTransaction( key, dependencies, transaction );
                }
            }
            else
            {
                transaction.AddCondition( Condition.KeyNotExists( valueKey ) );
            }

            this.SetItemTransaction( key, item, transaction );

#pragma warning restore 4014

            if ( await transaction.ExecuteAsync() )
            {
                return;
            }
            else
            {
                this.LogSource.Debug.Write( Formatted( "Transaction SetItem failed. Retrying." ) );
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    private void SetItemTransaction( string key, CacheItem item, ITransaction transaction )
    {
        // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
        var value = this.CreateRedisValue( item );

        var valueKey = this.KeyBuilder.GetValueKey( key );

        var expiry = CreateExpiry( item );

        string version;

        if ( item.Dependencies == null )
        {
            version = _noDependencyVersion;
        }
        else
        {
            // Store dependencies.
            foreach ( var dependency in item.Dependencies )
            {
                var dependencyKey = this.KeyBuilder.GetDependencyKey( dependency );
                transaction.SetAddAsync( dependencyKey, key );
            }

            // Generate unique identifier of the value
            version = Guid.NewGuid().ToString();
            var dependenciesString = version + _dependenciesSeparator + string.Join( _dependenciesSeparatorString, item.Dependencies );
            transaction.StringSetAsync( this.KeyBuilder.GetDependenciesKey( key ), dependenciesString );
        }

        // Store the item itself.
        transaction.ListRightPushAsync( valueKey, new RedisValue[] { version, value } );
        transaction.KeyExpireAsync( valueKey, expiry );
    }

    /// <inheritdoc />
    protected override CacheValue? GetItemCore( string key, bool includeDependencies )
    {
        var valueKey = this.KeyBuilder.GetValueKey( key );
        RedisValue[] itemList;
        string[]? dependencies = null;

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            itemList = this.Database.ListRange( valueKey );

            if ( itemList == null || itemList.Length == 0 )
            {
                return null;
            }

            if ( !includeDependencies || itemList[_itemVersionIndex] == _noDependencyVersion )
            {
                goto itemGotten;
            }

            dependencies = this.GetDependencies( key );

            if ( dependencies != null && dependencies[_itemVersionInDependenciesIndex] == (string) itemList[_itemVersionIndex] )
            {
                goto itemGotten;
            }
        }

        throw new CachingException( _tooManyGetItemAttemptsErrorMessage );

    itemGotten:

        var cacheValue = this.GetCacheValue( valueKey, itemList[_itemValueIndex] );

        if ( dependencies == null )
        {
            return new CacheValue( cacheValue );
        }
        else
        {
            return new CacheValue( cacheValue, ImmutableArray.Create( dependencies, _firstDependencyIndex, dependencies.Length - 1 ) );
        }
    }

    /// <inheritdoc />
    protected override async Task<CacheValue?> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
    {
        var valueKey = this.KeyBuilder.GetValueKey( key );
        RedisValue[] itemList;
        string[]? dependencies = null;

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            itemList = await this.Database.ListRangeAsync( valueKey );

            if ( itemList == null || itemList.Length == 0 )
            {
                return null;
            }

            if ( !includeDependencies || itemList[_itemVersionIndex] == _noDependencyVersion )
            {
                goto itemGotten;
            }

            dependencies = await this.GetDependenciesAsync( key );

            if ( dependencies != null && dependencies[_itemVersionInDependenciesIndex] == (string) itemList[_itemVersionIndex] )
            {
                goto itemGotten;
            }
        }

        throw new CachingException( _tooManyGetItemAttemptsErrorMessage );

    itemGotten:

        var cacheValue = this.GetCacheValue( valueKey, itemList[_itemValueIndex] );

        if ( dependencies == null )
        {
            return new CacheValue( cacheValue );
        }
        else
        {
            return new CacheValue( cacheValue, ImmutableArray.Create( dependencies, _firstDependencyIndex, dependencies.Length - 1 ) );
        }
    }

    /// <inheritdoc />
    protected override bool ContainsDependencyCore( string key )
    {
        return this.Database.KeyExists( this.KeyBuilder.GetDependencyKey( key ) );
    }

    /// <inheritdoc />
    protected override Task<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        return this.Database.KeyExistsAsync( this.KeyBuilder.GetDependencyKey( key ) );
    }

    /// <inheritdoc />
    protected override async Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        var dependencyKey = this.KeyBuilder.GetDependencyKey( key );

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var dependentItemKeys = await this.Database.SetMembersAsync( dependencyKey );

            if ( dependentItemKeys is not { Length: > 0 } )
            {
                // There is nothing to invalidate.
                return;
            }

            var transaction = this.Database.CreateTransaction();

            foreach ( var dependentItemKey in dependentItemKeys )
            {
                var dependencies = await this.GetDependenciesAsync( dependentItemKey, transaction );
                this.DeleteItemTransaction( dependentItemKey, dependencies, transaction );
            }

            if ( await transaction.ExecuteAsync() )
            {
                // Send notifications.
                await this.SendEventAsync( _dependencyInvalidatedEvent, key );

                foreach ( var dependentItemKey in dependentItemKeys )
                {
                    await this.SendEventAsync( _itemInvalidatedEvent, dependentItemKey );
                }

                return;
            }
            else
            {
                this.LogSource.Debug.Write( Formatted( "Transaction InvalidateDependency failed. Retrying." ) );
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    protected override void InvalidateDependencyCore( string key )
    {
        var dependencyKey = this.KeyBuilder.GetDependencyKey( key );

        for ( var attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
        {
            var dependentItemKeys = this.Database.SetMembers( dependencyKey );

            if ( dependentItemKeys is not { Length: > 0 } )
            {
                // There is nothing to invalidate.
                return;
            }

            var transaction = this.Database.CreateTransaction();

            foreach ( var dependentItemKey in dependentItemKeys )
            {
                var dependencies = this.GetDependencies( dependentItemKey, transaction );
                this.DeleteItemTransaction( dependentItemKey, dependencies, transaction );
            }

            if ( transaction.Execute() )
            {
                // Send notifications.
                this.SendEvent( _dependencyInvalidatedEvent, key );

                foreach ( var dependentItemKey in dependentItemKeys )
                {
                    this.SendEvent( _itemInvalidatedEvent, dependentItemKey );
                }

                return;
            }
            else
            {
                this.LogRetryTransaction();
            }
        }

        throw new CachingException( TooManyTransactionAttemptsErrorMessage );
    }

    public Task CleanUpAsync( CancellationToken cancellationToken = default )
    {
        var servers = this.Connection.GetEndPoints().Select( endpoint => this.Connection.GetServer( endpoint ) );

        return Task.WhenAll( servers.Select( s => this.CleanUpAsync( s, cancellationToken ) ) );
    }

    public Task CleanUpAsync( IServer server, CancellationToken cancellationToken = default )
    {
        List<Task> tasks = new();

        foreach ( var key in server.Keys( this.Database.Database, this.KeyBuilder.KeyPrefix + ":*", flags: CommandFlags.PreferSlave ) )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var tokenizer = new StringTokenizer( key );

            var keyPrefix = tokenizer.GetNext();

            if ( keyPrefix != this.KeyBuilder.KeyPrefix )
            {
                this.LogSource.Warning.Write( Formatted( "The key {Key} has an invalid prefix. Redis should not have returned it. Ignoring it.", keyPrefix ) );

                continue;
            }

            var keyKind = tokenizer.GetNext();

            if ( keyKind == RedisKeyBuilder.GarbageCollectionPrefix )
            {
                continue;
            }

            var smallKey = tokenizer.GetRest();

            switch ( keyKind )
            {
                case RedisKeyBuilder.ValueKindPrefix:
                    tasks.Add( this.CleanUpValue( key, smallKey ) );

                    break;

                case RedisKeyBuilder.DependenciesKindPrefix:
                    tasks.Add( this.CleanUpDependencies( smallKey ) );

                    break;

                case RedisKeyBuilder.DependencyKindPrefix:
                    tasks.Add( this.CleanUpDependency( smallKey ) );

                    break;
            }
        }

        return Task.WhenAll( tasks );
    }

    private async Task CleanUpDependency( string key )
    {
        var items = await this.Database.SetMembersAsync( this.KeyBuilder.GetDependencyKey( key ) );

        foreach ( var item in items )
        {
            if ( !await this.Database.KeyExistsAsync( this.KeyBuilder.GetValueKey( item ) ) )
            {
                this.LogSource.Warning.Write( Formatted( "The dependency key {Key} does not have the corresponding dependencies key. Deleting it.", key ) );

                await this.RemoveDependencyAsync( item, key, this.Database );
            }
        }
    }

    private async Task CleanUpDependencies( string key )
    {
        if ( !await this.Database.KeyExistsAsync( this.KeyBuilder.GetValueKey( key ) ) )
        {
            this.LogSource.Warning.Write( Formatted( "The dependencies key {Key} does not have the corresponding value key. Deleting it.", key ) );

            await this.DeleteItemAsync( key );
        }
    }

    private async Task CleanUpValue( RedisKey redisKey, string smallKey )
    {
        string itemVersion = this.Database.ListGetByIndex( redisKey, _itemVersionIndex, CommandFlags.PreferSlave );
        var dependencies = await this.GetDependenciesAsync( smallKey );

        if ( dependencies == null )
        {
            if ( itemVersion == _noDependencyVersion )
            {
                return;
            }

            this.LogSource.Warning.Write( Formatted( "The value key {Key} does not have the corresponding dependencies key. Deleting it.", smallKey ) );

            await this.Database.KeyDeleteAsync( redisKey );

            return;
        }

        if ( itemVersion != dependencies[_itemVersionInDependenciesIndex] )
        {
            this.LogSource.Warning.Write(
                Formatted(
                    "The value {Key} version and the corresponding dependencies version differ. Deleting both.",
                    smallKey ) );

            for ( var i = _firstDependencyIndex; i < dependencies.Length; i++ )
            {
                await this.RemoveDependencyAsync( smallKey, dependencies[i], this.Database );
            }

            await this.Database.KeyDeleteAsync( this.KeyBuilder.GetDependenciesKey( smallKey ) );
            await this.Database.KeyDeleteAsync( redisKey );

            return;
        }

        for ( var i = _firstDependencyIndex; i < dependencies.Length; i++ )
        {
            if ( !await this.Database.SetContainsAsync( this.KeyBuilder.GetDependencyKey( dependencies[i] ), smallKey ) )
            {
                this.LogSource.Warning.Write(
                    Formatted(
                        "The value key {Key} does not have the corresponding dependency key {Dependency}. Deleting it.",
                        smallKey,
                        dependencies[i] ) );

                await this.DeleteItemAsync( smallKey );
            }
        }
    }

    private sealed class DependenciesRedisCachingBackendFeatures : RedisCachingBackendFeatures
    {
        public DependenciesRedisCachingBackendFeatures( DependenciesRedisCachingBackend parent )
            : base( parent ) { }

        public override bool Dependencies => true;

        public override bool ContainsDependency => true;
    }
}