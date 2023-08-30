// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.CodeFixes;
using Metalama.Framework.Project;

namespace Metalama.Patterns.Contracts;

[CompileTime]
public static class ContractExtensions
{
    public static ContractOptions ContractOptions( this IProject project ) => project.Extension<ContractOptions>();

    public static void VerifyNotNullableDeclarations( this IAspectReceiver<ICompilation> compilation, bool includeInternalApis = false )
    {
        bool IsVisible( IMemberOrNamedType t ) => includeInternalApis || t.Accessibility is Accessibility.Public or Accessibility.ProtectedInternal;

        // Select types.
        var types = compilation.SelectMany( t => t.AllTypes )
            .Where( IsVisible );

        types.VerifyNotNullableDeclarations( includeInternalApis );
    }

    public static void VerifyNotNullableDeclarations( this IAspectReceiver<INamespace> ns, bool includeInternalApis = false )
    {
        bool IsVisible( IMemberOrNamedType t ) => includeInternalApis || t.Accessibility is Accessibility.Public or Accessibility.ProtectedInternal;

        // Select types.
        var types = ns.SelectMany( t => t.Types )
            .Where( IsVisible );

        types.VerifyNotNullableDeclarations( includeInternalApis );
    }

    public static void VerifyNotNullableDeclarations( this IAspectReceiver<INamedType> types, bool includeInternalApis = false )
    {
        bool IsVisible( IMemberOrNamedType t ) => includeInternalApis || t.Accessibility is Accessibility.Public or Accessibility.ProtectedInternal;

        static bool IsNullableType( IHasType d ) => d is { Type: { IsReferenceType: true, IsNullable: false }, RefKind: RefKind.None };

        static IAttribute? GetNullableAttribute( IDeclaration d )
            => d.Attributes.OfAttributeType( typeof(RequiredAttribute) ).FirstOrDefault() ??
               d.Attributes.OfAttributeType( typeof(NotNullAttribute) ).FirstOrDefault() ??
               d.Attributes.OfAttributeType( typeof(NotEmptyAttribute) ).FirstOrDefault();

        // Add aspects to fields and properties.
        var fieldsAndProperties = types
            .SelectMany(
                t => t.Properties
                    .Cast<IFieldOrProperty>()
                    .Union( t.Fields ) );

        fieldsAndProperties
            .Where( f => IsVisible( f ) && IsNullableType( f ) && GetNullableAttribute( f ) != null )
            .RequireAspect<NotNullAttribute>();

        // Add aspects to method parameters.
        var parameters = types.SelectMany( t => t.Methods )
            .Where( IsVisible )
            .SelectMany( t => t.Parameters );

        parameters
            .Where( parameter => IsNullableType( parameter ) && GetNullableAttribute( parameter ) != null )
            .RequireAspect<NotNullAttribute>();

        // Warn if the attribute is duplicate.
        fieldsAndProperties.Where( f => IsNullableType( f ) && GetNullableAttribute( f ) != null )
            .ReportDiagnostic(
                f =>
                {
                    var nullableAttribute = GetNullableAttribute( f );

                    return ContractDiagnostics.ContractRedundant.WithArguments( (f, nullableAttribute!.Type.Name) )
                        .WithCodeFixes( CodeFixFactory.RemoveAttributes( f, nullableAttribute.Type ) );
                } );

        parameters.Where( f => IsNullableType( f ) && GetNullableAttribute( f ) != null )
            .ReportDiagnostic(
                f =>
                {
                    var nullableAttribute = GetNullableAttribute( f );

                    return ContractDiagnostics.ContractRedundant.WithArguments( (f, nullableAttribute!.Type.Name) )
                        .WithCodeFixes( CodeFixFactory.RemoveAttributes( f, nullableAttribute.Type ) );
                } );
    }
}