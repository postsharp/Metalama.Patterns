// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(PS-specific)

/* Ported from PostSharp, addresed issue #4165
 * Runtime System.MissingMethodException when a method/field signature contains ValueType
 * 
 * Action: Skiping, PS specific.
 */

using System;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Issue4165
    {
        public static class Program
        {
            public static int Main()
            {
                TestClass test = new TestClass();
                test.Timestamp = DateTime.UtcNow;

                return 0;
            }
        }

        public class TestClass
        {
            [PostSharp.Patterns.Model.BuildTests.Contracts.Issue4165.Dependency.NotDefault]
            public DateTime Timestamp { get; set; }
        }
    }
}