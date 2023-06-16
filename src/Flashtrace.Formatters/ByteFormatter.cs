// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// A formatter for <see cref="byte"/> values.
    /// </summary>
    public sealed class ByteFormatter : Formatter<byte>
    {
        /// <summary>
        /// The singleton instance of <see cref="ByteFormatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly ByteFormatter Instance = new ByteFormatter();

        private ByteFormatter()
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, byte value )
        {
            stringBuilder.Append( value );
        }


    }
}