// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
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

        static bool RequiresVerification( IHasType d ) => d is { Type: { IsReferenceType: true, IsNullable: false }, RefKind: RefKind.None };

        // Add aspects to fields and properties.
        types
            .SelectMany(
                t => t.Properties
                    .Cast<IFieldOrProperty>()
                    .Union( t.Fields )
                    .Where( f => IsVisible( f ) && RequiresVerification( f ) ) )
            .RequireAspect<NotNullAttribute>();

        // Add aspects to method parameters.
        types.SelectMany( t => t.Methods )
            .Where( IsVisible )
            .SelectMany( t => t.Parameters )
            .Where( RequiresVerification )
            .RequireAspect<NotNullAttribute>();
    }
}