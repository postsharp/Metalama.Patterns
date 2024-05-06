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
    bool RequireCanExecuteMatch = false ) : NamingConventionMatch( NamingConvention )
{
    private static readonly IReadOnlyList<string> _commandPropertyCategories = new[] { CommandAttribute.CommandPropertyCategory };

    private static readonly IReadOnlyList<string> _canExecuteCategories =
        new[] { CommandAttribute.CanExecuteMethodCategory, CommandAttribute.CanExecutePropertyCategory };

    public override bool Success
        => !string.IsNullOrWhiteSpace( this.CommandPropertyName )
           && this.CommandPropertyConflictMatch.Outcome == DeclarationMatchOutcome.Success
           && (this.CanExecuteMatch.Outcome == DeclarationMatchOutcome.Success
               || (this.RequireCanExecuteMatch == false && this.CanExecuteMatch.Outcome == DeclarationMatchOutcome.NotFound));

    protected override IReadOnlyList<NamingConventionMatchMember> GetMembers()
    {
        return
        [
            new NamingConventionMatchMember( this.CommandPropertyConflictMatch, true, _commandPropertyCategories ),
            new NamingConventionMatchMember( this.CanExecuteMatch, this.RequireCanExecuteMatch, _canExecuteCategories )
        ];
    }
}