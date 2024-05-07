// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Collections.Immutable;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed record CommandNamingConventionMatch(
    INamingConvention NamingConvention,
    string? CommandPropertyName,
    MemberMatch<IMemberOrNamedType, DefaultMatchKind> CommandPropertyMatch,
    MemberMatch<IMember, DefaultMatchKind> CanExecuteMatch,
    bool RequireCanExecuteMatch = false ) : NamingConventionMatch( NamingConvention )
{
    private static readonly ImmutableArray<string> _commandPropertyCategories = ImmutableArray.Create( CommandAttribute.CommandPropertyCategory );

    private static readonly ImmutableArray<string> _canExecuteCategories =
        ImmutableArray.Create( CommandAttribute.CanExecuteMethodCategory, CommandAttribute.CanExecutePropertyCategory );

    public override bool Success
        => !string.IsNullOrWhiteSpace( this.CommandPropertyName )
           && this.CommandPropertyMatch.Outcome == MemberMatchOutcome.Success
           && (this.CanExecuteMatch.Outcome == MemberMatchOutcome.Success
               || (this.RequireCanExecuteMatch == false && this.CanExecuteMatch.Outcome == MemberMatchOutcome.NotFound));

    protected override ImmutableArray<MemberMatchDiagnosticInfo> GetMembers()
        => ImmutableArray.Create(
            new MemberMatchDiagnosticInfo( this.CommandPropertyMatch, true, _commandPropertyCategories ),
            new MemberMatchDiagnosticInfo( this.CanExecuteMatch, this.RequireCanExecuteMatch, _canExecuteCategories ) );
}