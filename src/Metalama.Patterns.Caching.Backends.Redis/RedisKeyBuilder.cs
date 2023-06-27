// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using StackExchange.Redis;
using System.Globalization;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal class RedisKeyBuilder
{
    public const string GarbageCollectionPrefix = "gc";
    public const string ValueKindPrefix = "value";
    public const string DependencyKindPrefix = "dependency";
    public const string DependenciesKindPrefix = "dependencies";

    public readonly RedisKey HearbeatKey;
    public readonly RedisChannel NotificationChannel;
    public readonly RedisChannel EventsChannel;

    public string KeyPrefix { get; private set; }

    public RedisKeyBuilder( IDatabase database, RedisCachingBackendConfiguration configuration )
    {
        this.KeyPrefix = configuration?.KeyPrefix ?? "_";
        this.HearbeatKey = this.KeyPrefix + ":" + GarbageCollectionPrefix + ":heartbeat";
        this.EventsChannel = this.KeyPrefix + ":events";
        this.NotificationChannel = string.Format( CultureInfo.InvariantCulture, "__keyspace@{0}__:{1}*", database.Database, this.KeyPrefix );
    }

    public RedisKey GetDependencyKey( string dependency )
    {
        return this.KeyPrefix + ":" + DependencyKindPrefix + ":" + dependency;
    }

    public RedisKey GetValueKey( string item )
    {
        return this.KeyPrefix + ":" + ValueKindPrefix + ":" + item;
    }

    public RedisKey GetDependenciesKey( string item )
    {
        return this.KeyPrefix + ":" + DependenciesKindPrefix + ":" + item;
    }

    public bool TryParseKeyspaceNotification( string channelName, out string keyKind, out string itemKey )
    {
        keyKind = null;
        itemKey = null;

        var tokenizer = new StringTokenizer( channelName );

        if ( tokenizer.GetNext() == null )
        {
            return false;
        }

        var prefix = tokenizer.GetNext();

        if ( prefix != this.KeyPrefix )
        {
            return false;
        }

        keyKind = tokenizer.GetNext();
        itemKey = tokenizer.GetRest();

        return true;
    }
}