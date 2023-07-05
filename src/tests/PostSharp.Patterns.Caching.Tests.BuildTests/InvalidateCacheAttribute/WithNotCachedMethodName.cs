// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

// @ExpectedMessage(CAC002)

namespace PostSharp.Patterns.Caching.BuildTests.InvalidateCacheAttribute
{
    namespace WithNotCachedMethodName
    {
        public class CachingClass
        {
            public object DoAction()
            {
                return null;
            }

            [Caching.InvalidateCache(nameof(DoAction))]
            public void Invalidate()
            { }
        }

        public class Program
        {
            public static void Main()
            {
            }
        }
    }
}
