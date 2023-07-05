// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

// @ExpectedMessage(CAC005)

using PostSharp.Patterns.Caching.TestHelpers;

namespace PostSharp.Patterns.Caching.BuildTests.InvalidateCacheAttribute
{
    namespace MethodWithNotMatchingParameterType
    {
        public class CachingClass
        {
            [Caching.Cache( IgnoreThisParameter = true )]
            public object DoAction( CachedValueChildClass param )
            {
                return null;
            }
        }

        public class InvalidatingClass
        {
            [Caching.InvalidateCache( typeof(CachingClass), nameof( CachingClass.DoAction ) )]
            public void Invalidate( CachedValueClass param )
            {
            }
        }

        public class Program
        {
            public static void Main()
            {
            }
        }
    }
}
