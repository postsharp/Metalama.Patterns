// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.Implementation
{
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
            int oldPosition = this.position;
            int p = this.s.IndexOf( this.separator, oldPosition );
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
}