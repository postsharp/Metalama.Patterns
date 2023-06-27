// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.Caching.Backends.Redis
{
    internal sealed class DependenciesRedisCachingBackend : RedisCachingBackend
    {

        private const char dependenciesSeparator = '\n';
        private static readonly string dependenciesSeparatorString = new string( dependenciesSeparator, 1 );
        private const int itemVersionIndex = 0;
        private const int itemValueIndex = 1;
        private const int itemVersionInDependenciesIndex = 0;
        private const int firstDependencyIndex = 1;
        private const string noDependencyVersion = "-";
        private const string tooManyGetItemAttemptsErrorMessage = "Too many get-item attempts.";
        internal const string TooManyTransactionAttemptsErrorMessage = "Too many transaction attempts.";

        private const string dependencyInvalidatedEvent = "dependency";
        private const string itemInvalidatedEvent = "item-invalidated";

        internal DependenciesRedisCachingBackend( IConnectionMultiplexer connection, RedisCachingBackendConfiguration configuration = null )
            : base( connection, configuration )
        {
        }

 

        internal DependenciesRedisCachingBackend( IConnectionMultiplexer connection, IDatabase database, RedisKeyBuilder keyBuilder,
                                      RedisCachingBackendConfiguration configuration )
            : base( connection, database, keyBuilder, configuration )
        {
        }

        protected override CachingBackendFeatures CreateFeatures() => new DependenciesRedisCachingBackendFeatures( this );

        private string[] GetDependencies( string key, ITransaction transaction = null )
        {
            RedisKey dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
            string dependencies = this.Database.StringGet( dependenciesKey );

            if ( transaction != null )
            {
                if ( dependencies == null )
                {
                    transaction.AddCondition( Condition.KeyNotExists( dependenciesKey ) );
                }
                else
                {
                    transaction.AddCondition( Condition.StringEqual( dependenciesKey, dependencies ) );
                }
            }

            return dependencies?.Split( dependenciesSeparator );
        }

        internal async Task<string[]> GetDependenciesAsync( string key, ITransaction transaction = null )
        {
            RedisKey dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
            string dependencies = await this.Database.StringGetAsync( dependenciesKey );

            if ( transaction != null )
            {
                if ( dependencies == null )
                {
                    transaction.AddCondition( Condition.KeyNotExists( dependenciesKey ) );
                }
                else
                {
                    transaction.AddCondition( Condition.StringEqual( dependenciesKey, dependencies ) );
                }
            }

            return dependencies?.Split( dependenciesSeparator );
        }

        protected override bool ProcessEvent( string kind, string key, Guid sourceId )
        {
            if ( base.ProcessEvent( kind, key, sourceId ) )
            {
                return true;
            }

            switch ( kind )
            {
                case dependencyInvalidatedEvent:
                    this.OnDependencyInvalidated( key, sourceId );
                    return true;

                case itemInvalidatedEvent:
                    this.OnItemRemoved( key, CacheItemRemovedReason.Invalidated, sourceId );
                    return true;

            }

            return false;
        }

        /// <inheritdoc />
        protected override async Task DeleteItemAsync( string key )
        {
            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                ITransaction transaction = this.Database.CreateTransaction();
                string[] dependencies = await this.GetDependenciesAsync( key, transaction );
                this.DeleteItemTransaction( key, dependencies, transaction );
                if ( await transaction.ExecuteAsync() )
                {
                    return;
                }
                else
                {
                    this.LogSource.Write( LogLevel.Debug, "Transaction DeleteItem failed. Retrying." );
                }
            }

            throw new CachingException( TooManyTransactionAttemptsErrorMessage );
        }

        private void LogRetryTransaction( [CallerMemberName] string memberName = null )
        {
            this.LogSource.Write(LogLevel.Debug, "Transaction {Method} failed. Retrying.", memberName);
        }

        /// <inheritdoc />
        protected override void DeleteItem( string key )
        {
            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                ITransaction transaction = this.Database.CreateTransaction();
                string[] dependencies = this.GetDependencies( key, transaction );
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

        private void DeleteItemTransaction( string key, string[] dependencies, ITransaction transaction )
        {
            RedisKey dependenciesKey = this.KeyBuilder.GetDependenciesKey( key );
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            if ( dependencies != null && dependencies.Length > firstDependencyIndex )
            {
                this.RemoveDependenciesTransaction( key, dependencies, transaction );
            }

#pragma warning disable 4014

            this.LogSource.Write( LogLevel.Debug, "KeyDelete({Key})", dependenciesKey );
            transaction.KeyDeleteAsync( dependenciesKey );
            this.LogSource.Write( LogLevel.Debug, "KeyDelete({Key})", valueKey );
            transaction.KeyDeleteAsync( valueKey );

#pragma warning restore 4014
        }

        internal void RemoveDependenciesTransaction( string key, string[] dependencies, ITransaction transaction )
        {
            if ( dependencies != null )
            {
                for ( int i = firstDependencyIndex; i < dependencies.Length; i++ )
                {
                    this.RemoveDependencyAsync( key, dependencies[i], transaction );
                }
            }
        }

        private Task RemoveDependencyAsync( string valueKey, string dependency, IDatabaseAsync database )
        {
            RedisKey dependencyKey = this.KeyBuilder.GetDependencyKey( dependency );
            this.LogSource.Write( LogLevel.Debug, "SetRemove({DependencyKey}, {ValueKey})", dependencyKey, valueKey );
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
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                ITransaction transaction = this.Database.CreateTransaction();

                RedisValue version = this.Database.ListGetByIndex( valueKey, itemVersionIndex );

                if ( version.HasValue )
                {
                    // There may be a race between two threads trying to set the same key with different values.
                    // By design (i.e., because we don't support distributed locks), it does not matter which thread wins.
                    // However, we want to prevent corrupted entries, i.e. we want to ensure that the value/dependency/dependencies
                    // keys are all consistent. This is why we're using the item versioning thing.

                    this.LogSource.Write( LogLevel.Debug, "Version of existing item is {Version}. Deleting the key {Key} under the condition that the version remains equal.", version, valueKey );
                    transaction.AddCondition( Condition.ListIndexEqual( valueKey, itemVersionIndex, version ) );
                    transaction.KeyDeleteAsync( valueKey );

                    if ( version != noDependencyVersion )
                    {
                        string[] dependencies = this.GetDependencies( key, transaction );
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
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                ITransaction transaction = this.Database.CreateTransaction();
#pragma warning disable 4014

                RedisValue version = await this.Database.ListGetByIndexAsync( valueKey, itemVersionIndex );

                if ( version.HasValue )
                {
                    // There may be a race between two threads trying to set the same key with different values.
                    // By design (i.e., because we don't support distributed locks), it does not matter which thread wins.
                    // However, we want to prevent corrupted entries, i.e. we want to ensure that the value/dependency/dependencies
                    // keys are all consistent. This is why we're using the item versioning thing.

                    this.LogSource.Write( LogLevel.Debug, "Version of existing item is {Version}. Deleting the key {Key} under the condition that the version remains equal.", version, valueKey );
                    transaction.AddCondition( Condition.ListIndexEqual( valueKey, itemVersionIndex, version ) );
#pragma warning disable 4014
                    transaction.KeyDeleteAsync( valueKey );
#pragma warning restore 4014

                    if ( version != noDependencyVersion )
                    {
                        string[] dependencies = this.GetDependencies( key, transaction );
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
                    this.LogSource.Write( LogLevel.Debug, "Transaction SetItem failed. Retrying." );
                }
            }

            throw new CachingException( TooManyTransactionAttemptsErrorMessage );
        }

        private void SetItemTransaction( string key, CacheItem item, ITransaction transaction )
        {
            // We could serialize in the background but it does not really make sense here, because the main cost is deserializing, not serializing.
            RedisValue value = this.CreateRedisValue( item );
            
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );

            TimeSpan? expiry = CreateExpiry( item );

            string version;

            if ( item.Dependencies == null )
            {
                version = noDependencyVersion;
            }
            else
            {
                // Store dependencies.
                foreach ( string dependency in item.Dependencies )
                {
                    RedisKey dependencyKey = this.KeyBuilder.GetDependencyKey( dependency );
                    transaction.SetAddAsync( dependencyKey, key );
                }

                // Generate unique identifier of the value
                version = Guid.NewGuid().ToString();
                string dependenciesString = version + dependenciesSeparator + string.Join( dependenciesSeparatorString, item.Dependencies );
                transaction.StringSetAsync( this.KeyBuilder.GetDependenciesKey( key ), dependenciesString );
            }

            // Store the item itself.
            transaction.ListRightPushAsync( valueKey, new RedisValue[] {version, value} );
            transaction.KeyExpireAsync( valueKey, expiry );
        }

        /// <inheritdoc />
        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );
            RedisValue[] itemList;
            string[] dependencies = null;

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                itemList = this.Database.ListRange( valueKey );

                if ( itemList == null || itemList.Length == 0 )
                {
                    return null;
                }

                if ( !includeDependencies || itemList[itemVersionIndex] == noDependencyVersion )
                {
                    goto itemGotten;
                }

                dependencies = this.GetDependencies( key );

                if ( dependencies != null && dependencies[itemVersionInDependenciesIndex] == (string) itemList[itemVersionIndex] )
                {
                    goto itemGotten;
                }
            }

            throw new CachingException( tooManyGetItemAttemptsErrorMessage );

            itemGotten:

            object cacheValue = this.GetCacheValue( valueKey, itemList[itemValueIndex] );

            if ( dependencies == null )
            {
                return new CacheValue( cacheValue );
            }
            else
            {
                return new CacheValue( cacheValue, ImmutableArray.Create( dependencies, firstDependencyIndex, dependencies.Length - 1 ) );
            }
        }

        /// <inheritdoc />
        protected override async Task<CacheValue> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
        {
            RedisKey valueKey = this.KeyBuilder.GetValueKey( key );
            RedisValue[] itemList;
            string[] dependencies = null;

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                itemList = await this.Database.ListRangeAsync( valueKey );

                if ( itemList == null || itemList.Length == 0 )
                {
                    return null;
                }


                if ( !includeDependencies || itemList[itemVersionIndex] == noDependencyVersion )
                {
                    goto itemGotten;
                }

                dependencies = await this.GetDependenciesAsync( key );

                if ( dependencies != null && dependencies[itemVersionInDependenciesIndex] == (string) itemList[itemVersionIndex] )
                {
                    goto itemGotten;
                }
            }

            throw new CachingException( tooManyGetItemAttemptsErrorMessage );

            itemGotten:

            object cacheValue = this.GetCacheValue( valueKey, itemList[itemValueIndex] );

            if ( dependencies == null )
            {
                return new CacheValue( cacheValue );
            }
            else
            {
                return new CacheValue( cacheValue, ImmutableArray.Create( dependencies, firstDependencyIndex, dependencies.Length - 1 ) );
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
            RedisKey dependencyKey = this.KeyBuilder.GetDependencyKey( key );

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                RedisValue[] dependentItemKeys = await this.Database.SetMembersAsync( dependencyKey );

                if ( dependentItemKeys == null || dependentItemKeys.Length <= 0 )
                {
                    // There is nothing to invalidate.
                    return;
                }

                ITransaction transaction = this.Database.CreateTransaction();

                foreach ( RedisValue dependentItemKey in dependentItemKeys )
                {
                    string[] dependencies = await this.GetDependenciesAsync( dependentItemKey, transaction );
                    this.DeleteItemTransaction( dependentItemKey, dependencies, transaction );
                }

                if ( await transaction.ExecuteAsync() )
                {
                    // Send notifications.
                    await this.SendEventAsync( dependencyInvalidatedEvent, key );
                    foreach ( RedisValue dependentItemKey in dependentItemKeys )
                    {
                        await this.SendEventAsync( itemInvalidatedEvent, dependentItemKey );
                    }
                    return;
                }
                else
                {
                    this.LogSource.Write( LogLevel.Debug, "Transaction InvalidateDependency failed. Retrying." );
                }
            }

            throw new CachingException( TooManyTransactionAttemptsErrorMessage );
        }

        protected override void InvalidateDependencyCore( string key )
        {
            RedisKey dependencyKey = this.KeyBuilder.GetDependencyKey( key );

            for ( int attempt = 0; attempt < this.Configuration.TransactionMaxRetries + 1; attempt++ )
            {
                RedisValue[] dependentItemKeys = this.Database.SetMembers( dependencyKey );

                if ( dependentItemKeys == null || dependentItemKeys.Length <= 0 )
                {
                    // There is nothing to invalidate.
                    return;
                }

                ITransaction transaction = this.Database.CreateTransaction();

                foreach ( RedisValue dependentItemKey in dependentItemKeys )
                {
                    string[] dependencies = this.GetDependencies( dependentItemKey, transaction );
                    this.DeleteItemTransaction( dependentItemKey, dependencies, transaction );
                }

                if ( transaction.Execute() )
                {
                    // Send notifications.
                    this.SendEvent( dependencyInvalidatedEvent, key );
                    foreach ( RedisValue dependentItemKey in dependentItemKeys )
                    {
                        this.SendEvent( itemInvalidatedEvent, dependentItemKey );
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

        public Task CleanUpAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<IServer> servers = this.Connection.GetEndPoints().Select( endpoint => this.Connection.GetServer( endpoint ) );

            return Task.WhenAll( servers.Select( s => this.CleanUpAsync(s, cancellationToken) ) );
        }

        public Task CleanUpAsync( IServer server, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<Task> tasks = new List<Task>();
            foreach ( RedisKey key in server.Keys( this.Database.Database, this.KeyBuilder.KeyPrefix + ":*", flags: CommandFlags.PreferSlave ) )
            {
                cancellationToken.ThrowIfCancellationRequested();

                StringTokenizer tokenizer = new StringTokenizer( key );

                string keyPrefix = tokenizer.GetNext();

                if ( keyPrefix != this.KeyBuilder.KeyPrefix )
                {
                    this.LogSource.Write( LogLevel.Warning, "The key {Key} has an invalid prefix. Redis should not have returned it. Ignoring it.", keyPrefix );
                    continue;
                }

                string keyKind = tokenizer.GetNext();

                if ( keyKind == RedisKeyBuilder.GarbageCollectionPrefix )
                {
                    continue;
                }

                string smallKey = tokenizer.GetRest();

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

        private async Task CleanUpDependency( string key)
        {
            RedisValue[] items = await this.Database.SetMembersAsync( this.KeyBuilder.GetDependencyKey( key ) );

            foreach ( RedisValue item in items )
            {
                if ( !await this.Database.KeyExistsAsync( this.KeyBuilder.GetValueKey( item ) ) )
                {
                    this.LogSource.Write( LogLevel.Warning, "The dependency key {Key} does not have the corresponding dependencies key. Deleting it.", key );

                    await this.RemoveDependencyAsync( item, key, this.Database );
                }
            }
        }

        private async Task CleanUpDependencies( string key )
        {
            if ( !await this.Database.KeyExistsAsync( this.KeyBuilder.GetValueKey( key ) ) )
            {
                this.LogSource.Write( LogLevel.Warning, "The dependencies key {Key} does not have the corresponding value key. Deleting it.", key );

                await this.DeleteItemAsync( key );
            }
        }

        private async Task CleanUpValue( RedisKey redisKey, string smallKey )
        {
            string itemVersion = this.Database.ListGetByIndex(redisKey, itemVersionIndex, CommandFlags.PreferSlave);
            string[] dependencies = await this.GetDependenciesAsync( smallKey );

            if ( dependencies == null )
            {
                if ( itemVersion == noDependencyVersion )
                {
                    return;
                }

                this.LogSource.Write( LogLevel.Warning, "The value key {Key} does not have the corresponding dependencies key. Deleting it.", smallKey );

                await this.Database.KeyDeleteAsync( redisKey );

                return;
            }

            if ( itemVersion != dependencies[itemVersionInDependenciesIndex] )
            {
                this.LogSource.Write( LogLevel.Warning, "The value {Key} version and the corresponding dependencies version differ. Deleting both.",
                                   smallKey );

                for ( int i = firstDependencyIndex; i < dependencies.Length; i++ )
                {
                    await this.RemoveDependencyAsync( smallKey, dependencies[i], this.Database );
                }

                await this.Database.KeyDeleteAsync( this.KeyBuilder.GetDependenciesKey( smallKey ) );
                await this.Database.KeyDeleteAsync( redisKey );

                return;
            }

            for ( int i = firstDependencyIndex; i < dependencies.Length; i++ )
            {
                if ( !await this.Database.SetContainsAsync( this.KeyBuilder.GetDependencyKey( dependencies[i] ), smallKey ) )
                {
                    this.LogSource.Write( LogLevel.Warning, "The value key {Key} does not have the corresponding dependency key {Dependency}. Deleting it.",
                                        smallKey, dependencies[i] );

                    await this.DeleteItemAsync( smallKey );
                }
            }
        }

        internal class DependenciesRedisCachingBackendFeatures : RedisCachingBackendFeatures
        {
            public DependenciesRedisCachingBackendFeatures( DependenciesRedisCachingBackend parent )
                : base( parent )
            {
            }

            public override bool Dependencies => true;

            public override bool ContainsDependency => true;
        }
    }
}
