// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Caching.Tests.Assets.Issue20774;
using Xunit;

namespace Metalama.Patterns.Caching.Tests
{
    public class Issue20774Tests
    {
        [Fact]
        public void InvalidateBeforeCachedClassHasBeenTouched()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( nameof( Issue20774Tests ) );

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
    }
}