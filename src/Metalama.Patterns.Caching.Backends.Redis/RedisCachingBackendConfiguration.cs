// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Serializers;
using ISerializer = Metalama.Patterns.Caching.Serializers.ISerializer;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Configuration for <see cref="RedisCachingBackend"/>.
/// </summary>
[PublicAPI]
public class RedisCachingBackendConfiguration : ICloneable
{
    private string? _keyPrefix = "cache";
    private int _database = -1;
    private Func<ISerializer>? _createSerializer;
    private bool _ownsConnection;
    private int _transactionMaxRetries = 5;
    private bool _supportsDependencies;
    private bool _isLocallyCached;
    private TimeSpan _connectionTimeout = RedisNotificationQueue.DefaultSubscriptionTimeout;
    private TimeSpan _defaultExpiration = TimeSpan.FromDays( 1 );

    /// <summary>
    /// Gets or sets the prefix for the key of all Redis items created by the <see cref="RedisCachingBackend"/>. The default value is <c>cache</c>.
    /// </summary>
    public string? KeyPrefix
    {
        get => this._keyPrefix;
        set
        {
            this.CheckFrozen();

            if ( value != null
#if NETFRAMEWORK || NETSTANDARD
                 && value.IndexOf( ":", StringComparison.Ordinal ) != -1
#else
                 && value.Contains( ":", StringComparison.Ordinal )
#endif
               )
            {
                throw new ArgumentOutOfRangeException( nameof(value), "The KeyPrefix property value cannot contain the ':' character." );
            }

            this._keyPrefix = value;
        }
    }

    /// <summary>
    /// Gets or sets the index of the database to use. The default value is <c>-1</c> (automatic selection).
    /// </summary>
    public int Database
    {
        get => this._database;
        set
        {
            this.CheckFrozen();
            this._database = value;
        }
    }

    /// <summary>
    /// Gets or sets a function that creates the serializer used to serialize objects into byte arrays (and conversely).
    /// The default value is <c>null</c>, which means that <see cref="BinarySerializer"/> will be used.
    /// </summary>
    public Func<ISerializer>? CreateSerializer
    {
        get => this._createSerializer;
        set
        {
            this.CheckFrozen();
            this._createSerializer = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether determines whether the <see cref="RedisCachingBackend"/> should dispose the Redis connection when the <see cref="RedisCachingBackend"/>
    /// itself is disposed.
    /// </summary>
    public bool OwnsConnection
    {
        get => this._ownsConnection;
        set
        {
            this.CheckFrozen();
            this._ownsConnection = value;
        }
    }

    /// <summary>
    /// Gets or sets the number of times Redis transactions are retried when they fail due to a data conflict, before an exception
    /// is raised. The default value is <c>5</c>.
    /// </summary>
    public int TransactionMaxRetries
    {
        get => this._transactionMaxRetries;
        set
        {
            this.CheckFrozen();
            this._transactionMaxRetries = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="RedisCachingBackend"/> should support dependencies. When this property is used,
    /// the <see cref="DependenciesRedisCachingBackend"/> class is used instead of <see cref="RedisCachingBackend"/>. When dependencies
    /// are enabled, at least one instance of the <see cref="RedisCacheDependencyGarbageCollector"/> MUST run.
    /// </summary>
    public bool SupportsDependencies
    {
        get => this._supportsDependencies;
        set
        {
            this.CheckFrozen();
            this._supportsDependencies = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a <see cref="MemoryCachingBackend"/> should be added in front of the <see cref="RedisCachingBackend"/>.
    /// </summary>
    public bool IsLocallyCached
    {
        get => this._isLocallyCached;
        set
        {
            this.CheckFrozen();
            this._isLocallyCached = value;
        }
    }

    /// <summary>
    /// Gets or sets the default expiration time of cached items.
    /// All items that don't have an explicit expiration time are automatically expired according to the value
    /// of this property, unless they have the <see cref="CacheItemPriority.NotRemovable"/> priority.
    /// The default value is 1 day.
    /// </summary>
    public TimeSpan DefaultExpiration
    {
        get => this._defaultExpiration;
        set
        {
            this.CheckFrozen();
            this._defaultExpiration = value;
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> if the object has been frozen.
    /// </summary>
    /// <exception cref="InvalidOperationException">The object can no longer be modified.</exception>
    protected void CheckFrozen()
    {
        if ( this.IsFrozen )
        {
            throw new InvalidOperationException( "This object can no longer be modified." );
        }
    }

    internal void Freeze()
    {
        this.IsFrozen = true;
    }

    object ICloneable.Clone() => this.Clone();

    /// <summary>
    /// Returns a non-frozen clone of the current instance.
    /// </summary>
    /// <returns></returns>
    public RedisCachingBackendConfiguration Clone()
    {
        var clone = (RedisCachingBackendConfiguration) this.MemberwiseClone();
        clone.IsFrozen = false;

        return clone;
    }

    /// <summary>
    /// Gets a value indicating whether the current instance is frozen (i.e. read-only).
    /// </summary>
    public bool IsFrozen { get; private set; }

    /// <summary>
    /// Gets or sets the time that the Redis backend will wait for a Redis connection.
    /// (When you create a new Redis backend, if it doesn't connect to a Redis server in this timeout, a <see cref="TimeoutException"/> is thrown.)
    /// </summary>
    /// <remarks>
    /// The default value is 1 minute.
    /// </remarks>
    public TimeSpan ConnectionTimeout
    {
        get => this._connectionTimeout;
        set
        {
            this.CheckFrozen();
            this._connectionTimeout = value;
        }
    }
}