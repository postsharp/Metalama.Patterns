// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] Outdated?
// TODO: [Porting] Consider using Microsoft.Toolkit.HighPerformance.Extensions.StringExtensions (now called CommunityToolkit)?

// NB: Also used by Redis backend, copied local for now.
// Ported from PostSharp.Patterns.Common/Utilities
// Was [ExplicitCrossPackageInternal]
internal ref struct StringTokenizer
{
    private readonly ReadOnlySpan<char> _s;
    private readonly char _separator;
    private int _position;

    public StringTokenizer( ReadOnlySpan<char> s, char separator = ':' )
    {
        this._s = s;
        this._position = 0;
        this._separator = separator;
    }

    public ReadOnlySpan<char> GetNext()
    {
        var oldPosition = this._position;

        for ( var i = oldPosition; i < this._s.Length; i++ )
        {
            if ( this._s[i] == this._separator )
            {
                this._position = i + 1;

                return this._s.Slice( oldPosition, i - oldPosition );
            }
        }

        return this.GetRest();
    }

    public ReadOnlySpan<char> GetRest() => this._s.Slice( this._position );
}