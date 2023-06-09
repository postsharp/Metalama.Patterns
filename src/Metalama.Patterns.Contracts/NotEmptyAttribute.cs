// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Collections;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentNullException"/> if the target is assigned a null or empty value.
/// The custom attributes can be added to locations of type <see cref="string"/> (where empty
/// means zero characters), or <see cref="ICollection"/>, <see cref="ICollection{T}"/> or <see cref="IReadOnlyCollection{T}"/>  (where empty means zero items). 
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.NotEmptyErrorMessage"/>.</para>
/// </remarks>
public sealed class NotEmptyAttribute : ContractAspect
{
    public override void Validate( dynamic? value )
    {
        CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );

        var typeOfICollection = (INamedType) TypeFactory.GetType( typeof( ICollection ) );

        var targetType = (INamedType) CompileTimeHelpers.GetTargetType( meta.Target );

        if ( targetType.Equals( SpecialType.String ) )
        {
            if ( string.IsNullOrEmpty( value ) )
            {                
                throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    typeof( ArgumentNullException ),
                    typeof( NotEmptyAttribute ),
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    ContractLocalizedTextProvider.NotEmptyErrorMessage ) );
            }
        }
        else if ( targetType.Is( typeOfICollection ) )
        {
            var countProperty = typeOfICollection.Properties.Single( p => p.Name == nameof( ICollection.Count ) );
            targetType.TryFindImplementationForInterfaceMember( countProperty, out var countPropertyImpl );

            if ( countPropertyImpl is { IsExplicitInterfaceImplementation: true } )
            {
                if ( value == null || meta.Cast( typeOfICollection, value )!.Count <= 0 )
                {
                    throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                        typeof( ArgumentNullException ),
                        typeof( NotEmptyAttribute ),
                        value,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.NotEmptyErrorMessage ) );
                }
            }
            else
            {
                if ( value == null || value!.Count <= 0 )
                {
                    throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                        typeof( ArgumentNullException ),
                        typeof( NotEmptyAttribute ),
                        value,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.NotEmptyErrorMessage ) );
                }
            }
        }
        else
        {
            // TODO: Consider moving interface discovery to eligibility and set tags?

            var typeOfIReadOnlyCollection1 = (INamedType) TypeFactory.GetType( typeof( IReadOnlyCollection<> ) );
            var typeOfICollection1 = (INamedType) TypeFactory.GetType( typeof( ICollection<> ) );

            INamedType? foundInterface = null;

            foreach ( var interfaceType in GetSelfAndAllImplementedInterfaces( targetType ) )
            {
                if ( interfaceType.IsGeneric )
                {
                    var originalDefinition = interfaceType.GetOriginalDefinition();
                    if ( originalDefinition.Equals( typeOfIReadOnlyCollection1 ) || originalDefinition.Equals( typeOfICollection1 ) )
                    {
                        foundInterface = interfaceType;
                        break;
                    }
                }
            }

            if ( foundInterface != null )
            {

                var countProperty = foundInterface.Properties.Single( p => p.Name == "Count" );
                targetType.TryFindImplementationForInterfaceMember( countProperty, out var countPropertyImpl );

                if ( countPropertyImpl is { IsExplicitInterfaceImplementation: true } )
                {
                    if ( value == null || meta.Cast( foundInterface, value )!.Count <= 0 )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentNullException ),
                            typeof( NotEmptyAttribute ),
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            ContractLocalizedTextProvider.NotEmptyErrorMessage ) );
                    }
                }
                else
                {
                    if ( value == null || value!.Count <= 0 )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentNullException ),
                            typeof( NotEmptyAttribute ),
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            ContractLocalizedTextProvider.NotEmptyErrorMessage ) );
                    }
                }
            }
        }
    }

    [CompileTime]
    static IEnumerable<INamedType> GetSelfAndAllImplementedInterfaces( INamedType type )
    {        
        if ( type.TypeKind == TypeKind.Interface )
        {
            yield return type;
        }

        foreach ( var i in type.AllImplementedInterfaces )
        {
            yield return i;
        }
    }
}