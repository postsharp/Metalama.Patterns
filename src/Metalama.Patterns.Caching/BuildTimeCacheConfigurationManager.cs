// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Reflection;

namespace Metalama.Patterns.Caching;

internal static class BuildTimeCacheConfigurationManager
{
    public static ICacheItemConfiguration GetConfigurationFromAttributes( MemberInfo method )
    {
        List<ICacheItemConfiguration> configurations = new();

        PopulateConfigurations( method.DeclaringType, configurations );

        var assemblyConfiguration = GetConfigurationOnDeclaration( method.DeclaringType.Assembly );

        if ( assemblyConfiguration != null )
        {
            configurations.Add( assemblyConfiguration );
        }

        var mergedConfiguration = new CacheItemConfiguration();

        foreach ( var configuration in configurations )
        {
            mergedConfiguration.ApplyFallback( configuration );
        }

        return mergedConfiguration;
    }

    private static CacheConfigurationAttribute GetConfigurationAttribute( ICustomAttributeProvider declaration )
    {
        object[] attributes = declaration.GetCustomAttributes( typeof(CacheConfigurationAttribute), false );

        if ( attributes.Length > 0 )
        {
            return (CacheConfigurationAttribute) attributes[0];
        }
        else
        {
            return null;
        }
    }

    private static void PopulateConfigurations( Type type, List<ICacheItemConfiguration> configurations )
    {
        var configuration = GetConfigurationOnDeclaration( type );

        if ( configuration != null )
        {
            configurations.Add( configuration );
        }

        var baseType = type.BaseType;

        if ( baseType != null )
        {
            PopulateConfigurations( baseType, configurations );
        }
    }

    // TODO: [Porting] Use of PostSharpEnvironment.CurrentProject.StateStore

    private static ICacheItemConfiguration GetConfigurationOnDeclaration( ICustomAttributeProvider declaration )
    {
        throw new NotImplementedException( "Porting - PostSharpEnvironment.CurrentProject.StateStore" );
#if TODO
            CacheConfigurationAttributeCache attributeCache = PostSharpEnvironment.CurrentProject.StateStore.Get<CacheConfigurationAttributeCache>();

            if ( attributeCache == null )
            {
                attributeCache = new CacheConfigurationAttributeCache();
                PostSharpEnvironment.CurrentProject.StateStore.Set( attributeCache );
            }

            CacheConfigurationAttribute attribute;
            if ( !attributeCache.Attributes.TryGetValue( declaration, out attribute ) )
            {
                attribute = GetConfigurationAttribute( declaration );
                attributeCache.Attributes[declaration] = attribute;
            }

            return attribute?.Configuration;
#endif
    }

    private sealed class CacheConfigurationAttributeCache
    {
        public readonly Dictionary<object, CacheConfigurationAttribute> Attributes = new();
    }
}