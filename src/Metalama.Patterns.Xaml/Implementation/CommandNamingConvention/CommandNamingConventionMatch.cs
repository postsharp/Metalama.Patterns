// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed record CommandNamingConventionMatch(
    INamingConvention NamingConvention,
    string? CommandPropertyName,
    DeclarationMatch<IMemberOrNamedType> CommandPropertyConflictMatch,
    DeclarationMatch<IMember> CanExecuteMatch,
    bool RequireCanExecuteMatch = false ) : INamingConventionMatch
{
    public bool Success
        => !string.IsNullOrWhiteSpace( this.CommandPropertyName )
           && this.CommandPropertyConflictMatch.Outcome == DeclarationMatchOutcome.Success
           && (this.CanExecuteMatch.Outcome == DeclarationMatchOutcome.Success
               || (this.RequireCanExecuteMatch == false && this.CanExecuteMatch.Outcome == DeclarationMatchOutcome.NotFound));

    private static readonly IReadOnlyList<string> _commandPropertyCategories = new[] { CommandAttribute.CommandPropertyCategory };

    private static readonly IReadOnlyList<string> _canExecuteCategories =
        new[] { CommandAttribute.CanExecuteMethodCategory, CommandAttribute.CanExecutePropertyCategory };

    public void VisitDeclarationMatches<TVisitor>( in TVisitor visitor )
        where TVisitor : IDeclarationMatchVisitor
    {
        visitor.Visit( this.CommandPropertyConflictMatch, true, _commandPropertyCategories );
        visitor.Visit( this.CanExecuteMatch, this.RequireCanExecuteMatch, _canExecuteCategories );
    }
}