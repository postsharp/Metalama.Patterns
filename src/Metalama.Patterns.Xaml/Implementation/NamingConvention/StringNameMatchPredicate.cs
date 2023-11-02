// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct StringNameMatchPredicate : INameMatchPredicate
{
    private readonly string _name;

    public StringNameMatchPredicate( string name )
    {
        this._name = name ?? throw new ArgumentNullException( nameof(name) );
    }

    public void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection )
    {
        singleValue = this._name;
        collection = null;
    }

    public bool IsMatch( string name ) => string.Equals( name, this._name, StringComparison.Ordinal );
}