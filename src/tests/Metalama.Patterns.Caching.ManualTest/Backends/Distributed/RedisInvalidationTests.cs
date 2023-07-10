using Metalama.Patterns.Caching.Tests.Backends.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metalama.Patterns.Caching.Implementation;
using Xunit;
using Metalama.Patterns.Caching.Backends.Redis;
using StackExchange.Redis;
using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Tests.Backends
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