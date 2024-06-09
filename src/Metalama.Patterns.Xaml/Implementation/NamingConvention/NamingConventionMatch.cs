// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Collections.Immutable;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal abstract record NamingConventionMatch( INamingConvention NamingConvention, IReadOnlyList<InspectedMember> InspectedMembers )
{
    private ImmutableArray<MemberMatchDiagnosticInfo> _members;

    public abstract NamingConventionOutcome Outcome { get; }

    protected abstract ImmutableArray<MemberMatchDiagnosticInfo> GetMemberDiagnostics();

    public ImmutableArray<MemberMatchDiagnosticInfo> Members => this._members.IsDefault ? this._members = this.GetMemberDiagnostics() : this._members;
}