﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal static class DependencyPropertyNamingConventionHelper
{
    public static DependencyPropertyNamingConventionMatch Match<TMatchPropertyChangingNamePredicate, TMatchPropertyChangedNamePredicate,
                                                                TMatchValidateNamePredicate>(
        INamingConvention namingConvention,
        IProperty targetProperty,
        InspectedMemberAdder inspectedMember,
        string dependencyPropertyName,
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
        var assets = targetProperty.Compilation.Cache.GetOrAdd( _ => new DependencyPropertyAssets() );

        IsValidResult<ChangeHandlerSignatureKind> IsPropertyChangingMethodValid( IMethod method, InspectedMemberAdder inspectedDeclarations1 )
        {
            var signature = GetChangeHandlerSignature( method, targetProperty, assets, false );
            var isValid = signature != ChangeHandlerSignatureKind.Invalid;
            inspectedDeclarations1.Add( method, isValid, DependencyPropertyAspectBuilder.PropertyChangingMethodCategory );

            return new IsValidResult<ChangeHandlerSignatureKind>( isValid, signature );
        }

        IsValidResult<ChangeHandlerSignatureKind> IsPropertyChangedMethodValid( IMethod method, InspectedMemberAdder inspectedDeclarations1 )
        {
            var signature = GetChangeHandlerSignature( method, targetProperty, assets, true );
            var isValid = signature != ChangeHandlerSignatureKind.Invalid;
            inspectedDeclarations1.Add( method, isValid, DependencyPropertyAspectBuilder.PropertyChangedMethodCategory );

            return new IsValidResult<ChangeHandlerSignatureKind>( isValid, signature );
        }

        IsValidResult<ValidationHandlerSignatureKind> IsValidateMethodValid( IMethod method, InspectedMemberAdder inspectedDeclarations1 )
        {
            var signature = GetValidationHandlerSignature( method, targetProperty, assets );
            var isValid = signature != ValidationHandlerSignatureKind.Invalid;
            inspectedDeclarations1.Add( method, isValid, DependencyPropertyAspectBuilder.PropertyChangedMethodCategory );

            return new IsValidResult<ValidationHandlerSignatureKind>( isValid, signature );
        }

        var declaringType = targetProperty.DeclaringType;

        var conflictingMember = (IMemberOrNamedType?) declaringType.AllMembers().FirstOrDefault( m => m.Name == registrationFieldName )
                                ?? declaringType.NestedTypes.FirstOrDefault( t => t.Name == registrationFieldName );

        var registrationFieldConflictMatch = MemberMatch<IMemberOrNamedType>.SuccessOrConflict( conflictingMember );

        var findPropertyChangingResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchPropertyChangingPredicate,
            IsPropertyChangingMethodValid,
            inspectedMember );

        var findPropertyChangedResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchPropertyChangedPredicate,
            IsPropertyChangedMethodValid,
            inspectedMember );

        var findValidateResult = declaringType.Methods.FindValidMatchingDeclaration(
            matchValidateNamePredicate,
            IsValidateMethodValid,
            inspectedMember );

        return new DependencyPropertyNamingConventionMatch(
            namingConvention,
            dependencyPropertyName,
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
        DependencyPropertyAssets assets,
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

                if ( p[0].Type.Equals( assets.DependencyProperty ) )
                {
                    return method.IsStatic ? ChangeHandlerSignatureKind.StaticDependencyProperty : ChangeHandlerSignatureKind.InstanceDependencyProperty;
                }
                else if ( method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || p[0].Type.Equals( declaringType )
                              || p[0].Type.Equals( assets.DependencyObject )) )
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
                     && p[0].Type.Equals( assets.DependencyProperty )
                     && (p[1].Type.SpecialType == SpecialType.Object
                         || p[1].Type.Equals( declaringType )
                         || p[1].Type.Equals( assets.DependencyObject )) )
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
        IProperty targetProperty,
        DependencyPropertyAssets assets )
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

                if ( p[0].Type.Equals( assets.DependencyProperty )
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
                              || p[0].Type.Equals( assets.DependencyObject ))
                          && (p[1].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[1].Type )
                              || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ValidationHandlerSignatureKind.StaticInstanceAndValue;
                }

                break;

            case 3:

                if ( method.IsStatic
                     && p[0].Type.Equals( assets.DependencyProperty )
                     && (p[1].Type.SpecialType == SpecialType.Object
                         || p[1].Type.Equals( declaringType )
                         || p[1].Type.Equals( assets.DependencyObject ))
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