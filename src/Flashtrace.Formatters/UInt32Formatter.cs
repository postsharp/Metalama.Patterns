// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PostSharp.Patterns.Formatters
{
    /// <summary>
    /// A formatter for <see cref="uint"/> values.
    /// </summary>
    public sealed class UInt32Formatter : Formatter<uint>
    {
        /// <summary>
        /// The singleton instance of <see cref="UInt32Formatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly UInt32Formatter Instance = new UInt32Formatter();

        private UInt32Formatter()
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, uint value )
        {
            stringBuilder.Append( value );
        }


    }
}