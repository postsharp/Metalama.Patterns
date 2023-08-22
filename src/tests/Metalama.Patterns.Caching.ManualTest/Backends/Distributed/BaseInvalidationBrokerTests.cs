﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

public abstract class BaseInvalidationBrokerTests : BaseDistributedCacheTests
{
    protected BaseInvalidationBrokerTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

    protected abstract Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix );

    protected abstract CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix );

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var testId = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        var testId = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_1" ) ), testId ),
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_2" ) ), testId ),
            this.CreateInvalidationBroker( new MemoryCachingBackend( new MemoryCache( "_3" ) ), testId )
        };
    }
}