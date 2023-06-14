// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(PS-specific)

/* Ported from PostSharp, addresed issue #24728
 * Contracts: ILocationValidationAspect<uint> handles int values causing negative number overflow
 * 
 * Action: Skiping, PS specific.
 */

//#ExpectedMessage(LA0121)

using System;
using PostSharp.Aspects;
using Metalama.Patterns.Contracts;
using PostSharp.Reflection;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Issue24728
    {
        public static class Program
        {
            public static int Main()
            {
                try
                {
                    TestUInt(uint.MaxValue - 1);
                    TestInt(-2);
                }
                catch (ArgumentException)
                {
                    return 0;
                }

                return 1;
            }

            private static void TestInt([ValidateUInt] int foo)
            {
            }

            private static void TestUInt([ValidateInt] uint foo)
            {
            }
        }

        public class ValidateUInt : LocationContractAttribute, ILocationValidationAspect<uint>
        {
            public Exception ValidateValue(uint value, string locationName, LocationKind locationKind, LocationValidationContext context)
            {
                return null;
            }
        }

        public class ValidateInt : LocationContractAttribute, ILocationValidationAspect<int>
        {
            public Exception ValidateValue(int value, string locationName, LocationKind locationKind, LocationValidationContext context)
            {
                return null;
            }
        }
    }
}