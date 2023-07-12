using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.LoadTests.Tests
{
    class RedisLoadTestConfiguration : LoadTestConfiguration
    {
        public int CollectorsCount { get; set; }
    }

    class RedisLoadTest : BaseTestClass<RedisLoadTestConfiguration>
    {
        private readonly string keyPrefix = Guid.NewGuid().ToString();

        public override void Test( RedisLoadTestConfiguration configuration, TimeSpan duration )
        {
            Console.WriteLine( "collector init" );

            RedisCacheDependencyGarbageCollector[] collectors = new RedisCacheDependencyGarbageCollector[configuration.CollectorsCount];

            try
            {
                for ( int i = 0; i < collectors.Length; i++ )
                {
                    RedisCachingBackendConfiguration collectorConfiguration = new RedisCachingBackendConfiguration()
                                                                              {
                                                                                  KeyPrefix = this.keyPrefix,
                                                                                  OwnsConnection = true
                                                                              };

                    IConnectionMultiplexer collectorConnection = CreateConnection();

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
            string connectionString =
                "postsharp-test.redis.cache.windows.net:6380,password=zVXBseSX6KMMKaMJ13iyWwCWUIIUqrqIoKAlm882CzE=,ssl=True,abortConnect=False";

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect( connectionString );

            return connection;
        }

        protected override CachingBackend CreateCachingBackend()
        {
            IConnectionMultiplexer connection = CreateConnection();

            RedisCachingBackendConfiguration configuration = new RedisCachingBackendConfiguration()
                                                             {
                                                                 KeyPrefix = this.keyPrefix,
                                                                 OwnsConnection = true,
                                                                 SupportsDependencies = true
                                                             };

            return RedisCachingBackend.Create( connection, configuration );
        }
    }
}