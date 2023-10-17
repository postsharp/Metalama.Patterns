// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Backends.Redis;

#pragma warning disable SA1623

/// <summary>
/// Options for <see cref="RedisCacheSynchronizer"/>.
/// </summary>
[PublicAPI]
public sealed record RedisCacheSynchronizerConfiguration : CacheSynchronizerConfiguration
{
    internal RedisConnectionFactory RedisConnectionFactory { get; }

    public RedisCacheSynchronizerConfiguration( ConfigurationOptions redisConnectionOptions, string? prefix = null )
    {
        this.RedisConnectionFactory = new RedisConnectionFactory( redisConnectionOptions );
        this.OwnsConnection = true;

        if ( prefix != null )
        {
            this.Prefix = prefix;
        }
    }

    public RedisCacheSynchronizerConfiguration( IConnectionMultiplexer connection, string? prefix = null )
    {
        this.RedisConnectionFactory = new RedisConnectionFactory( connection );

        if ( prefix != null )
        {
            this.Prefix = prefix;
        }
    }

    /// <summary>
    /// Gets or sets the name of the Redis channel to use to exchange invalidation messages. The default value is <c>RedisCacheInvalidator</c>.
    /// </summary>
    public string ChannelName { get; init; } = nameof(RedisCacheSynchronizer);

    /// <summary>
    /// Gets or sets a value indicating whether determines whether disposing the <see cref="RedisCacheSynchronizer"/> also disposes the Redis connection. The default value is <c>false</c>.
    /// </summary>
    public bool OwnsConnection { get; init; }

    /// <summary>
    /// Gets or sets the time that the Redis invalidator will wait for a Redis connection.
    /// (When you create a new Redis invalidator, if it doesn't connect to a Redis server in this timeout, a <see cref="TimeoutException"/> is thrown.)
    /// </summary>
    /// <remarks>
    /// The default value is 1 minute.
    /// </remarks>
    public TimeSpan ConnectionTimeout { get; init; } = RedisNotificationQueue.DefaultSubscriptionTimeout;
}