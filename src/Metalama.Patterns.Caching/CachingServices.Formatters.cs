// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Generic;
using PostSharp.Patterns.Formatters;
using PostSharp.Patterns.Caching.Formatters;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1034 // Nested types should not be visible


namespace PostSharp.Patterns.Caching
{
    public partial class CachingServices
    {
        /// <summary>
        /// Allows to get and register formatters used to generate caching keys.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1724")]
        public sealed class Formatters : FormatterRepository<CachingFormattingRole>
        {
            static Formatters()
            {
                Register(typeof(IEnumerable<>), typeof(CollectionFormatter<,>));
            }
            private Formatters()
            {

            }

            internal static void Initialize()
            {
                // Force the static constructor to run.
            }
        }
       
    }
}
