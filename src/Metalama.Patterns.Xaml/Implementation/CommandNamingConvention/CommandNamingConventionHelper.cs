// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal static class CommandNamingConventionHelper
{
    public static CommandNamingConventionMatch Match<TMatchCanExecuteNamePredicate>(
        INamingConvention namingConvention,
        IMethod executeMethod,
        InspectedDeclarationsAdder inspectedDeclarations,
        string commandPropertyName,
        in TMatchCanExecuteNamePredicate matchCanExecuteNamePredicate,
        bool considerMethod = true,
        bool considerProperty = true,
        bool requireCanExecuteMatch = false )
        where TMatchCanExecuteNamePredicate : INameMatchPredicate
    {
        var declaringType = executeMethod.DeclaringType;

        var conflictingMember = (IMemberOrNamedType?) declaringType.AllMembers().FirstOrDefault( m => m.Name == commandPropertyName )
                                ?? declaringType.NestedTypes.FirstOrDefault( t => t.Name == commandPropertyName );

        var commandPropertyConflictMatch = DeclarationMatch<IMemberOrNamedType>.SuccessOrConflict( conflictingMember );

        DeclarationMatch<IMethod>? canExecuteMethodMatch = null;

        if ( considerMethod )
        {
            canExecuteMethodMatch =
                executeMethod.DeclaringType.Methods.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecuteMethodDelegate,
                    inspectedDeclarations );
        }

        DeclarationMatch<IProperty>? canExecutePropertyMatch = null;

        if ( considerProperty )
        {
            canExecutePropertyMatch =
                executeMethod.DeclaringType.Properties.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecutePropertyDelegate,
                    inspectedDeclarations );
        }

        if ( canExecuteMethodMatch?.Outcome == DeclarationMatchOutcome.Success && canExecutePropertyMatch?.Outcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                DeclarationMatch<IMember>.Ambiguous(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                canExecuteMethodMatch.Value.ForDeclarationType<IMember>(),
                requireCanExecuteMatch );
        }
        else if ( canExecutePropertyMatch?.Outcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                canExecutePropertyMatch.Value.ForDeclarationType<IMember>(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == DeclarationMatchOutcome.Ambiguous || canExecutePropertyMatch?.Outcome == DeclarationMatchOutcome.Ambiguous )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                DeclarationMatch<IMember>.Ambiguous(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == DeclarationMatchOutcome.Invalid || canExecutePropertyMatch?.Outcome == DeclarationMatchOutcome.Invalid )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                DeclarationMatch<IMember>.Invalid(),
                requireCanExecuteMatch );
        }
        else
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                DeclarationMatch<IMember>.NotFound( matchCanExecuteNamePredicate ),
                requireCanExecuteMatch );
        }
    }

    internal static Func<IMethod, InspectedDeclarationsAdder, bool> IsValidCanExecuteMethodDelegate { get; } = IsValidCanExecuteMethod;

    private static bool IsValidCanExecuteMethod( IMethod method, InspectedDeclarationsAdder inspectedDeclarations )
    {
        var isValid = IsValidCanExecuteMethod( method );
        inspectedDeclarations.Add( method, isValid, CommandAttribute.CanExecuteMethodCategory );

        return isValid;
    }

    internal static Func<IProperty, InspectedDeclarationsAdder, bool> IsValidCanExecutePropertyDelegate { get; } = IsValidCanExecuteProperty;

    private static bool IsValidCanExecuteProperty( IProperty property, InspectedDeclarationsAdder inspectedDeclarations )
    {
        var isValid = IsValidCanExecuteProperty( property );
        inspectedDeclarations.Add( property, isValid, CommandAttribute.CanExecutePropertyCategory );

        return isValid;
    }

    private static bool IsValidCanExecuteMethod( IMethod method )
        => method.ReturnType.SpecialType == SpecialType.Boolean
           && method.Parameters is [] or [{ RefKind: RefKind.None or RefKind.In }]
           && method.TypeParameters.Count == 0;

    private static bool IsValidCanExecuteProperty( IProperty property )
        => property.Type.SpecialType == SpecialType.Boolean
           && property.GetMethod != null;
}