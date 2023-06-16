// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace PostSharp.Patterns.Formatters
{
    /// <summary>
    /// Efficient formatter for <see cref="short"/>.
    /// </summary>
    public sealed class Int16Formatter : Formatter<short>
    {
        /// <summary>
        /// The singleton instance of <see cref="Int16Formatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly Int16Formatter Instance = new Int16Formatter();

        private Int16Formatter()
        {
        }

        /// <inheritdoc />
        public override void Write(UnsafeStringBuilder stringBuilder, short value)
        {
            stringBuilder.Append( value );
        }
    }
}