// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

/// <summary>
/// This class is now probably not necessary because its "dispose of disposables" features is no longer used by any of our tests.
/// But it seems to be doing more things, so I'll leave it here.
/// </summary>
internal sealed class DisposingRedisCachingBackend : CachingBackendEnhancer
{
    private readonly IDisposable[] _disposables;

    // ReSharper disable once UnusedMember.Global
    public IConnectionMultiplexer Connection => this.RedisBackend.Connection;

    public IDatabase Database => this.RedisBackend.Database;

    public RedisCachingBackendConfiguration Configuration => this.RedisBackend.Configuration;

    private RedisCachingBackend RedisBackend => GetRedisCachingBackend( this );

    public DisposingRedisCachingBackend( CachingBackend underlyingBackend, params IDisposable[] disposables ) : base( underlyingBackend )
    {
        this._disposables = disposables;
    }

    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );

        foreach ( var d in this._disposables )
        {
            d.Dispose();
        }
    }

    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken );

        foreach ( var d in this._disposables )
        {
            d.Dispose();
        }
    }

    private static RedisCachingBackend GetRedisCachingBackend( CachingBackend cachingBackend )
    {
        return cachingBackend switch
        {
            CachingBackendEnhancer enhancer => GetRedisCachingBackend( enhancer.UnderlyingBackend ),
            RedisCachingBackend redisCachingBackend => redisCachingBackend,
            _ => throw new InvalidOperationException()
        };
    }
}