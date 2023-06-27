// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Formatters;
using Flashtrace.Formatters;

namespace Metalama.Patterns.Caching
{
    public partial class CachingServices
    {
        /// <summary>
        /// Allows to get and register formatters used to generate caching keys.
        /// </summary>        
        public sealed class Formatters : FormatterRepository
        {
            public static Formatters Instance { get; } = new Formatters( CachingFormattingRole.Instance );

            private Formatters( FormattingRole role ) 
                : base( role )
            {
                this.Register( typeof( IEnumerable<> ), typeof( CollectionFormatter<> ) );
            }
        }       
    }
}