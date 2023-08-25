// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ManualTest.Backends.Distributed;
using Metalama.Patterns.Caching.TestHelpers;
using StackExchange.Redis;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Single;

public sealed class RedisCacheBackendTests : BaseCacheBackendTests, IAssemblyFixture<RedisSetupFixture>
{
    private readonly RedisSetupFixture _redisSetupFixture;

    public RedisCacheBackendTests( TestContext testContext, RedisSetupFixture redisSetupFixture, ITestOutputHelper testOutputHelper ) : base(
        testContext,
        testOutputHelper )
    {
        this._redisSetupFixture = redisSetupFixture;
    }

    protected override void Cleanup()
    {
        base.Cleanup();
        AssertEx.Equal( 0, RedisNotificationQueue.NotificationProcessingThreads, "RedisNotificationQueue.NotificationProcessingThreads" );
    }

    protected override CachingBackend CreateBackend()
    {
        return this.CreateBackend( null );
    }

    protected override async Task<CachingBackend> CreateBackendAsync()
    {
        return await this.CreateBackendAsync( null );
    }

    protected override ITestableCachingComponent CreateCollector( CachingBackend backend )
    {
        return RedisCacheDependencyGarbageCollector.Create( ((DisposingRedisCachingBackend) backend).UnderlyingBackend );
    }

    protected override async Task<ITestableCachingComponent> CreateCollectorAsync( CachingBackend backend )
    {
        return await RedisCacheDependencyGarbageCollector.CreateAsync( ((DisposingRedisCachingBackend) backend).UnderlyingBackend );
    }

    private DisposingRedisCachingBackend CreateBackend( string? keyPrefix )
    {
        return RedisFactory.CreateBackend( this.TestContext, this._redisSetupFixture, keyPrefix, supportsDependencies: true );
    }

    private async Task<DisposingRedisCachingBackend> CreateBackendAsync( string? keyPrefix )
    {
        return await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, keyPrefix, supportsDependencies: true );
    }

    private static string GeneratePrefix()
    {
        var keyPrefix = Guid.NewGuid().ToString();

        return keyPrefix;
    }

    [Fact( Timeout = Timeout, Skip = "Ignore Redis" )]
    public Task TestGarbageCollectionAsync()
    {
        var prefix = GeneratePrefix();

        var redisTestInstance = this._redisSetupFixture.TestInstance;
        this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;

        Assert.Equal( 0, this.GetAllKeys( prefix ).Count );

        // [Porting] The null passed to CreateAsync in the second using below will throw because that parameter is [Required]. This test
        // is currently skipped (as per original code), but would throw here if re-enabled. Not fixing, as can't be certain of original intent.
        throw new NotImplementedException( "This test was identified as known-broken during porting, see comment above." );
/*
        using ( var cache = await RedisFactory.CreateBackendAsync( this.TestContext, this._redisSetupFixture, prefix: prefix, supportsDependencies: true ) )
        using ( var collector = await RedisCacheDependencyGarbageCollector.CreateAsync( cache.Connection, null ) )
        {
            cache.SetItem( "i1", new CacheItem( "value", ImmutableList.Create( "d1", "d2", "d3" ) ) );
            cache.SetItem( "i2", new CacheItem( "value", ImmutableList.Create( "d1", "d2", "d3" ) ) );
            cache.SetItem( "i3", new CacheItem( "value", ImmutableList.Create( "d1", "d2", "d3" ) ) );

            // ReSharper restore MethodHasAsyncOverload

            Assert.True( this.GetAllKeys( prefix ).Count > 0 );

            await cache.InvalidateDependencyAsync( "d1" );

            await TestableCachingComponentDisposer.DisposeAsync<ITestableCachingComponent>( cache, collector );
        }

        // ReSharper restore UseAwaitUsing

        // Make sure we dispose the back-end so that the GC key gets removed too.

        IList<string> keys = this.GetAllKeys( prefix );

        this._testOutputHelper.WriteLine( "Remaining keys:" + string.Join( ", ", keys ) );

        Assert.Equal( 0, keys.Count );
*/
    }

    [Fact( Timeout = Timeout, Skip = "Ignore Redis" )]
    public async Task TestGarbageCollectionByExpiration()
    {
        var keyPrefix = GeneratePrefix();

        // [Porting] Not fixing, can't be certain of original intent.
        // ReSharper disable UseAwaitUsing
        using ( var cache = await this.CreateBackendAsync( keyPrefix ) )
        using ( var collector = await RedisCacheDependencyGarbageCollector.CreateAsync( cache.UnderlyingBackend ) )
        {
            const string valueSmallKey = "i";
            const string dependencySmallKey = "d";
            var offset = this.GetExpirationQuantum();

            var keyBuilder = new RedisKeyBuilder( cache.Database, cache.Configuration );

            string valueKey = keyBuilder.GetValueKey( valueSmallKey );
            string dependenciesKey = keyBuilder.GetDependenciesKey( valueSmallKey );
            string dependencyKey = keyBuilder.GetDependencyKey( dependencySmallKey );

            collector.NotificationQueue.SuspendProcessing();

            var itemExpiredEvent = new TaskCompletionSource<bool>();

            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            cache.ItemRemoved += ( _, args ) =>
            {
                Assert.Equal( valueSmallKey, args.Key );
                Assert.Equal( CacheItemRemovedReason.Expired, args.RemovedReason );
                Assert.False( itemExpiredEvent.Task.IsCompleted );
                itemExpiredEvent.SetResult( true );
            };

            var cacheItem = new CacheItem(
                "v",
                ImmutableList.Create( dependencySmallKey ),
                new CacheItemConfiguration { AbsoluteExpiration = offset } );

            await cache.SetItemAsync( valueSmallKey, cacheItem );

            Assert.True( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependencyKey ) );

            collector.NotificationQueue.ResumeProcessing();

            Assert.True( await itemExpiredEvent.Task.WithTimeout( TimeoutTimeSpan ) );

            await Task.Delay( this.GetExpirationQuantum( 2 ) );

            Assert.False( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.False( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.False( await cache.Database.KeyExistsAsync( dependencyKey ) );

            await TestableCachingComponentDisposer.DisposeAsync<ITestableCachingComponent>( cache, collector );
        }

        // ReSharper restore UseAwaitUsing
    }

    [Fact( Timeout = Timeout, Skip = "Ignore Redis" )]
    public Task TestSetBeforeGarbageCollectionByExpiration()
    {
        // [Porting] The null passed to CreateAsync in the second using below will throw because that parameter is [Required]. This test
        // is currently skipped (as per original code), but would throw here if re-enabled. Not fixing, as can't be certain of original intent.
        throw new NotImplementedException( "This test was identified as known-broken during porting, see comment above." );
/*
        var keyPrefix = GeneratePrefix();

        using ( var cache = await this.CreateBackendAsync( keyPrefix ) )
        using ( var collector = await RedisCacheDependencyGarbageCollector.CreateAsync( cache.Connection, null ) )
        {
            const string valueSmallKey = "i";
            const string dependencySmallKey = "d";
            var offset = this.GetExpirationTolerance();

            var keyBuilder = new RedisKeyBuilder( cache.Database, cache.Configuration );

            string valueKey = keyBuilder.GetValueKey( valueSmallKey );
            string dependenciesKey = keyBuilder.GetDependenciesKey( valueSmallKey );
            string dependencyKey = keyBuilder.GetDependencyKey( dependencySmallKey );

            collector.NotificationQueue.SuspendProcessing();

            var expiringCacheItem = new CacheItem(
                "v",
                ImmutableList.Create( dependencySmallKey ),
                new CacheItemConfiguration { AbsoluteExpiration = offset } );

            var nonExpiringCacheItem = new CacheItem(
                "v",
                ImmutableList.Create( dependencySmallKey ) );

            await cache.SetItemAsync( valueSmallKey, expiringCacheItem );

            Assert.True( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependencyKey ) );

            await Task.Delay( this.GetExpirationTolerance( 2 ) );

            Assert.False( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependencyKey ) );

            await cache.SetItemAsync( valueSmallKey, nonExpiringCacheItem );

            Assert.True( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependencyKey ) );

            collector.NotificationQueue.ResumeProcessing();

            Assert.True( await cache.Database.KeyExistsAsync( valueKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependenciesKey ) );
            Assert.True( await cache.Database.KeyExistsAsync( dependencyKey ) );

            await TestableCachingComponentDisposer.DisposeAsync<ITestableCachingComponent>( cache, collector );
        }
*/

        // ReSharper restore UseAwaitUsing
    }

    [Fact( Timeout = Timeout, Skip = "Ignore Redis" )]
    public async Task TestCleanUp()
    {
        var keyPrefix = GeneratePrefix();

        // [Porting] Not fixing, can't be certain of original intent (twice).
        // ReSharper disable UseAwaitUsing
        // ReSharper disable once MethodHasAsyncOverload
        using ( var redisCachingBackend = this.CreateBackend( keyPrefix ) )
        using ( var cache = (DependenciesRedisCachingBackend) redisCachingBackend.UnderlyingBackend )
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable MethodHasAsyncOverload
            cache.SetItem( "i1", new CacheItem( "value", ImmutableList.Create( "d1", "d2", "d3" ) ) );
            cache.SetItem( "i2", new CacheItem( "value", ImmutableList.Create( "d1", "d2", "d3" ) ) );
            cache.SetItem( "i3", new CacheItem( "value" ) );

            // ReSharper restore MethodHasAsyncOverload

            cache.Database.ListRightPush( GetValueKey( cache, "lonely-value-key" ), new RedisValue[] { Guid.NewGuid().ToString(), "value" } );
            cache.Database.StringSet( GetDependenciesKey( cache, "lonely-dependencies-key1" ), "non-existing-value-key1" );
            cache.Database.StringSet( GetDependenciesKey( cache, "lonely-dependencies-key2" ), "" );
            cache.Database.SetAdd( GetDependencyKey( cache, "lonely-dependency-key" ), "non-existing-value-key2" );

            cache.Database.ListRightPush(
                GetValueKey( cache, "non-corresponding-version-key" ),
                new RedisValue[] { "non-corresponding-version1", "value" } );

            cache.Database.StringSet(
                GetDependenciesKey( cache, "non-corresponding-version-key" ),
                "non-corresponding-version2\nnon-corresponding-version-dependency-key" );

            cache.Database.SetAdd( GetDependencyKey( cache, "non-corresponding-version-dependency-key" ), "non-corresponding-version-key" );

            Assert.True( ValueSmallKeyExists( cache, "i1" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "i1" ) );
            Assert.True( ValueSmallKeyExists( cache, "i2" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "i2" ) );
            Assert.True( ValueSmallKeyExists( cache, "i3" ) );
            Assert.False( DependenciesSmallKeyExists( cache, "i3" ) );

            Assert.True( ValueSmallKeyExists( cache, "lonely-value-key" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "lonely-dependencies-key1" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "lonely-dependencies-key2" ) );
            Assert.False( ValueSmallKeyExists( cache, "non-existing-value-key1" ) );
            Assert.False( ValueSmallKeyExists( cache, "non-existing-value-key2" ) );

            Assert.True( ValueSmallKeyExists( cache, "non-corresponding-version-key" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "non-corresponding-version-key" ) );

            Assert.True( DependencySmallKeyExists( cache, "d1" ) );
            Assert.True( DependencySmallKeyExists( cache, "d2" ) );
            Assert.True( DependencySmallKeyExists( cache, "d3" ) );

            Assert.True( DependencySmallKeyExists( cache, "lonely-dependency-key" ) );

            Assert.True( DependencySmallKeyExists( cache, "non-corresponding-version-dependency-key" ) );

            await cache.CleanUpAsync();

            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable MethodHasAsyncOverload
            Assert.NotNull( cache.GetItem( "i1" ) );
            Assert.NotNull( cache.GetItem( "i2" ) );
            Assert.NotNull( cache.GetItem( "i3" ) );

            // ReSharper restore MethodHasAsyncOverload

            Assert.True( ValueSmallKeyExists( cache, "i1" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "i1" ) );
            Assert.True( ValueSmallKeyExists( cache, "i2" ) );
            Assert.True( DependenciesSmallKeyExists( cache, "i2" ) );
            Assert.True( ValueSmallKeyExists( cache, "i3" ) );
            Assert.False( DependenciesSmallKeyExists( cache, "i3" ) );

            Assert.False( ValueSmallKeyExists( cache, "lonely-value-key" ) );
            Assert.False( DependenciesSmallKeyExists( cache, "lonely-dependencies-key1" ) );
            Assert.False( DependenciesSmallKeyExists( cache, "lonely-dependencies-key2" ) );
            Assert.False( ValueSmallKeyExists( cache, "non-existing-value-key1" ) );
            Assert.False( ValueSmallKeyExists( cache, "non-existing-value-key2" ) );

            Assert.False( ValueSmallKeyExists( cache, "non-corresponding-version-key" ) );
            Assert.False( DependenciesSmallKeyExists( cache, "non-corresponding-version-key" ) );

            Assert.True( DependencySmallKeyExists( cache, "d1" ) );
            Assert.True( DependencySmallKeyExists( cache, "d2" ) );
            Assert.True( DependencySmallKeyExists( cache, "d3" ) );

            Assert.False( DependencySmallKeyExists( cache, "lonely-dependency-key" ) );

            Assert.False( DependencySmallKeyExists( cache, "non-corresponding-version-dependency-key" ) );

            await TestableCachingComponentDisposer.DisposeAsync( cache );
        }

        // ReSharper restore UseAwaitUsing
    }

    private static string GetValueKey( DependenciesRedisCachingBackend cache, string smallKey )
    {
        return cache.Configuration.KeyPrefix + ":value:" + smallKey;
    }

    private static string GetDependenciesKey( DependenciesRedisCachingBackend cache, string smallKey )
    {
        return cache.Configuration.KeyPrefix + ":dependencies:" + smallKey;
    }

    private static string GetDependencyKey( DependenciesRedisCachingBackend cache, string smallKey )
    {
        return cache.Configuration.KeyPrefix + ":dependency:" + smallKey;
    }

    private static bool ValueSmallKeyExists( DependenciesRedisCachingBackend cache, string valueSmallKey )
    {
        return cache.Database.KeyExists( GetValueKey( cache, valueSmallKey ) );
    }

    private static bool DependenciesSmallKeyExists( DependenciesRedisCachingBackend cache, string valueSmallKey )
    {
        return cache.Database.KeyExists( GetDependenciesKey( cache, valueSmallKey ) );
    }

    private static bool DependencySmallKeyExists( DependenciesRedisCachingBackend cache, string valueSmallKey )
    {
        return cache.Database.KeyExists( GetDependencyKey( cache, valueSmallKey ) );
    }

    private IList<string> GetAllKeys( string prefix )
    {
        using ( var connection = RedisFactory.CreateConnection( this.TestContext ) )
        {
            var servers = connection.Inner.GetEndPoints().Select( endpoint => connection.Inner.GetServer( endpoint ) ).ToList();
            var keys = servers.SelectMany( server => server.Keys( pattern: prefix + ":*" ) ).ToList();

            return keys.Select( k => k.ToString() ).Where( k => k?.IndexOf( ":gc:", StringComparison.Ordinal ) == -1 ).ToList();
        }
    }
}