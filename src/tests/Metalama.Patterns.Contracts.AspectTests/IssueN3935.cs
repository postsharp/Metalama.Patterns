// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(Covered by unit tests in NotNullAttributeTests)

/* Ported from PostSharp, addresed issue #3935
 * [NotNull] constraint cannot be added to parameter of generic parameter with constraint
 * 
 * Action:
 *  Implemented as unit tests in NotNullAttributeTests:
 *      Given_MethodWithNotNullGenericParameter_When_NotNullPassed_Then_Success
 *      Given_MethodWithNotNullGenericParameter_When_NotNullPassed_Then_ExceptionIsThrown
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;
using Metalama.Patterns.Contracts;
using PostSharp.Reflection;
using PostSharp.Serialization;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace IssueN3935
    {
        class A { }

        class B<T0> where T0 : A
        {
            public B([NotNull] T0 x) { }
        }

        class Program
        {
            static void Main(string[] args)
            {
            }
        }
    }
}
