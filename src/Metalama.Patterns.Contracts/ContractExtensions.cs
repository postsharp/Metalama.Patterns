// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.CodeFixes;

namespace Metalama.Patterns.Contracts;

[CompileTime]
[PublicAPI]
public static class ContractExtensions
{
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

        var requiredAttribute = (INamedType) TypeFactory.GetType( typeof(RequiredAttribute) );
        var notNullableAttribute = (INamedType) TypeFactory.GetType( typeof(NotNullAttribute) );

        static bool IsNullableType( IHasType d ) => d is { Type: { IsReferenceType: true, IsNullable: false }, RefKind: RefKind.None };

        IAttribute? GetNotNullAspectAttribute( IDeclaration d )
            => d.Attributes.OfAttributeType( requiredAttribute ).FirstOrDefault() ??
               d.Attributes.OfAttributeType( notNullableAttribute ).FirstOrDefault();

        // Add aspects to fields and properties.
        var fieldsAndProperties = types
            .SelectMany(
                t => t.Properties
                    .Cast<IFieldOrProperty>()
                    .Union( t.Fields ) )
            .Where( f => IsVisible( f ) && IsNullableType( f ) && f.Attributes.All( a => a.Type.Name != "AllowNullAttribute" ) );

        fieldsAndProperties
            .Where( f => GetNotNullAspectAttribute( f ) == null && f.Writeability is Writeability.InitOnly or Writeability.All )
            .RequireAspect<NotNullAttribute>();

        // Add aspects to method parameters.
        var parameters = types
            .SelectMany( t => t.Methods.Cast<IMethodBase>().Concat( t.Constructors ) )
            .Where( IsVisible )
            .SelectMany( t => t.Parameters )
            .Where( IsNullableType );

        parameters
            .Where( parameter => GetNotNullAspectAttribute( parameter ) == null )
            .RequireAspect<NotNullAttribute>();

        // Warn if the attribute is duplicate.
        fieldsAndProperties.Where( f => GetNotNullAspectAttribute( f ) != null )
            .ReportDiagnostic(
                f =>
                {
                    var nullableAttribute = GetNotNullAspectAttribute( f );

                    return ContractDiagnostics.ContractRedundant.WithArguments( (f, nullableAttribute!.Type.Name) )
                        .WithCodeFixes( CodeFixFactory.RemoveAttributes( f, nullableAttribute.Type ) );
                } );

        parameters.Where( f => GetNotNullAspectAttribute( f ) != null )
            .ReportDiagnostic(
                f =>
                {
                    var nullableAttribute = GetNotNullAspectAttribute( f );

                    return ContractDiagnostics.ContractRedundant.WithArguments( (f, nullableAttribute!.Type.Name) )
                        .WithCodeFixes( CodeFixFactory.RemoveAttributes( f, nullableAttribute.Type ) );
                } );
    }

    public static ContractOptions GetContractOptions( this IMetaTarget target ) => target.Declaration.GetContractOptions();

    public static ContractOptions GetContractOptions( this IMethod declaration ) => declaration.Enhancements().GetOptions<ContractOptions>();

    public static ContractOptions GetContractOptions( this INamedType declaration ) => declaration.Enhancements().GetOptions<ContractOptions>();

    public static ContractOptions GetContractOptions( this IFieldOrPropertyOrIndexer declaration ) => declaration.Enhancements().GetOptions<ContractOptions>();

    public static ContractOptions GetContractOptions( this IParameter declaration ) => declaration.Enhancements().GetOptions<ContractOptions>();

    public static ContractOptions GetContractOptions( this IDeclaration declaration )
        => declaration switch
        {
            IParameter parameter => parameter.Enhancements().GetOptions<ContractOptions>(),
            IFieldOrPropertyOrIndexer field => field.Enhancements().GetOptions<ContractOptions>(),
            INamedType namedType => namedType.Enhancements().GetOptions<ContractOptions>(),
            IMethod method => method.Enhancements().GetOptions<ContractOptions>(),
            _ => throw new ArgumentOutOfRangeException()
        };
}