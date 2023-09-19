// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Aspects.Helpers;

[CompileTime]
internal static class CachingAspectConfigurationHelper
{
    // TODO: Consider caching this method at compile time once compile-time user caching is supported by the framework.
    private static CachingAspectConfiguration GetConfigurationFromAttributes( IMethod method )
    {
        var configurations = new List<CachingAspectConfiguration>();

        PopulateConfigurations( method.DeclaringType, configurations );

        var assemblyConfiguration = GetConfigurationOnDeclaration( method.DeclaringType.DeclaringAssembly );

        if ( assemblyConfiguration != null )
        {
            configurations.Add( assemblyConfiguration );
        }

        var mergedConfiguration = new CachingAspectConfiguration();

        foreach ( var configuration in configurations )
        {
            mergedConfiguration = mergedConfiguration.ApplyFallbackValues( configuration );
        }

        return mergedConfiguration;
    }

    private static void PopulateConfigurations( INamedType type, List<CachingAspectConfiguration> configurations )
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

    private static CachingAspectConfiguration? GetConfigurationOnDeclaration( IDeclaration declaration )
    {
        if ( declaration.Attributes.Count == 0 )
        {
            return null;
        }

        var attributeType = (INamedType) TypeFactory.GetType( typeof(CachingConfigurationAttribute) );
        var attr = declaration.Attributes.OfAttributeType( attributeType ).SingleOrDefault();

        return attr == null ? null : ParseAttribute( attr );
    }

    /// <summary>
    /// Applies the effective configuration of the method by applying fallback configuration from <see cref="CachingConfigurationAttribute"/>
    /// attributes on ancestor types and the declaring assembly.
    /// </summary>
    public static CachingAspectConfiguration ApplyEffectiveConfiguration( this CachingAspectConfiguration configuration, IMethod method )
    {
        var configurationFromAttributes = GetConfigurationFromAttributes( method );

        return configuration.ApplyFallbackValues( configurationFromAttributes );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingAspectConfiguration"/> class from the given
    /// custom attribute which must have construction semantics equivalent to <see cref="CachingConfigurationAttribute"/>
    /// and <see cref="CacheAttribute"/> (which both have the same construction semantics).
    /// </summary>
    public static CachingAspectConfiguration ParseAttribute( this IAttribute attribute )
    {
        if ( attribute == null )
        {
            throw new ArgumentNullException( nameof(attribute) );
        }

        if ( !attribute.Type.Is( typeof(ICachingConfigurationAttribute) ) )
        {
            throw new ArgumentOutOfRangeException(
                nameof(attribute),
                $"Must be an {nameof(ICachingConfigurationAttribute)}." );
        }

        try
        {
            T? GetAttributeValueType<T>( string name )
                where T : struct
            {
                if ( attribute.TryGetNamedArgument( name, out var value ) )
                {
                    return (T) value.Value!;
                }
                else
                {
                    return null;
                }
            }

            T? GetAttributeReferenceType<T>( string name )
                where T : class
            {
                if ( attribute.TryGetNamedArgument( name, out var value ) )
                {
                    return (T?) value.Value;
                }
                else
                {
                    return null;
                }
            }

            TimeSpan? GetAttributeTimeSpan( string name )
            {
                if ( attribute.TryGetNamedArgument( name, out var value ) )
                {
                    return TimeSpan.FromMinutes( (double) value.Value! );
                }
                else
                {
                    return default;
                }
            }

            return new CachingAspectConfiguration
            {
                ProfileName = GetAttributeReferenceType<string>( nameof(ICachingConfigurationAttribute.ProfileName) ),
                AutoReload = GetAttributeValueType<bool>( nameof(ICachingConfigurationAttribute.AutoReload) ),
                AbsoluteExpiration = GetAttributeTimeSpan( nameof(ICachingConfigurationAttribute.AbsoluteExpiration) ),
                SlidingExpiration = GetAttributeTimeSpan( nameof(ICachingConfigurationAttribute.SlidingExpiration) ),
                Priority = GetAttributeValueType<CacheItemPriority>( nameof(ICachingConfigurationAttribute.Priority) ),
                IgnoreThisParameter = GetAttributeValueType<bool>( nameof(ICachingConfigurationAttribute.IgnoreThisParameter) ),
                UseDependencyInjection = GetAttributeValueType<bool>( nameof(ICachingConfigurationAttribute.UseDependencyInjection) )
            };
        }
        catch ( Exception e )
        {
            throw new CachingAssertionFailedException( "The construction semantics of the attribute are not as expected.", e );
        }
    }

    public static AttributeConstruction ToAttributeConstruction( this CachingAspectConfiguration configuration )
    {
        var args = new Dictionary<string, object?>();

        if ( configuration.AbsoluteExpiration.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.AbsoluteExpiration)] = configuration.AbsoluteExpiration.Value.TotalMinutes;
        }

        if ( configuration.AutoReload.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.AutoReload)] = configuration.AutoReload;
        }

        if ( configuration.IgnoreThisParameter.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.IgnoreThisParameter)] = configuration.IgnoreThisParameter;
        }

        if ( configuration.Priority.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.Priority)] = configuration.Priority;
        }

        if ( configuration.SlidingExpiration.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.SlidingExpiration)] = configuration.SlidingExpiration.Value.TotalMinutes;
        }

        if ( configuration.ProfileName != null )
        {
            args[nameof(CachingConfigurationAttribute.ProfileName)] = configuration.ProfileName;
        }

        if ( configuration.UseDependencyInjection != null )
        {
            args[nameof(CachingConfigurationAttribute.UseDependencyInjection)] = configuration.UseDependencyInjection;
        }

        return AttributeConstruction.Create( typeof(CacheAttribute), namedArguments: args.ToList() );
    }
}