// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Loggers;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal class RedisConnectionFactory
{
    public IConnectionMultiplexer? RedisConnection { get; }

    public ConfigurationOptions? RedisConnectionOptions { get; }

    public RedisConnectionFactory( ConfigurationOptions redisConnectionOptions )
    {
        this.RedisConnectionOptions = redisConnectionOptions;
    }

    public RedisConnectionFactory( IConnectionMultiplexer connection )
    {
        this.RedisConnection = connection;
    }

    internal IConnectionMultiplexer GetConnection( IServiceProvider? serviceProvider )
    {
        if ( this.RedisConnection != null )
        {
            return this.RedisConnection;
        }
        else
        {
            var textWriter = serviceProvider != null
                ? new FlashTraceTextWriter( serviceProvider.GetFlashtraceSource( "Redis", FlashtraceRole.Caching ).Information )
                : null;

            return Task.Run( () => ConnectionMultiplexer.ConnectAsync( this.RedisConnectionOptions!, textWriter ) ).Result;
        }
    }

    internal async Task<IConnectionMultiplexer> GetConnectionAsync( IServiceProvider? serviceProvider, CancellationToken cancellationToken )
    {
        if ( this.RedisConnection != null )
        {
            return this.RedisConnection;
        }
        else
        {
            cancellationToken.ThrowIfCancellationRequested();

            var textWriter = serviceProvider != null
                ? new FlashTraceTextWriter( serviceProvider.GetFlashtraceSource( "Redis", FlashtraceRole.Caching ).Information )
                : null;

            return await ConnectionMultiplexer.ConnectAsync( this.RedisConnectionOptions!, textWriter );
        }
    }
}