// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Reflection;

namespace Metalama.Patterns.Caching
{
    internal static class BuildTimeCacheConfigurationManager
    {

        public static ICacheItemConfiguration GetConfigurationFromAttributes( MemberInfo method )
        {
            List<ICacheItemConfiguration> configurations = new List<ICacheItemConfiguration>();

            PopulateConfigurations( method.DeclaringType, configurations );

            ICacheItemConfiguration assemblyConfiguration = GetConfigurationOnDeclaration( method.DeclaringType.GetAssembly() );
            if ( assemblyConfiguration != null )
            {
                configurations.Add( assemblyConfiguration );
            }

            CacheItemConfiguration mergedConfiguration = new CacheItemConfiguration();

            foreach ( ICacheItemConfiguration configuration in configurations )
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
            ICacheItemConfiguration configuration = GetConfigurationOnDeclaration( type );

            if ( configuration != null )
            {
                configurations.Add( configuration );
            }

            Type baseType = type.GetBaseType();

            if ( baseType != null )
            {
                PopulateConfigurations( baseType, configurations );
            }
        }


        private static ICacheItemConfiguration GetConfigurationOnDeclaration( ICustomAttributeProvider declaration )
        {
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
        }

        private sealed class CacheConfigurationAttributeCache
        {
            public readonly Dictionary<object, CacheConfigurationAttribute> Attributes = new Dictionary<object, CacheConfigurationAttribute>();
        }
    }
}
