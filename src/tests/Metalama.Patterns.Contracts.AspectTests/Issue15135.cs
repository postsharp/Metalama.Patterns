// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

/* Ported from PostSharp, addressed issue #15135 
 * LA0199 when adding code contract with IAspectProvider
 * 
 * Action: Skiping, PS specific.
 */

// @Skipped(PS-specific)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;
using PostSharp.Aspects.Serialization;
using PostSharp.Extensibility;
using PostSharp.Reflection;

namespace Metalama.Patterns.Contracts.AspectTests
{
    namespace Issue15135
    {
        interface IProgram
        {

            [NotNull]
            string Foo([NotNull] string bar);
        }

        class Program : IProgram
        {
            static int Main(string[] args)
            {

                IProgram p = new Program();

                try
                {
                    p.Foo(null);
                    return 1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    p.Foo("a");
                    return 2;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return 0;

            }


            public string Foo(string bar)
            {
                return null;
            }

        }

        [AttributeUsage(
            AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
            AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
            AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
        [MulticastAttributeUsage(MulticastTargets.Method | MulticastTargets.Parameter, Inheritance =
            MulticastInheritance.Strict)]
        internal sealed class NotNullAttribute : MulticastAttribute, IAspectProvider
        {
            public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
            {
                var target = targetElement;

                var targetMethod = target as MethodInfo;


                if (targetMethod != null)
                {
                    if (targetMethod.IsAbstract || targetMethod.DeclaringType.Namespace.StartsWith("JetBrains."))
                        yield break;

                    target = targetMethod.ReturnParameter;
                }
                else
                {
                    var targetParameter = (ParameterInfo)target;
                    var targetMember = (MethodBase)targetParameter.Member;
                    if (targetMember.IsAbstract || targetMember.DeclaringType.Namespace.StartsWith("JetBrains."))
                        yield break;
                }

                yield return new AspectInstance(target, new ObjectConstruction(typeof(RequiredAttribute)));
            }
        }

    }
}
