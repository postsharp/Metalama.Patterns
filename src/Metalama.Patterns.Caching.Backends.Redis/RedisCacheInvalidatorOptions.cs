// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Options for <see cref="RedisCacheInvalidator"/>.
/// </summary>
[PublicAPI]
public class RedisCacheInvalidatorOptions : CacheInvalidatorOptions
{
    /// <summary>
    /// Gets or sets the name of the Redis channel to use to exchange invalidation messages. The default value is <c>RedisCacheInvalidator</c>.
    /// </summary>
    public string ChannelName { get; set; } = nameof(RedisCacheInvalidator);

    /// <summary>
    /// Gets or sets a value indicating whether determines whether disposing the <see cref="RedisCacheInvalidator"/> also disposes the Redis connection. The default value is <c>false</c>.
    /// </summary>
    public bool OwnsConnection { get; set; }

    /// <summary>
    /// Gets or sets the time that the Redis invalidator will wait for a Redis connection.
    /// (When you create a new Redis invalidator, if it doesn't connect to a Redis server in this timeout, a <see cref="TimeoutException"/> is thrown.)
    /// </summary>
    /// <remarks>
    /// The default value is 1 minute.
    /// </remarks>
    public TimeSpan ConnectionTimeout { get; set; } = RedisNotificationQueue.DefaultSubscriptionTimeout;
}