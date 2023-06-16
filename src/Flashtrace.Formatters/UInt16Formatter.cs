// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// A formatter for <see cref="ushort"/> values.
    /// </summary>
    public sealed class UInt16Formatter : Formatter<ushort>
    {
        /// <summary>
        /// The singleton instance of <see cref="UInt16Formatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly UInt16Formatter Instance = new UInt16Formatter();

        private UInt16Formatter()
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, ushort value )
        {
             stringBuilder.Append( value );
        }


    }
}