// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal abstract record NamingConventionMatch( INamingConvention NamingConvention )
{
    private IReadOnlyList<NamingConventionMatchMember>? _members;

    public abstract bool Success { get; }

    protected abstract IReadOnlyList<NamingConventionMatchMember> GetMembers();

    public IReadOnlyList<NamingConventionMatchMember> Members => this._members ??= this.GetMembers();
}

[CompileTime]
internal sealed record NamingConventionMatchMember( IDeclarationMatch Match, bool IsRequired, IReadOnlyList<string> Categories );