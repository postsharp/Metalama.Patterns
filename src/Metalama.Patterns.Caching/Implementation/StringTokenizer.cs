// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] Outdated?
// TODO: [Porting] Consider using microsoft.toolkit.highperformance.extensions.stringextensions (now called CommunityToolkit)?
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.toolkit.highperformance.extensions.stringextensions.tokenize?view=win-comm-toolkit-dotnet-6.1#microsoft-toolkit-highperformance-extensions-stringextensions-tokenize(system-string-system-char)
// NB: Also used by Redis backend, copied local for now.
// Ported from PostSharp.Patterns.Common/Utilities
[ExplicitCrossPackageInternal]
internal struct StringTokenizer
{
    private readonly string _s;
    private int _position;
    private readonly char _separator;

    public StringTokenizer( string s, char separator = ':' )
    {
        this._s = s;
        this._position = 0;
        this._separator = separator;
    }

    public string GetNext()
    {
        var oldPosition = this._position;
        var p = this._s.IndexOf( this._separator, oldPosition );

        if ( p < 0 )
        {
            return this.GetRest();
        }
        else
        {
            this._position = p + 1;

            return this._s.Substring( oldPosition, p - oldPosition );
        }
    }

    public string GetRest() => this._s.Substring( this._position );
}