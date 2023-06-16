// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// Efficient formatter for <see cref="int"/>.
    /// </summary>
    public sealed class Int32Formatter : Formatter<int>
    {
        /// <summary>
        /// The singleton instance of <see cref="Int32Formatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly Int32Formatter Instance = new Int32Formatter();

        private Int32Formatter()
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, int value )
        {
            stringBuilder.Append( value );
        }
    }
}