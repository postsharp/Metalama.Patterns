// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace Flashtrace.Formatters
{
    internal sealed class FormattableFormatter<TValue,TRole> : Formatter<TValue> 
        where TRole : FormattingRole, new()
        where TValue : IFormattable
    {
        public override void Write( UnsafeStringBuilder stringBuilder, TValue value )
        {
            if ( value == null )
            {
                stringBuilder.Append( 'n', 'u', 'l', 'l' );
            }
            else
            {
                value.Format( stringBuilder, FormatterRepository<TRole>.Role );
            }
        }
    }
}
