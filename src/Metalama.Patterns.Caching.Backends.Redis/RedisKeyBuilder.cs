// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal sealed class RedisKeyBuilder
{
    public const string GarbageCollectionPrefix = "gc";
    public const string ValueKindPrefix = "value";
    public const string DependencyKindPrefix = "dependency";
    public const string DependenciesKindPrefix = "dependencies";

#pragma warning disable SA1401

    // ReSharper disable NotAccessedField.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly RedisKey HeartbeatKey;
    public readonly RedisChannel NotificationChannel;
    public readonly RedisChannel EventsChannel;
#pragma warning restore SA1401

    // ReSharper restore NotAccessedField.Global

    public string KeyPrefix { get; }

    public RedisKeyBuilder( IDatabase database, RedisCachingBackendConfiguration? configuration )
    {
        this.KeyPrefix = configuration?.KeyPrefix ?? "_";
        this.HeartbeatKey = this.KeyPrefix + ":" + GarbageCollectionPrefix + ":heartbeat";
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

    public bool TryParseKeyspaceNotification( string channelName, [NotNullWhen( true )] out string? keyKind, [NotNullWhen( true )] out string? itemKey )
    {
        keyKind = null;
        itemKey = null;

        var tokenizer = new StringTokenizer( channelName );

        // [Porting] Was `if ( tokenizer.GetNext() == null )`, but tokenizer.GetNext() never returns null. Updated check, behaviour may have changed. 
        if ( string.IsNullOrEmpty( tokenizer.GetNext() ) )
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