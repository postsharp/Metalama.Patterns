// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(Covered by unit tests in RegularExpressionAttributeTests)

/* Ported from PostSharp, addresed issue #5808
 * RegularExpressionAttribute includes the regular expression into the error message without escaping
 * 
 * Action:
 *  Implemented as unit tests in RegularExpressionAttributeTests:
 *      Given_FieldWithRegexMatch_When_IncorrectValuePassed_Then_ExceptionMessageIsCorrect_1
 *      Given_FieldWithRegexMatch_When_IncorrectValuePassed_Then_ExceptionMessageIsCorrect_2
 */

using System;
using Metalama.Patterns.Contracts;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Issue5808
    {
        public static class Program
        {
            public static int Main()
            {
                try
                {
                    TestProperty1 = "hello";
                }
                catch ( ArgumentException e )
                {
                    if ( e.Message.StartsWith( @"The property 'TestProperty1' must match the regular expression '^[a-z]{4}$'." ) )
                        goto test2;

                    return 1;
                }

                return -1;

                test2:
                try
                {
                    TestProperty2 = "{hello}";
                }
                catch ( ArgumentException e )
                {
                    if ( e.Message.StartsWith( @"The property 'TestProperty2' must match the regular expression '^\{[a-z]{4}}$'." ) )
                        return 0;

                    return 2;
                }

                return -1;
            }

            [RegularExpression( @"^[a-z]{4}$" )]
            public static string TestProperty1 { get; set; }

            [RegularExpression( @"^\{[a-z]{4}}$" )]
            public static string TestProperty2 { get; set; }
        }
    }
}