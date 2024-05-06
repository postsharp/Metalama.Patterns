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
        InspectedMemberAdder inspectedMember,
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

        var commandPropertyConflictMatch = MemberMatch<IMemberOrNamedType>.SuccessOrConflict( conflictingMember );

        MemberMatch<IMethod>? canExecuteMethodMatch = null;

        if ( considerMethod )
        {
            canExecuteMethodMatch =
                executeMethod.DeclaringType.Methods.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecuteMethodDelegate,
                    inspectedMember );
        }

        MemberMatch<IProperty>? canExecutePropertyMatch = null;

        if ( considerProperty )
        {
            canExecutePropertyMatch =
                executeMethod.DeclaringType.Properties.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecutePropertyDelegate,
                    inspectedMember );
        }

        if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Success && canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember>.Ambiguous(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                canExecuteMethodMatch.Cast<IMember>(),
                requireCanExecuteMatch );
        }
        else if ( canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                canExecutePropertyMatch.Cast<IMember>(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Ambiguous || canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Ambiguous )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember>.Ambiguous(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Invalid || canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Invalid )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember>.Invalid(),
                requireCanExecuteMatch );
        }
        else
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember>.NotFound( matchCanExecuteNamePredicate ),
                requireCanExecuteMatch );
        }
    }

    private static Func<IMethod, InspectedMemberAdder, bool> IsValidCanExecuteMethodDelegate { get; } = IsValidCanExecuteMethod;

    private static bool IsValidCanExecuteMethod( IMethod method, InspectedMemberAdder inspectedMember )
    {
        var isValid = IsValidCanExecuteMethod( method );
        inspectedMember.Add( method, isValid, CommandAttribute.CanExecuteMethodCategory );

        return isValid;
    }

    private static Func<IProperty, InspectedMemberAdder, bool> IsValidCanExecutePropertyDelegate { get; } = IsValidCanExecuteProperty;

    private static bool IsValidCanExecuteProperty( IProperty property, InspectedMemberAdder inspectedMember )
    {
        var isValid = IsValidCanExecuteProperty( property );
        inspectedMember.Add( property, isValid, CommandAttribute.CanExecutePropertyCategory );

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