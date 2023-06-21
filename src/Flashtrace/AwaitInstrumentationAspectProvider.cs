// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Custom;
using System.Reflection;

namespace Flashtrace
{
    internal class AwaitInstrumentationAspectProvider : IAspectProvider
    {
        static readonly HashSet<MethodInfo> targetMethods = new HashSet<MethodInfo>();

        public IEnumerable<AspectInstance> ProvideAspects(object targetElement)
        {
            if ( PostSharpEnvironment.CurrentProject.TargetAssembly.GetName().Name == "PostSharp.Patterns.Common" )
            {
                // We don't have PostSharp.Patterns.Common.Weaver so we can't instrument ourselves.
                yield break;
            }

            Type[] types = new[] { typeof( Logger ),  typeof( LogLevelSource ) };

            IEnumerable<MethodInfo> methods = types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                    .Where(m => m.Name == "OpenAsyncActivity" || m.Name == "OpenActivity"));

            foreach ( MethodInfo method in methods)
            {
            
	            foreach (MethodUsageCodeReference usage in ReflectionSearch.GetMethodsUsingDeclaration(method) )
	            {
                    if ( usage.UsingMethod is not MethodInfo targetMethod )
                    {
                        continue;
                    }

                    if ( targetMethod.GetStateMachineKind() != StateMachineKind.Async )
	                {
	                    continue;
	                }

	                targetMethod = targetMethod.GetStateMachinePublicMethod();

	                // Ensure we add the aspect just once.
	                if (!targetMethods.Add(targetMethod))
	                    continue;

	                if ( !targetMethod.GetSemanticInfo().IsSelectable )
	                {
	                    CommonMessageSource.Instance.Write( targetMethod, SeverityType.Error, "COM011", method, targetMethod );
	                    continue;
	                }
						
	                yield return new AspectInstance(targetMethod, new AwaitInstrumentationAspect());
	            }
			}
        }
    }
}
