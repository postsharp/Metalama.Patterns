// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

internal sealed class FormattableFormatter<TValue> : Formatter<TValue>
    where TValue : IFormattable
{
    public FormattableFormatter( IFormatterRepository repository ) : base( repository ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, TValue value )
    {
        if ( value == null )
        {
            stringBuilder.Append( 'n', 'u', 'l', 'l' );
        }
        else
        {
            value.Format( stringBuilder, this.Repository );
        }
    }
}