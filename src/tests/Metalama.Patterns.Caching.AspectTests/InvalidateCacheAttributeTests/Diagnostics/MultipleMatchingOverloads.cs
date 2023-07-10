﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.Diagnostics;

public static class MultipleMatchingOverloads
{
    public class CachingClass
    {
        [Cache(IgnoreThisParameter = true)]
        public object DoAction()
        {
            return null;
        }

        [Cache( IgnoreThisParameter = true )]
        public object DoAction( int param )
        {
            return null;
        }
    }

    public class InvalidatingClass
    {
        [InvalidateCache( typeof(CachingClass), nameof( CachingClass.DoAction ) )]
        public void Invalidate( int param )
        {
        }
    }
}