// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Globalization;

namespace Flashtrace.Formatters;

internal sealed class DynamicFormatter<TValue> : Formatter<TValue> 
{
    private readonly FormattingOptions _options;

    public DynamicFormatter( IFormatterRepository repository ) : this( repository, FormattingOptions.Default ) 
    {
    }

    private DynamicFormatter( IFormatterRepository repository, FormattingOptions options )
        : base( repository )
    {
        this._options = options ?? FormattingOptions.Default;
    }

    private DynamicFormatter<TValue>? otherFormatter;

    public override FormatterAttributes Attributes => FormatterAttributes.Dynamic;

    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options == this._options )
        {
            return this;
        }
        else
        {
            if ( this.otherFormatter == null )
            {
                // There are just two options currently.
                this.otherFormatter = new DynamicFormatter<TValue>( this.Repository, this._options );
            }

            return this.otherFormatter;
        }
    }

    public override void Write( UnsafeStringBuilder stringBuilder, TValue value )
    {
        if ( value == null )
        {
            stringBuilder.Append( 'n', 'u', 'l', 'l' );
        }
        else
        {
            IFormatter formatter = this.Repository.Get(value.GetType() ).WithOptions(this._options);

            if ( formatter == null )
                throw new AssertionFailedException( string.Format( CultureInfo.InvariantCulture, "Cannot get a formatter for type {0}.", value.GetType() ) );

            if ( (formatter.Attributes & FormatterAttributes.Dynamic) != 0 )
                throw new AssertionFailedException( string.Format( CultureInfo.InvariantCulture, "Infinite loop in resolving formatters for type {0}.", value.GetType() ) );

            formatter.Write( stringBuilder, value );
        }
    }
}