// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests.Backends.Single
{
    // ReSharper disable once UnusedType.Global
    public sealed class MemoryCachingBackendTests : BaseCacheBackendTests
    {
        public MemoryCachingBackendTests( CachingTestOptions cachingTestOptions, ITestOutputHelper testOutputHelper ) : base(
            cachingTestOptions,
            testOutputHelper ) { }

        protected override CheckAfterDisposeCachingBackend CreateBackend()
        {
            return new CheckAfterDisposeCachingBackend( MemoryCacheFactory.CreateBackend( this.ServiceProvider ) );
        }
    }

    public sealed class SerializingMemoryCachingBackendTests : BaseCacheBackendTests
    {
        public SerializingMemoryCachingBackendTests( CachingTestOptions cachingTestOptions, ITestOutputHelper testOutputHelper ) : base(
            cachingTestOptions,
            testOutputHelper ) { }

        protected override CheckAfterDisposeCachingBackend CreateBackend()
        {
            return new CheckAfterDisposeCachingBackend( MemoryCacheFactory.CreateBackend( this.ServiceProvider, withSerializer: true ) );
        }
    }
}