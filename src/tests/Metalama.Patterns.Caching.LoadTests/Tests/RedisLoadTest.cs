// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.LoadTests.Tests;

internal sealed class RedisLoadTest : BaseTestClass<RedisLoadTestConfiguration>
{
    private readonly string _keyPrefix = Guid.NewGuid().ToString();

    public override void Test( RedisLoadTestConfiguration configuration, TimeSpan duration )
    {
        Console.WriteLine( "collector init" );

        var collectors = new RedisCacheDependencyGarbageCollector[configuration.CollectorsCount];

        try
        {
            for ( var i = 0; i < collectors.Length; i++ )
            {
                var collectorConfiguration = new RedisCachingBackendConfiguration() { KeyPrefix = this._keyPrefix, OwnsConnection = true };

                var collectorConnection = CreateConnection();

                collectors[i] = RedisCacheDependencyGarbageCollector.Create( collectorConnection, collectorConfiguration );
            }

            base.Test( configuration, duration );
        }
        finally
        {
            Task.WaitAll( collectors.Select( c => c.DisposeAsync() ).ToArray() );
        }
    }

    private static IConnectionMultiplexer CreateConnection()
    {
        // ReSharper disable StringLiteralTypo
        const string connectionString = "postsharp-test.redis.cache.windows.net:6380,password=zVXBseSX6KMMKaMJ13iyWwCWUIIUqrqIoKAlm882CzE=,ssl=True,abortConnect=False";
        
        // ReSharper restore StringLiteralTypo

        var connection = ConnectionMultiplexer.Connect( connectionString );

        return connection;
    }

    protected override CachingBackend CreateCachingBackend()
    {
        var connection = CreateConnection();

        var configuration = new RedisCachingBackendConfiguration() { KeyPrefix = this._keyPrefix, OwnsConnection = true, SupportsDependencies = true };

        return RedisCachingBackend.Create( connection, configuration );
    }
}