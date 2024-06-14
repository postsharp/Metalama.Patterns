﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if TEST_OPTIONS
// @RemoveOutputCode
#endif

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.Diagnostics;

public static class SameTypeAndNonExistingMethod
{
    public class CachingClass
    {
        [Cache]
        public object DoAction()
        {
            return null!;
        }

        [InvalidateCache( "NonExistingMethod" )]
        public void Invalidate() { }
    }
}