﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters.UnitTests
{
    internal class NullableFormatter<T> : Formatter<T?>
        where T : struct
    {
        public NullableFormatter( IFormatterRepository repository ) : base( repository ) { }

        public override void Write( UnsafeStringBuilder stringBuilder, T? value )
        {
            stringBuilder.Append( '<' );
            stringBuilder.Append( value == null ? "null" : value.ToString() );
            stringBuilder.Append( '>' );
        }
    }
}