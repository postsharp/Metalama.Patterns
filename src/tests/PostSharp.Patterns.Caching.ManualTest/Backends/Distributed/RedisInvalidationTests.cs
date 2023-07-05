#if POSTSHARP_CACHING_REDIS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;
using Xunit;
using PostSharp.Patterns.Caching.Backends.Redis;
using StackExchange.Redis;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.Tests.Backends.Distributed;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public class RedisInvalidationTests : BaseInvalidationBrokerTests
    {
        public RedisInvalidationTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
        {
            return await RedisCacheInvalidator.CreateAsync(
                backend,
                RedisFactory.CreateConnection( this.TestContext ),
                new RedisCacheInvalidatorOptions {Prefix = prefix, OwnsConnection = true} );
        }

        protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
        {
            return RedisCacheInvalidator.Create(
                backend,
                RedisFactory.CreateConnection( this.TestContext ),
                new RedisCacheInvalidatorOptions {Prefix = prefix, OwnsConnection = true} );
        }

        protected override void ConnectToRedisIfRequired()
        {
            RedisFactory.CreateTestInstance( this.TestContext );
        }
    }
}

#endif