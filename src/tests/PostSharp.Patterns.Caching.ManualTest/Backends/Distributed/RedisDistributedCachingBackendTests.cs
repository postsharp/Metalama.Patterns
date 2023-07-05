#if POSTSHARP_CACHING_REDIS

using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PostSharp.Patterns.Caching.Backends.Redis;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Tests.Backends.Distributed;
using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public class RedisDistributedCachingBackendTests : BaseDistributedCacheTests
    {
        public RedisDistributedCachingBackendTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override async Task<CachingBackend[]> CreateBackendsAsync()
        {
            string prefix = Guid.NewGuid().ToString();

            return new[]
                   {
                       await RedisFactory.CreateBackendAsync( this.TestContext, prefix, supportsDependencies:true ),
                       await RedisFactory.CreateBackendAsync( this.TestContext, prefix, supportsDependencies:true ),
                   };

        }

        protected override CachingBackend[] CreateBackends()
        {
            string prefix = Guid.NewGuid().ToString();

            return new[]
                   {
                       RedisFactory.CreateBackend( this.TestContext, prefix, supportsDependencies:true ),
                       RedisFactory.CreateBackend( this.TestContext, prefix, supportsDependencies:true ),
                   };
        }

        protected override void ConnectToRedisIfRequired()
        {
            RedisTestInstance redisTestInstance = RedisPersistentInstance.GetOrLaunchRedisInstance();
            this.TestContext.Properties["RedisEndpoint"] = redisTestInstance.Endpoint;
        }
    }
}

#endif