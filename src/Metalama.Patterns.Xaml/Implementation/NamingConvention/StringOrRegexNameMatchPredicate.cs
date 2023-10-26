// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct StringOrRegexNameMatchPredicate : INameMatchPredicate
{
    private readonly string? _name;
    private readonly Regex? _matchName;

    public StringOrRegexNameMatchPredicate( Regex matchName )
    {
        this._matchName = matchName ?? throw new ArgumentNullException( nameof( matchName ) );
    }

    public StringOrRegexNameMatchPredicate( string name )
    {
        this._name = name ?? throw new ArgumentNullException( nameof( name ) );
    }

    public StringOrRegexNameMatchPredicate( string? name, Regex? matchName )
    {
        if ( name == null == ( matchName == null ))
        {
            throw new ArgumentException( "Exactly one of name and matchName must be specified." );
        }

        this._name = name;
        this._matchName = matchName;
    }

    public void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection )
    {
        singleValue = this._name ?? this._matchName!.ToString();
        collection = null;
    }

    public bool IsMatch( string name ) => this._name != null ? string.Equals( name, this._name, StringComparison.Ordinal ) : this._matchName!.IsMatch( name );
}