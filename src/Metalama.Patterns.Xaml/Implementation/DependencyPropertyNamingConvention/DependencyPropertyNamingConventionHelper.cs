// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Diagnostics;
using System.Windows;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal static class DependencyPropertyNamingConventionHelper
{
    public static DependencyPropertyNamingConventionMatch Match<TMatchPropertyChangingNamePredicate, TMatchPropertyChangedNamePredicate, TMatchValidateNamePredicate>(
        INamingConvention namingConvention,
        IProperty targetProperty,
        InspectedDeclarationsAdder inspectedDeclarations,
        string registrationFieldName,
        in TMatchPropertyChangingNamePredicate matchPropertyChangingPredicate,
        in TMatchPropertyChangedNamePredicate matchPropertyChangedPredicate,
        in TMatchValidateNamePredicate matchValidateNamePredicate,
        bool requirePropertyChangingMatch = false,
        bool requirePropertyChangedMatch = false,
        bool requireValidateMatch = false )
        where TMatchPropertyChangingNamePredicate : INameMatchPredicate
        where TMatchPropertyChangedNamePredicate : INameMatchPredicate
        where TMatchValidateNamePredicate : INameMatchPredicate
    {
        IsValidResult<ChangeHandlerSignatureKind> IsPropertyChangingMethodValid( IMethod method, InspectedDeclarationsAdder inspectedDeclarations )
        {
            var signature = GetChangeHandlerSignature( method, targetProperty, false );
            var isValid = signature != ChangeHandlerSignatureKind.Invalid;
            inspectedDeclarations.Add( method, isValid, DependencyPropertyAspectBuilder._propertyChangingMethodCategory );
            return new IsValidResult<ChangeHandlerSignatureKind>( isValid, signature );
        }

        IsValidResult<ChangeHandlerSignatureKind> IsPropertyChangedMethodValid( IMethod method, InspectedDeclarationsAdder inspectedDeclarations )
        {
            var signature = GetChangeHandlerSignature( method, targetProperty, true );
            var isValid = signature != ChangeHandlerSignatureKind.Invalid;
            inspectedDeclarations.Add( method, isValid, DependencyPropertyAspectBuilder._propertyChangedMethodCategory );
            return new IsValidResult<ChangeHandlerSignatureKind>( isValid, signature );
        }

        IsValidResult<ValidationHandlerSignatureKind> IsValidateMethodValid( IMethod method, InspectedDeclarationsAdder inspectedDeclarations )
        {
            var signature = GetValidationHandlerSignature( method, targetProperty );
            var isValid = signature != ValidationHandlerSignatureKind.Invalid;
            inspectedDeclarations.Add( method, isValid, DependencyPropertyAspectBuilder._propertyChangedMethodCategory );
            return new IsValidResult<ValidationHandlerSignatureKind>( isValid, signature );
        }

        var declaringType = targetProperty.DeclaringType;

        var conflictingMember = (IMemberOrNamedType?) declaringType.AllMembers().FirstOrDefault( m => m.Name == registrationFieldName )
                                ?? declaringType.NestedTypes.FirstOrDefault( t => t.Name == registrationFieldName );

        var registrationFieldConflictMatch = DeclarationMatch<IMemberOrNamedType>.SuccessOrConflict( conflictingMember );

        var findPropertyChangingResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchPropertyChangingPredicate,
            IsPropertyChangingMethodValid,
            inspectedDeclarations );

        var findPropertyChangedResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchPropertyChangedPredicate,
            IsPropertyChangedMethodValid,
            inspectedDeclarations );

        var findValidateResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchValidateNamePredicate,
            IsValidateMethodValid,
            inspectedDeclarations );

        return new DependencyPropertyNamingConventionMatch(
            namingConvention,
            registrationFieldName,
            registrationFieldConflictMatch,
            findPropertyChangingResult.Match,
            findPropertyChangedResult.Match,
            findValidateResult.Match,
            findPropertyChangingResult.Metadata,
            findPropertyChangedResult.Metadata,
            findValidateResult.Metadata,
            requirePropertyChangingMatch,
            requirePropertyChangedMatch,
            requireValidateMatch );
    }

    private static ChangeHandlerSignatureKind GetChangeHandlerSignature(
        IMethod method,
        IProperty targetProperty,
        bool allowOldValue )
    {
        var declaringType = targetProperty.DeclaringType;
        var propertyType = targetProperty.Type;

        var p = method.Parameters;

        if ( method.ReturnType.SpecialType != SpecialType.Void || p.Count > 2 || p.Any( parameter => parameter.RefKind is not (RefKind.None or RefKind.In) ) )
        {
            return ChangeHandlerSignatureKind.Invalid;
        }

        switch ( p.Count )
        {
            case 0:
                return method.IsStatic ? ChangeHandlerSignatureKind.StaticNoParameters : ChangeHandlerSignatureKind.InstanceNoParameters;

            case 1:

                if ( p[0].Type.Equals( typeof( DependencyProperty ) ) )
                {
                    return method.IsStatic ? ChangeHandlerSignatureKind.StaticDependencyProperty : ChangeHandlerSignatureKind.InstanceDependencyProperty;
                }
                else if ( method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || p[0].Type.Equals( declaringType )
                              || p[0].Type.Equals( typeof( DependencyObject ) )) )
                {
                    return ChangeHandlerSignatureKind.StaticInstance;
                }
                else if ( !method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[0].Type )
                              || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ChangeHandlerSignatureKind.InstanceValue;
                }

                break;

            case 2:

                if ( method.IsStatic
                     && p[0].Type.Equals( typeof( DependencyProperty ) )
                     && (p[1].Type.SpecialType == SpecialType.Object
                         || p[1].Type.Equals( declaringType )
                         || p[1].Type.Equals( typeof( DependencyObject ) )) )
                {
                    return ChangeHandlerSignatureKind.StaticDependencyPropertyAndInstance;
                }
                else if ( allowOldValue
                          && !method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[0].Type )
                              || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1))
                          && (p[1].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[1].Type )
                              || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ChangeHandlerSignatureKind.InstanceOldValueAndNewValue;
                }

                break;
        }

        return ChangeHandlerSignatureKind.Invalid;
    }

    private static ValidationHandlerSignatureKind GetValidationHandlerSignature(
        IMethod method,
        IProperty targetProperty )
    {
        var declaringType = targetProperty.DeclaringType;
        var propertyType = targetProperty.Type;

        var p = method.Parameters;

        if ( method.ReturnType.SpecialType != SpecialType.Boolean
             || method.ReturnParameter.RefKind != RefKind.None
             || p.Count > 3
             || p.Any( parameter => parameter.RefKind is not (RefKind.None or RefKind.In) ) )
        {
            return ValidationHandlerSignatureKind.Invalid;
        }

        switch ( p.Count )
        {
            case 0:
                return ValidationHandlerSignatureKind.Invalid;

            case 1:

                if ( p[0].Type.SpecialType == SpecialType.Object
                     || propertyType.Is( p[0].Type )
                     || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1) )
                {
                    return method.IsStatic ? ValidationHandlerSignatureKind.StaticValue : ValidationHandlerSignatureKind.InstanceValue;
                }

                break;

            case 2:

                if ( p[0].Type.Equals( typeof( DependencyProperty ) )
                     && (p[1].Type.SpecialType == SpecialType.Object
                         || propertyType.Is( p[1].Type )
                         || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return method.IsStatic
                        ? ValidationHandlerSignatureKind.StaticDependencyPropertyAndValue
                        : ValidationHandlerSignatureKind.InstanceDependencyPropertyAndValue;
                }
                else if ( method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || p[0].Type.Equals( declaringType )
                              || p[0].Type.Equals( typeof( DependencyObject ) ))
                          && (p[1].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[1].Type )
                              || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ValidationHandlerSignatureKind.StaticInstanceAndValue;
                }

                break;

            case 3:

                if ( method.IsStatic
                     && p[0].Type.Equals( typeof( DependencyProperty ) )
                     && (p[1].Type.SpecialType == SpecialType.Object
                         || p[1].Type.Equals( declaringType )
                         || p[1].Type.Equals( typeof( DependencyObject ) ))
                     && (p[2].Type.SpecialType == SpecialType.Object
                         || propertyType.Is( p[2].Type )
                         || (p[2].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ValidationHandlerSignatureKind.StaticDependencyPropertyAndInstanceAndValue;
                }

                break;
        }

        return ValidationHandlerSignatureKind.Invalid;
    }
}