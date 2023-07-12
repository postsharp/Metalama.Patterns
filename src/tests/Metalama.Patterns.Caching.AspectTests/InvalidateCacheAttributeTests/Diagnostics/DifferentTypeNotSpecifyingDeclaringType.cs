﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.Diagnostics;

public static class DifferentTypeNotSpecifyingDeclaringType
{
    public class CachingClass
    {
        [Cache]
        public object DoAction()
        {
            return null;
        }
    }

    public class InvalidatingClass
    {
        [InvalidateCache( nameof(CachingClass.DoAction) )]
        public void Invalidate() { }
    }
}