// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Tests.Backends;

public abstract class BaseInvalidationBrokerTests : BaseDistributedCacheTests
{
    protected BaseInvalidationBrokerTests( TestContext testContext ) : base( testContext ) { }

    protected abstract Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix );

    protected abstract CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix );

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var testId = Guid.NewGuid().ToString();

        return new[]
        {
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var testId = Guid.NewGuid().ToString();

        return new[]
        {
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
        };
    }
}