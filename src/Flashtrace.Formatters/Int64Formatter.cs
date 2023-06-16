// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// Efficient formatter for <see cref="long"/>.
    /// </summary>
    public sealed class Int64Formatter : Formatter<long>
    {
        /// <summary>
        /// The singleton instance of <see cref="Int64Formatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly Int64Formatter Instance = new Int64Formatter();

        private Int64Formatter()
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, long value )
        {
            stringBuilder.Append( value );
        }
    }
}