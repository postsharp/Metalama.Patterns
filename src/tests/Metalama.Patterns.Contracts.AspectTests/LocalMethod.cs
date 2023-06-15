// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(PostSharp-specific and/or not a Contracts concern)

/* Ported from PostSharp.
 * 
 * Action: Skiping, PS specific (or at least a fabric concern, not a Contracts concern).
 */

// #MinToolsVersion(15.0)
// #ExpectedMessage(LA0167//"PostSharp\\.Patterns\\.Common\\.BuildTests\\.Contracts\\.LocalMethod\\.Program\\.Main\\.MyPrivateMethod")

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PostSharp.Aspects;
using PostSharp.Reflection;

namespace Metalama.Patterns.Contracts.AspectTests
{
    namespace LocalMethod
    {
        [MyAspectProvider]
        public static class Program
        {
            public static int Main()
            {
                void MyPrivateMethod(string h)
                {
                    Console.WriteLine(h);
                }

                MyPrivateMethod("abc");

                return 0;
            }
        }


        class MyAspectProvider : TypeLevelAspect, IAspectProvider
        {
            public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
            {
                Type targetType = (Type) targetElement;
                foreach (MethodInfo method in targetType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                                                                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance))
                {
                    ParameterInfo parameter = method.GetParameters().FirstOrDefault(p => p.Name == "h");
                    if (parameter != null)
                    {
                        yield return new AspectInstance(parameter, new ObjectConstruction(typeof(RequiredAttribute).GetConstructor(Type.EmptyTypes)));
                    }
                }
            }
        }

    }
}
