using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PostSharp.Patterns.Caching.Backends.Redis;
using PostSharp.Patterns.Caching.Implementation;
using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Tests.Backends.Distributed
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