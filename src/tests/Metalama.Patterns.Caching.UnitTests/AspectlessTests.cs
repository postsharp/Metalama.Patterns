// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests;

public sealed class AspectlessTests : BaseCachingTests
{
    public AspectlessTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }

    [Fact]
    public void TestCacheHit()
    {
        this.InitializeTestWithCachingBackend( nameof(AspectlessTests) );

        try
        {
            var o = new C();

            var value1 = o.Get();
            var value2 = o.Get();

            Assert.Equal( value1, value2 );
        }
        finally
        {
            TestProfileConfigurationFactory.DisposeTest();
        }
    }

    private sealed class C
    {
        private int _invocations;

        public int Get()
        {
            var cachedMethod = CachedMethodMetadata.ForCallingMethod();

            Assert.Equal( nameof(this.Get), cachedMethod.Method.Name );

            return CachingService.Default.GetFromCacheOrExecute<int>( cachedMethod, this, Array.Empty<object?>(), ( instance, args ) => this._invocations++ );
        }
    }
}