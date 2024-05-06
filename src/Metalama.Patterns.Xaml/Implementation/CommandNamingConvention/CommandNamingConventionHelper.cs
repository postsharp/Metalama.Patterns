// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal static class CommandNamingConventionHelper
{
    public static CommandNamingConventionMatch Match(
        INamingConvention namingConvention,
        IMethod executeMethod,
        InspectedMemberAdder inspectedMember,
        string commandPropertyName,
        INameMatchPredicate matchCanExecuteNamePredicate,
        bool considerMethod = true,
        bool considerProperty = true,
        bool requireCanExecuteMatch = false )
    {
        var declaringType = executeMethod.DeclaringType;

        var conflictingMember = (IMemberOrNamedType?) declaringType.AllMembers().FirstOrDefault( m => m.Name == commandPropertyName )
                                ?? declaringType.NestedTypes.FirstOrDefault( t => t.Name == commandPropertyName );

        var commandPropertyConflictMatch =
            conflictingMember != null
                ? MemberMatch<IMemberOrNamedType, DefaultMatchKind>.Conflict( conflictingMember )
                : MemberMatch<IMemberOrNamedType, DefaultMatchKind>.Success( DefaultMatchKind.Default );

        MemberMatch<IMethod, DefaultMatchKind>? canExecuteMethodMatch = null;

        if ( considerMethod )
        {
            canExecuteMethodMatch =
                executeMethod.DeclaringType.Methods.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecuteMethod,
                    inspectedMember,
                    CommandAttribute.CanExecuteMethodCategory );
        }

        MemberMatch<IProperty, DefaultMatchKind>? canExecutePropertyMatch = null;

        if ( considerProperty )
        {
            canExecutePropertyMatch =
                executeMethod.DeclaringType.Properties.FindValidMatchingDeclaration(
                    matchCanExecuteNamePredicate,
                    IsValidCanExecuteProperty,
                    inspectedMember,
                    CommandAttribute.CanExecutePropertyCategory );
        }

        if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Success && canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember, DefaultMatchKind>.Ambiguous(),
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
                MemberMatch<IMember, DefaultMatchKind>.Ambiguous(),
                requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatch?.Outcome == MemberMatchOutcome.Invalid || canExecutePropertyMatch?.Outcome == MemberMatchOutcome.Invalid )
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember, DefaultMatchKind>.Invalid(),
                requireCanExecuteMatch );
        }
        else
        {
            return new CommandNamingConventionMatch(
                namingConvention,
                commandPropertyName,
                commandPropertyConflictMatch,
                MemberMatch<IMember, DefaultMatchKind>.NotFound( matchCanExecuteNamePredicate.Candidates ),
                requireCanExecuteMatch );
        }
    }

    private static DefaultMatchKind? IsValidCanExecuteMethod( IMethod method )
        => method.ReturnType.SpecialType == SpecialType.Boolean
           && method.Parameters is [] or [{ RefKind: RefKind.None or RefKind.In }]
           && method.TypeParameters.Count == 0
            ? DefaultMatchKind.Default
            : null;

    private static DefaultMatchKind? IsValidCanExecuteProperty( IProperty property )
        => property.Type.SpecialType == SpecialType.Boolean
           && property.GetMethod != null
            ? DefaultMatchKind.Default
            : null;
}