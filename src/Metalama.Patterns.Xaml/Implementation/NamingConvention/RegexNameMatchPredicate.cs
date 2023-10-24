// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct RegexNameMatchPredicate : INameMatchPredicate
{
    private readonly Regex _regex;

    public RegexNameMatchPredicate( Regex regex )
    {        
        this._regex = regex;
    }

    public void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection )
    {
        singleValue = this._regex.ToString();
        collection = null;
    }

    public bool IsMatch( string name ) => this._regex.IsMatch( name );
}