// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using System.Net;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

internal static class RedisFactory
{
    public static RedisTestInstance CreateTestInstance( TestContext testContext, RedisSetupFixture redisSetupFixture )
    {
        RedisTestInstance redisTestInstance = null;

        if ( !testContext.Properties.Contains( "RedisEndpoint" ) )
        {
            redisTestInstance = redisSetupFixture.TestInstance;
            testContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        return redisTestInstance;
    }

    public static DisposingRedisCachingBackend CreateBackend(
        TestContext testContext,
        RedisSetupFixture redisSetupFixture,
        string prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false )
    {
        var redisTestInstance = CreateTestInstance( testContext, redisSetupFixture );

        var configuration =
            new RedisCachingBackendConfiguration
            {
                KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
                OwnsConnection = true,
                SupportsDependencies = supportsDependencies,
                IsLocallyCached = locallyCached
            };

        IConnectionMultiplexer connection = CreateConnection( testContext );

        return new DisposingRedisCachingBackend( RedisCachingBackend.Create( connection, configuration ) );
    }

    public static DisposingConnectionMultiplexer CreateConnection( TestContext testContext )
    {
        var socketManager = new SocketManager();

        var redisConfigurationOptions = new ConfigurationOptions();
        redisConfigurationOptions.EndPoints.Add( (EndPoint) testContext.Properties["RedisEndpoint"] );
        redisConfigurationOptions.AbortOnConnectFail = false;
        redisConfigurationOptions.SocketManager = socketManager;

        var connection = ConnectionMultiplexer.Connect( redisConfigurationOptions, Console.Out );

        return new DisposingConnectionMultiplexer( connection, socketManager );
    }

    public static async Task<DisposingRedisCachingBackend> CreateBackendAsync(
        TestContext testContext,
        RedisSetupFixture redisSetupFixture,
        string prefix = null,
        bool supportsDependencies = false,
        bool locallyCached = false )
    {
        RedisTestInstance redisTestInstance = null;

        if ( !testContext.Properties.Contains( "RedisEndpoint" ) )
        {
            redisTestInstance = redisSetupFixture.TestInstance;
            testContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }

        var configuration = new RedisCachingBackendConfiguration
        {
            KeyPrefix = prefix ?? Guid.NewGuid().ToString(),
            OwnsConnection = true,
            SupportsDependencies = supportsDependencies,
            IsLocallyCached = locallyCached
        };

        return new DisposingRedisCachingBackend( await RedisCachingBackend.CreateAsync( CreateConnection( testContext ), configuration ) );
    }
}

internal class DisposingConnectionMultiplexer : IConnectionMultiplexer, IDisposable
{
    private readonly IDisposable[] disposables;
    private ConnectionMultiplexer inner;

    public ConnectionMultiplexer Inner => this.inner;

    public DisposingConnectionMultiplexer( ConnectionMultiplexer inner, params IDisposable[] disposables )
    {
        this.inner = inner;
        this.disposables = disposables;
    }

    string IConnectionMultiplexer.ClientName => this.inner.ClientName;

    string IConnectionMultiplexer.Configuration => this.inner.Configuration;

    int IConnectionMultiplexer.TimeoutMilliseconds => this.inner.TimeoutMilliseconds;

    long IConnectionMultiplexer.OperationCount => this.inner.OperationCount;

#pragma warning disable CS0618 // Type or member is obsolete
    bool IConnectionMultiplexer.PreserveAsyncOrder { get => this.inner.PreserveAsyncOrder; set => this.inner.PreserveAsyncOrder = value; }
#pragma warning restore CS0618 // Type or member is obsolete

    bool IConnectionMultiplexer.IsConnected => this.inner.IsConnected;

    bool IConnectionMultiplexer.IsConnecting => this.inner.IsConnecting;

    bool IConnectionMultiplexer.IncludeDetailInExceptions
    {
        get => this.inner.IncludeDetailInExceptions;
        set => this.inner.IncludeDetailInExceptions = value;
    }

    int IConnectionMultiplexer.StormLogThreshold { get => this.inner.StormLogThreshold; set => this.inner.StormLogThreshold = value; }

    event EventHandler<RedisErrorEventArgs> IConnectionMultiplexer.ErrorMessage
    {
        add
        {
            this.inner.ErrorMessage += value;
        }

        remove
        {
            this.inner.ErrorMessage -= value;
        }
    }

    event EventHandler<ConnectionFailedEventArgs> IConnectionMultiplexer.ConnectionFailed
    {
        add
        {
            this.inner.ConnectionFailed += value;
        }

        remove
        {
            this.inner.ConnectionFailed -= value;
        }
    }

    event EventHandler<InternalErrorEventArgs> IConnectionMultiplexer.InternalError
    {
        add
        {
            this.inner.InternalError += value;
        }

        remove
        {
            this.inner.InternalError -= value;
        }
    }

    event EventHandler<ConnectionFailedEventArgs> IConnectionMultiplexer.ConnectionRestored
    {
        add
        {
            this.inner.ConnectionRestored += value;
        }

        remove
        {
            this.inner.ConnectionRestored -= value;
        }
    }

    event EventHandler<EndPointEventArgs> IConnectionMultiplexer.ConfigurationChanged
    {
        add
        {
            this.inner.ConfigurationChanged += value;
        }

        remove
        {
            this.inner.ConfigurationChanged -= value;
        }
    }

    event EventHandler<EndPointEventArgs> IConnectionMultiplexer.ConfigurationChangedBroadcast
    {
        add
        {
            this.inner.ConfigurationChangedBroadcast += value;
        }

        remove
        {
            this.inner.ConfigurationChangedBroadcast -= value;
        }
    }

    event EventHandler<HashSlotMovedEventArgs> IConnectionMultiplexer.HashSlotMoved
    {
        add
        {
            this.inner.HashSlotMoved += value;
        }

        remove
        {
            this.inner.HashSlotMoved -= value;
        }
    }

    void IConnectionMultiplexer.Close( bool allowCommandsToComplete ) => this.inner.Close( allowCommandsToComplete );

    Task IConnectionMultiplexer.CloseAsync( bool allowCommandsToComplete ) => this.inner.CloseAsync( allowCommandsToComplete );

    bool IConnectionMultiplexer.Configure( TextWriter log ) => this.inner.Configure( log );

    Task<bool> IConnectionMultiplexer.ConfigureAsync( TextWriter log ) => this.inner.ConfigureAsync( log );

    void IDisposable.Dispose() => this.inner.Dispose();

    void IConnectionMultiplexer.ExportConfiguration( Stream destination, ExportOptions options ) => this.inner.ExportConfiguration( destination, options );

    ServerCounters IConnectionMultiplexer.GetCounters() => this.inner.GetCounters();

    IDatabase IConnectionMultiplexer.GetDatabase( int db, object asyncState ) => this.inner.GetDatabase( db, asyncState );

    EndPoint[] IConnectionMultiplexer.GetEndPoints( bool configuredOnly ) => this.inner.GetEndPoints( configuredOnly );

    int IConnectionMultiplexer.GetHashSlot( RedisKey key ) => this.inner.GetHashSlot( key );

    IServer IConnectionMultiplexer.GetServer( string host, int port, object asyncState ) => this.inner.GetServer( host, port, asyncState );

    IServer IConnectionMultiplexer.GetServer( string hostAndPort, object asyncState ) => this.inner.GetServer( hostAndPort, asyncState );

    IServer IConnectionMultiplexer.GetServer( IPAddress host, int port ) => this.inner.GetServer( host, port );

    IServer IConnectionMultiplexer.GetServer( EndPoint endpoint, object asyncState ) => this.inner.GetServer( endpoint, asyncState );

    string IConnectionMultiplexer.GetStatus() => this.inner.GetStatus();

    void IConnectionMultiplexer.GetStatus( TextWriter log ) => this.inner.GetStatus( log );

    string IConnectionMultiplexer.GetStormLog() => this.inner.GetStormLog();

    ISubscriber IConnectionMultiplexer.GetSubscriber( object asyncState ) => this.inner.GetSubscriber( asyncState );

    int IConnectionMultiplexer.HashSlot( RedisKey key ) => this.inner.HashSlot( key );

    long IConnectionMultiplexer.PublishReconfigure( CommandFlags flags ) => this.inner.PublishReconfigure( flags );

    Task<long> IConnectionMultiplexer.PublishReconfigureAsync( CommandFlags flags ) => this.inner.PublishReconfigureAsync( flags );

    void IConnectionMultiplexer.RegisterProfiler( Func<ProfilingSession> profilingSessionProvider ) => this.inner.RegisterProfiler( profilingSessionProvider );

    void IConnectionMultiplexer.ResetStormLog() => this.inner.ResetStormLog();

    string IConnectionMultiplexer.ToString() => this.inner.ToString();

    void IConnectionMultiplexer.Wait( Task task ) => this.inner.Wait( task );

    T IConnectionMultiplexer.Wait<T>( Task<T> task ) => this.inner.Wait<T>( task );

    void IConnectionMultiplexer.WaitAll( params Task[] tasks ) => this.inner.WaitAll( tasks );
}

/// <summary>
/// This class is now probably not necessary because its "dispose of disposables" features is no longer used by any of our tests.
/// But it seems to be doing more things, so I'll leave it here.
/// </summary>
internal class DisposingRedisCachingBackend : CachingBackendEnhancer
{
    private readonly IDisposable[] disposables;

    public IConnectionMultiplexer Connection => this.RedisBackend.Connection;

    public IDatabase Database => this.RedisBackend.Database;

    public RedisCachingBackendConfiguration Configuration => this.RedisBackend.Configuration;

    public RedisCachingBackend RedisBackend => GetRedisCachingBackend( this );

    public DisposingRedisCachingBackend( CachingBackend underlyingBackend, params IDisposable[] disposables ) : base( underlyingBackend )
    {
        this.disposables = disposables;
    }

    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );

        foreach ( var d in this.disposables )
        {
            d?.Dispose();
        }
    }

    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken );

        foreach ( var d in this.disposables )
        {
            d?.Dispose();
        }
    }

    private static RedisCachingBackend GetRedisCachingBackend( CachingBackend cachingBackend )
    {
        var enhancer = cachingBackend as CachingBackendEnhancer;

        if ( enhancer != null )
        {
            return GetRedisCachingBackend( enhancer.UnderlyingBackend );
        }

        var redisCachingBackend = cachingBackend as RedisCachingBackend;

        if ( redisCachingBackend != null )
        {
            return redisCachingBackend;
        }

        throw new InvalidOperationException();
    }
}