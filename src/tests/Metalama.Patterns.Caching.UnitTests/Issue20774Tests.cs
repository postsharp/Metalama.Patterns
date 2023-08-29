// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class Issue20774Tests : BaseCachingTests
    {
        [Fact]
        public void InvalidateBeforeCachedClassHasBeenTouched()
        {
            this.InitializeTestWithCachingBackend( nameof(Issue20774Tests) );

            try
            {
                // This shouldn't fail, even though the CachedClass type hasn't been touched yet.
                new InvalidatingClass().Invalidate();
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }

        public Issue20774Tests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }
    }
}