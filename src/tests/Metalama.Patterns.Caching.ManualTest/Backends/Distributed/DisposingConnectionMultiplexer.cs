// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using StackExchange.Redis;
using StackExchange.Redis.Profiling;
using System.Net;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

internal sealed class DisposingConnectionMultiplexer : IConnectionMultiplexer
{
    private readonly IDisposable[] _disposables;

    public ConnectionMultiplexer Inner { get; }

    public DisposingConnectionMultiplexer( ConnectionMultiplexer inner, params IDisposable[] disposables )
    {
        this.Inner = inner;
        this._disposables = disposables;
    }

    string IConnectionMultiplexer.ClientName => this.Inner.ClientName;

    string IConnectionMultiplexer.Configuration => this.Inner.Configuration;

    int IConnectionMultiplexer.TimeoutMilliseconds => this.Inner.TimeoutMilliseconds;

    long IConnectionMultiplexer.OperationCount => this.Inner.OperationCount;

#pragma warning disable CS0618 // Type or member is obsolete
    bool IConnectionMultiplexer.PreserveAsyncOrder { get => this.Inner.PreserveAsyncOrder; set => this.Inner.PreserveAsyncOrder = value; }
#pragma warning restore CS0618 // Type or member is obsolete

    bool IConnectionMultiplexer.IsConnected => this.Inner.IsConnected;

    bool IConnectionMultiplexer.IsConnecting => this.Inner.IsConnecting;

    bool IConnectionMultiplexer.IncludeDetailInExceptions
    {
        get => this.Inner.IncludeDetailInExceptions;
        set => this.Inner.IncludeDetailInExceptions = value;
    }

    int IConnectionMultiplexer.StormLogThreshold { get => this.Inner.StormLogThreshold; set => this.Inner.StormLogThreshold = value; }

    event EventHandler<RedisErrorEventArgs> IConnectionMultiplexer.ErrorMessage
    {
        add => this.Inner.ErrorMessage += value;
        remove => this.Inner.ErrorMessage -= value;
    }

    event EventHandler<ConnectionFailedEventArgs> IConnectionMultiplexer.ConnectionFailed
    {
        add => this.Inner.ConnectionFailed += value;
        remove => this.Inner.ConnectionFailed -= value;
    }

    event EventHandler<InternalErrorEventArgs> IConnectionMultiplexer.InternalError
    {
        add => this.Inner.InternalError += value;
        remove => this.Inner.InternalError -= value;
    }

    event EventHandler<ConnectionFailedEventArgs> IConnectionMultiplexer.ConnectionRestored
    {
        add => this.Inner.ConnectionRestored += value;
        remove => this.Inner.ConnectionRestored -= value;
    }

    event EventHandler<EndPointEventArgs> IConnectionMultiplexer.ConfigurationChanged
    {
        add => this.Inner.ConfigurationChanged += value;
        remove => this.Inner.ConfigurationChanged -= value;
    }

    event EventHandler<EndPointEventArgs> IConnectionMultiplexer.ConfigurationChangedBroadcast
    {
        add => this.Inner.ConfigurationChangedBroadcast += value;
        remove => this.Inner.ConfigurationChangedBroadcast -= value;
    }

    event EventHandler<HashSlotMovedEventArgs> IConnectionMultiplexer.HashSlotMoved
    {
        add => this.Inner.HashSlotMoved += value;
        remove => this.Inner.HashSlotMoved -= value;
    }

    void IConnectionMultiplexer.Close( bool allowCommandsToComplete ) => this.Inner.Close( allowCommandsToComplete );

    Task IConnectionMultiplexer.CloseAsync( bool allowCommandsToComplete ) => this.Inner.CloseAsync( allowCommandsToComplete );

    bool IConnectionMultiplexer.Configure( TextWriter log ) => this.Inner.Configure( log );

    Task<bool> IConnectionMultiplexer.ConfigureAsync( TextWriter log ) => this.Inner.ConfigureAsync( log );

    void IDisposable.Dispose()
    {
        this.Inner.Dispose();

        foreach ( var disposable in this._disposables )
        {
            disposable.Dispose();
        }
    }

    void IConnectionMultiplexer.ExportConfiguration( Stream destination, ExportOptions options ) => this.Inner.ExportConfiguration( destination, options );

    ServerCounters IConnectionMultiplexer.GetCounters() => this.Inner.GetCounters();

    IDatabase IConnectionMultiplexer.GetDatabase( int db, object asyncState ) => this.Inner.GetDatabase( db, asyncState );

    EndPoint[] IConnectionMultiplexer.GetEndPoints( bool configuredOnly ) => this.Inner.GetEndPoints( configuredOnly );

    int IConnectionMultiplexer.GetHashSlot( RedisKey key ) => this.Inner.GetHashSlot( key );

    IServer IConnectionMultiplexer.GetServer( string host, int port, object asyncState ) => this.Inner.GetServer( host, port, asyncState );

    IServer IConnectionMultiplexer.GetServer( string hostAndPort, object asyncState ) => this.Inner.GetServer( hostAndPort, asyncState );

    IServer IConnectionMultiplexer.GetServer( IPAddress host, int port ) => this.Inner.GetServer( host, port );

    IServer IConnectionMultiplexer.GetServer( EndPoint endpoint, object asyncState ) => this.Inner.GetServer( endpoint, asyncState );

    string IConnectionMultiplexer.GetStatus() => this.Inner.GetStatus();

    void IConnectionMultiplexer.GetStatus( TextWriter log ) => this.Inner.GetStatus( log );

    string IConnectionMultiplexer.GetStormLog() => this.Inner.GetStormLog();

    ISubscriber IConnectionMultiplexer.GetSubscriber( object asyncState ) => this.Inner.GetSubscriber( asyncState );

    int IConnectionMultiplexer.HashSlot( RedisKey key ) => this.Inner.HashSlot( key );

    long IConnectionMultiplexer.PublishReconfigure( CommandFlags flags ) => this.Inner.PublishReconfigure( flags );

    Task<long> IConnectionMultiplexer.PublishReconfigureAsync( CommandFlags flags ) => this.Inner.PublishReconfigureAsync( flags );

    void IConnectionMultiplexer.RegisterProfiler( Func<ProfilingSession> profilingSessionProvider ) => this.Inner.RegisterProfiler( profilingSessionProvider );

    void IConnectionMultiplexer.ResetStormLog() => this.Inner.ResetStormLog();

    string IConnectionMultiplexer.ToString() => this.Inner.ToString();

    void IConnectionMultiplexer.Wait( Task task ) => this.Inner.Wait( task );

    T IConnectionMultiplexer.Wait<T>( Task<T> task ) => this.Inner.Wait( task );

    void IConnectionMultiplexer.WaitAll( params Task[] tasks ) => this.Inner.WaitAll( tasks );
}