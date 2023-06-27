// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Backends.Redis;

// TODO: [Porting] Outdated?
// TODO: [Porting] Consider using microsoft.toolkit.highperformance.extensions.stringextensions (now called CommunityToolkit)?
// https://learn.microsoft.com/en-us/dotnet/api/microsoft.toolkit.highperformance.extensions.stringextensions.tokenize?view=win-comm-toolkit-dotnet-6.1#microsoft-toolkit-highperformance-extensions-stringextensions-tokenize(system-string-system-char)
// NB: Also used by Metalama.Patterns.Caching, copied local for now.
// Ported from PostSharp.Patterns.Common/Utilities
[ExplicitCrossPackageInternal]
internal struct StringTokenizer
{
    private readonly string s;
    private int position;
    private readonly char separator;

    public StringTokenizer( string s, char separator = ':' )
    {
        this.s = s;
        this.position = 0;
        this.separator = separator;
    }

    public string GetNext()
    {
        var oldPosition = this.position;
        var p = this.s.IndexOf( this.separator, oldPosition );

        if ( p < 0 )
        {
            return this.GetRest();
        }
        else
        {
            this.position = p + 1;

            return this.s.Substring( oldPosition, p - oldPosition );
        }
    }

    public string GetRest()
    {
        return this.s.Substring( this.position );
    }
}