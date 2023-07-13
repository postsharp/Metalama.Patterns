// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.Diagnostics;

public static class WithNoCachingMethodName
{
    public class CachingClass
    {
        [Cache]
        public object DoAction()
        {
            return null!;
        }

        [InvalidateCache]
        public void Invalidate() { }
    }
}