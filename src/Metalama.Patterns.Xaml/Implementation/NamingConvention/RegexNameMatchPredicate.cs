// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct RegexNameMatchPredicate : INameMatchPredicate
{
    private readonly Regex _matchName;

    public RegexNameMatchPredicate( Regex matchName )
    {        
        this._matchName = matchName ?? throw new ArgumentNullException( nameof( matchName ) );
    }

    public void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection )
    {
        singleValue = this._matchName.ToString();
        collection = null;
    }

    public bool IsMatch( string name ) => this._matchName.IsMatch( name );
}