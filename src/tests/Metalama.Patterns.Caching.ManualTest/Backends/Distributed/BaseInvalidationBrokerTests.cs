// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Microsoft.Extensions.Caching.Memory;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

public abstract class BaseInvalidationBrokerTests : BaseDistributedCacheTests
{
    protected BaseInvalidationBrokerTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

    protected abstract Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix );

    protected override async Task<CachingBackend[]> CreateBackendsAsync()
    {
        var testId = Guid.NewGuid().ToString();

        return new CachingBackend[]
        {
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend(), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend(), testId ),
            await this.CreateInvalidationBrokerAsync( new MemoryCachingBackend(), testId )
        };
    }

    protected override CachingBackend[] CreateBackends()
    {
        return this.CreateBackendsAsync().Result;
    }
}