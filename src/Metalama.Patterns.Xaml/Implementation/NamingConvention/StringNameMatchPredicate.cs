// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Collections.Immutable;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal sealed class StringNameMatchPredicate : INameMatchPredicate
{
    private readonly string _name;

    public StringNameMatchPredicate( string name )
    {
        this._name = name ?? throw new ArgumentNullException( nameof(name) );
        this.Candidates = ImmutableArray.Create( name );
    }

    public bool IsMatch( string name ) => string.Equals( name, this._name, StringComparison.Ordinal );

    public ImmutableArray<string> Candidates { get; }
}