// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Microsoft.Extensions.Caching.Memory;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single
{
    // ReSharper disable once UnusedType.Global
    public sealed class MemoryCachingBackendTests : BaseCacheBackendTests
    {
        public MemoryCachingBackendTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

        protected override CachingBackend CreateBackend()
        {
            return MemoryCacheFactory.CreateBackend();
        }
    }
}