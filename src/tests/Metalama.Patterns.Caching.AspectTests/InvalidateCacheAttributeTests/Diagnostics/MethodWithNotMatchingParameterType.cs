﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.Diagnostics;

public static class MethodWithNotMatchingParameterType
{
    public class CachingClass
    {
        [Cache( IgnoreThisParameter = true )]
        public object DoAction( CachedValueChildClass param )
        {
            return null;
        }
    }

    public class InvalidatingClass
    {
        [InvalidateCache( typeof(CachingClass), nameof(CachingClass.DoAction) )]
        public void Invalidate( CachedValueClass param ) { }
    }
}