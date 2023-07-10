﻿// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

// @ExpectedMessage(CAC008)

namespace PostSharp.Patterns.Caching.BuildTests.InvalidateCacheAttribute
{
    namespace SameTypeAndNonExistingMethod
    {
        public class CachingClass
        {
            [Caching.Cache]
            public object DoAction()
            {
                return null;
            }

            [Caching.InvalidateCache( "NonExistingMethod" )]
            public void Invalidate()
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