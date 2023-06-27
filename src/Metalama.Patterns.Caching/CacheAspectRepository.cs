// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Metalama.Patterns.Caching;

// TODO: [Porting] Probably conceptually inapplicable - caches aspect instances at runtime?
internal static class CacheAspectRepository
{
    private static readonly ConcurrentDictionary<MethodInfo, ICacheAspect> _configurations = new();

    // ReSharper disable once UnusedMember.Global : Would originally have been called from woven code?
    public static void Add( MethodInfo method, ICacheAspect aspect )
        =>

            // We need the aspect initialization to run as soon as the module is loaded (module initializer)
            _configurations[method] = aspect;

    public static ICacheAspect? Get( MethodInfo method )
    {
        var genericMethod = GetGenericDefinition( method );

        if ( !_configurations.TryGetValue( genericMethod, out var configuration ) )
        {
            if ( !genericMethod.IsDefined( typeof(CacheAttribute) ) )
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0}.{1} method is not enhanced by the [Cache] aspect.",
                        method.DeclaringType!.Name,
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
        if ( method is { IsGenericMethod: true, IsGenericMethodDefinition: false } ||
             method.DeclaringType is { IsGenericType: true, IsGenericTypeDefinition: false } )
        {
            return (MethodInfo) method.Module.ResolveMethod( method.MetadataToken )!;
        }
        else
        {
            return method;
        }
    }
}