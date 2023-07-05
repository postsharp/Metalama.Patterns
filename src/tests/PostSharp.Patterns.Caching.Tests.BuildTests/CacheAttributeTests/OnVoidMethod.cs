// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

// @ExpectedMessage(CAC001)

using System;

namespace PostSharp.Patterns.Caching.BuildTests.CacheAttributeTests
{
    namespace OnVoidMethod
    {
        public class CachingClass
        {
            [Cache]
            public void DoAction()
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
