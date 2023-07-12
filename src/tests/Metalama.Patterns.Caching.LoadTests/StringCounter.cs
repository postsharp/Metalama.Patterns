// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Generic;
using System.Numerics;

namespace Metalama.Patterns.Caching.LoadTests
{
    public class StringCounter
    {
        private readonly Dictionary<string, BigInteger> counters = new();

        private readonly Dictionary<string, List<string>> details = new();

        public IReadOnlyDictionary<string, BigInteger> Counters => this.counters;

        public IReadOnlyDictionary<string, List<string>> Details => this.details;

        public void Increment( string name, string detail = null )
        {
            BigInteger count;

            if ( !this.counters.TryGetValue( name, out count ) )
            {
                this.counters.Add( name, 1 );
            }
            else
            {
                this.counters[name] = count + 1;
            }

            if ( detail == null )
            {
                return;
            }

            List<string> thisDetails;

            if ( !this.details.TryGetValue( name, out thisDetails ) )
            {
                thisDetails = new List<string>();
                this.details.Add( name, thisDetails );
            }

            thisDetails.Add( detail );
        }
    }
}