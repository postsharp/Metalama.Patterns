using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed
{
    public class SimpleRedisDistributedTest : BaseDistributedCacheTests
    {
        public SimpleRedisDistributedTest( TestContext testContext ) : base( testContext )
        {
        }

        protected override bool TestDependencies { get; } = false;

        protected override async Task<CachingBackend[]> CreateBackendsAsync()
        {
            string prefix = Guid.NewGuid().ToString();
            
            return new[]
                   {
                       await RedisFactory.CreateBackendAsync( TestContext, prefix ),
                       await RedisFactory.CreateBackendAsync( TestContext, prefix ),
                   };

        }

        protected override CachingBackend[] CreateBackends()
        {
            string prefix = Guid.NewGuid().ToString();

            return new[]
                   {
                       RedisFactory.CreateBackend( TestContext, prefix ),
                       RedisFactory.CreateBackend( TestContext, prefix ),
                   };
        }

        protected override void ConnectToRedisIfRequired()
        {
            RedisTestInstance redisTestInstance = RedisPersistentInstance.GetOrLaunchRedisInstance();
            this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }
    }
}