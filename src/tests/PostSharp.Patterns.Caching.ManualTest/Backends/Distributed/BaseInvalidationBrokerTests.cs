using Xunit;
using PostSharp.Patterns.Caching.Dependencies;
using PostSharp.Patterns.Caching.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public abstract class BaseInvalidationBrokerTests : BaseDistributedCacheTests
    {
        protected BaseInvalidationBrokerTests( TestContext testContext ) : base( testContext )
        {
        }

        protected abstract Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix );
        protected abstract CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix );

        protected override async Task<CachingBackend[]> CreateBackendsAsync()
        {
            string testId = Guid.NewGuid().ToString();


            return new[]
                   {
                       await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
                       await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
                       await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
                   };
        }

        protected override CachingBackend[] CreateBackends()
        {
            string testId = Guid.NewGuid().ToString();


            return new[]
                   {
                        this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
                       this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
                       this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
                   };
        }
    }
}
