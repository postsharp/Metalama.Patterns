﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Caching.Implementation;

[CompileTime]
internal static class CompileTimeCacheConfigurationHelper
{
    public static CompileTimeCacheItemConfiguration GetConfigurationFromAttributes( IMethod method )
    {
        var configurations = new List<CompileTimeCacheItemConfiguration>();

        PopulateConfigurations( method.DeclaringType, configurations );

        var assemblyConfiguration = GetConfigurationOnDeclaration( method.DeclaringType.DeclaringAssembly );
        if ( assemblyConfiguration != null )
        {
            configurations.Add( assemblyConfiguration );
        }

        var mergedConfiguration = new CompileTimeCacheItemConfiguration();

        foreach ( var configuration in configurations )
        {
            mergedConfiguration.ApplyFallback( configuration );
        }

        return mergedConfiguration;
    }

    private static void PopulateConfigurations( INamedType type, List<CompileTimeCacheItemConfiguration> configurations )
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

    private static CompileTimeCacheItemConfiguration? GetConfigurationOnDeclaration( IDeclaration declaration )
    {
        var attributeType = (INamedType) TypeFactory.GetType( typeof( CacheConfigurationAttribute ) );
        var attr = declaration.Attributes.OfAttributeType( attributeType ).SingleOrDefault();
        return attr == null ? null : new CompileTimeCacheItemConfiguration( attr );
    }
}