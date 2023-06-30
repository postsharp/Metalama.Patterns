// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Reflection;

namespace Metalama.Patterns.Caching;

// TODO: [Porting] Remove BuildTimeCacheConfigurationManager if still unused after Metalama aspect development. 
// ReSharper disable once UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
#pragma warning disable IDE0051
internal static class BuildTimeCacheConfigurationManager
{
    public static IRunTimeCacheItemConfiguration GetConfigurationFromAttributes( MemberInfo method )
    {
        List<IRunTimeCacheItemConfiguration> configurations = new();

        PopulateConfigurations( method.DeclaringType!, configurations );

        var assemblyConfiguration = GetConfigurationOnDeclaration( method.DeclaringType!.Assembly );

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

    private static CacheConfigurationAttribute? GetConfigurationAttribute( ICustomAttributeProvider declaration )
    {
        var attributes = declaration.GetCustomAttributes( typeof(CacheConfigurationAttribute), false );

        if ( attributes.Length > 0 )
        {
            return (CacheConfigurationAttribute) attributes[0];
        }
        else
        {
            return null;
        }
    }

    private static void PopulateConfigurations( Type type, List<IRunTimeCacheItemConfiguration> configurations )
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

    // TODO: [Porting] Remove these disables if class is retained.
    // ReSharper disable once ReturnTypeCanBeNotNullable
    // ReSharper disable once UnusedParameter.Local
    private static IRunTimeCacheItemConfiguration? GetConfigurationOnDeclaration( ICustomAttributeProvider declaration )
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
        // TODO: [Porting] Remove this disable if class is retained.
#pragma warning disable SA1401
        public readonly Dictionary<object, CacheConfigurationAttribute> Attributes = new();
#pragma warning restore SA1401
    }
}