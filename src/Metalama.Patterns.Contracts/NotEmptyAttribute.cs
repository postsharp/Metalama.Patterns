// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentNullException"/> if the target is assigned a null or empty value.
/// The custom attributes can be added to locations of type <see cref="string"/> (where empty
/// means zero characters), or <see cref="ICollection"/>, <see cref="ICollection{T}"/> or <see cref="IReadOnlyCollection{T}"/>
/// (where empty means zero items). 
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractTextProvider.NotEmptyErrorMessage"/>.</para>
/// </remarks>
[PublicAPI]
[Inheritable]
public sealed class NotEmptyAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEmptyAttribute"/> class.
    /// </summary>
    public NotEmptyAttribute() { }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );

        // TODO: #33296 Fails during eligibility rule evaluation because TypeFactory.GetType leads to service is not available.
#if false
        builder.MustSatisfy(
            f => f.Type is INamedType t && (t.Equals( SpecialType.String ) || TryGetCompatibleTargetInterface( t, out _, out _ )), 
            f => $"the type of {f} must string or implement ICollection, ICollection<T> or IReadOnlyCollection<T>" );
#endif
    }

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var targetType = (INamedType) meta.Target.GetTargetType();

        if ( targetType.Equals( SpecialType.String ) )
        {
            if ( string.IsNullOrEmpty( value ) )
            {
                meta.Target.Project.ContractOptions().Templates.OnNotEmptyContractViolated( value );
            }
        }
        else if ( TryGetCompatibleTargetInterface( targetType, out var interfaceType, out var requiresCast ) )
        {
            if ( requiresCast )
            {
                if ( value == null || meta.Cast( interfaceType, value )!.Count <= 0 )
                {
                    meta.Target.Project.ContractOptions().Templates.OnNotEmptyContractViolated( value );
                }
            }
            else
            {
                if ( value == null || value!.Count <= 0 )
                {
                    meta.Target.Project.ContractOptions().Templates.OnNotEmptyContractViolated( value );
                }
            }
        }
        else
        {
            ThrowValidateCalledOnIneligibleTarget();
        }
    }

    // TODO: #33294 Use simpler mechanism to throw a compile-time exception from a template if/when available.
    [CompileTime]
    private static void ThrowValidateCalledOnIneligibleTarget() => throw new InvalidOperationException( "Validate called on an ineligible target." );

    [CompileTime]
    private static bool TryGetCompatibleTargetInterface(
        INamedType targetType,
        [NotNullWhen( true )] out INamedType? interfaceType,
        out bool requiresCast )
    {
        var typeOfICollection = (INamedType) TypeFactory.GetType( typeof(ICollection) );

        if ( targetType.Is( typeOfICollection ) )
        {
            var countProperty = typeOfICollection.Properties.Single( p => p.Name == nameof(ICollection.Count) );
            targetType.TryFindImplementationForInterfaceMember( countProperty, out var countPropertyImpl );

            interfaceType = typeOfICollection;
            requiresCast = countPropertyImpl is { IsExplicitInterfaceImplementation: true };

            return true;
        }
        else
        {
            var typeOfIReadOnlyCollection1 = (INamedType) TypeFactory.GetType( typeof(IReadOnlyCollection<>) );
            var typeOfICollection1 = (INamedType) TypeFactory.GetType( typeof(ICollection<>) );

            INamedType? foundInterface = null;

            foreach ( var t in targetType.GetSelfAndAllImplementedInterfaces() )
            {
                if ( t.IsGeneric )
                {
                    var originalDefinition = t.GetOriginalDefinition();

                    if ( originalDefinition.Equals( typeOfIReadOnlyCollection1 ) ||
                         originalDefinition.Equals( typeOfICollection1 ) )
                    {
                        foundInterface = t;

                        break;
                    }
                }
            }

            if ( foundInterface != null )
            {
                var countProperty = foundInterface.Properties.Single( p => p.Name == "Count" );
                targetType.TryFindImplementationForInterfaceMember( countProperty, out var countPropertyImpl );

                interfaceType = foundInterface;
                requiresCast = countPropertyImpl is { IsExplicitInterfaceImplementation: true };

                return true;
            }
        }

        interfaceType = null;
        requiresCast = false;

        return false;
    }
}