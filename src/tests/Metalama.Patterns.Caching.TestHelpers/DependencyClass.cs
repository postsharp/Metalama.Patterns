// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text;
using Metalama.Patterns.Formatters;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public class DependencyClass : IFormattable
    {
        private readonly string key;

        public DependencyClass( string key )
        {
            this.key = key;
        }

        void IFormattable.Format( UnsafeStringBuilder stringBuilder, FormattingRole role )
        {
            stringBuilder.Append( this.key );
        }
    }
}