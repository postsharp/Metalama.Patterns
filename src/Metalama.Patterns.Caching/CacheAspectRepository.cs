// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using PostSharp.Patterns.Caching.Implementation;

namespace PostSharp.Patterns.Caching
{
    internal static class CacheAspectRepository
    {
        private static readonly ConcurrentDictionary<MethodInfo,ICacheAspect> configurations = new ConcurrentDictionary<MethodInfo, ICacheAspect>();



        public static void Add( MethodInfo method, ICacheAspect aspect )
        {
            // We need the aspect initialization to run as soon as the module is loaded (module initializer)
            configurations[method] = aspect;
        }

        public static ICacheAspect Get( MethodInfo method )
        {
            ICacheAspect configuration;
            MethodInfo genericMethod = GetGenericDefinition( method );
            if ( !configurations.TryGetValue(genericMethod , out configuration ) )
            {
                if ( !genericMethod.IsDefined(typeof(CacheAttribute) ) )
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                              "The {0}.{1} method is not enhanced by the [Cache] aspect.",
                                                              method.DeclaringType.Name,
                                                              method.Name ) );
                }
                else
                {
                    // The class has not been initialized.
                    return null;
                }
            }
            return configuration;
        }

        private static MethodInfo GetGenericDefinition( MethodInfo method )
        {
            if ( (method.IsGenericMethod && !method.IsGenericMethodDefinition) ||
                 (method.DeclaringType.IsGenericType && !method.DeclaringType.IsGenericTypeDefinition) )
            {
                return (MethodInfo) method.Module.ResolveMethod( method.MetadataToken );
            }
            else
            {
                return method;
            }

        }


    }
}
