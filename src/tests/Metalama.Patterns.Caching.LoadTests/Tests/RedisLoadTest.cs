// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Backends.Redis;
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

                collectors[i] = RedisCacheDependencyGarbageCollector.Create( collectorConnection, configuration: collectorConfiguration );
            }

            base.Test( configuration, duration );
        }
        finally
        {
            Task.WaitAll( collectors.Select( c => c.DisposeAsync().AsTask() ).ToArray() );
        }
    }

    private static IConnectionMultiplexer CreateConnection()
    {
        const string redisConnectionStringEnvironmentVariableName = "REDIS_CONNECTION_STRING";
        var redisConnectionString = Environment.GetEnvironmentVariable( redisConnectionStringEnvironmentVariableName );

        if ( string.IsNullOrEmpty( redisConnectionString ) )
        {
            throw new InvalidOperationException(
                $"We use Azure Cache for Redis for this test. We don't keep the service active. To perform the test, create a new Azure Cache for Redis and assign the connection string to '{redisConnectionStringEnvironmentVariableName}' environment variable. This can be done in the debugging profile, if needed." );
        }

        var connection = ConnectionMultiplexer.Connect( redisConnectionString );

        return connection;
    }

    protected override CachingBackend CreateCachingBackend()
    {
        var connection = CreateConnection();

        var configuration = new RedisCachingBackendConfiguration() { KeyPrefix = this._keyPrefix, OwnsConnection = true, SupportsDependencies = true };

        return CachingBackend.Create( b => b.Redis( connection ).WithConfiguration( configuration ) );
    }
}