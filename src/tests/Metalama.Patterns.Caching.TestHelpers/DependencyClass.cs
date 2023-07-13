// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using IFormattable = Flashtrace.Formatters.IFormattable;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public class DependencyClass : IFormattable
    {
        private readonly string _key;

        public DependencyClass( string key )
        {
            this._key = key;
        }

        void IFormattable.Format( UnsafeStringBuilder stringBuilder, IFormatterRepository formatterRepository )
        {
            stringBuilder.Append( this._key );
        }
    }
}