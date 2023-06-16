// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// Formatter for <see cref="Nullable{T}"/>
    /// </summary>
    internal sealed class NullableFormatter<TKind, T> : Formatter<T?>
        where T : struct
        where TKind : FormattingRole, new()
    {
        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, T? value )
        {
            if ( value == null )
            {
                stringBuilder.Append('n');
                stringBuilder.Append('u');
                stringBuilder.Append('l');
                stringBuilder.Append('l');
            }
            else
            {
                FormatterRepository<TKind>.Get<T>().Write( stringBuilder, value.Value );
            }
        }
    }
}