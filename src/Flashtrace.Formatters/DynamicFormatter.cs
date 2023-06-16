// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Flashtrace.Formatters;

internal sealed class DynamicFormatter<TValue, TRole> : Formatter<TValue> 
    where TRole : FormattingRole, new()
{
    private readonly FormattingOptions options;

    public DynamicFormatter() : this( FormattingOptions.Default ) 
    {
    }

    private DynamicFormatter( FormattingOptions options )
    {
        this.options = options ?? FormattingOptions.Default;
    }

    private DynamicFormatter<TValue, TRole> otherFormatter;

    public override FormatterAttributes Attributes => FormatterAttributes.Dynamic;

    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options == this.options )
        {
            return this;
        }
        else
        {
            if ( this.otherFormatter == null )
            {
                // There are just two options currently.
                this.otherFormatter = new DynamicFormatter<TValue, TRole>( options );
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
            IFormatter formatter = FormatterRepository<TRole>.Get(value.GetType() ).WithOptions(this.options);

            if ( formatter == null )
                throw new AssertionFailedException( string.Format( CultureInfo.InvariantCulture, "Cannot get a formatter for type {0}.", value.GetType() ) );

            if ( (formatter.Attributes & FormatterAttributes.Dynamic) != 0 )
                throw new AssertionFailedException( string.Format( CultureInfo.InvariantCulture, "Infinite loop in resolving formatters for type {0}.", value.GetType() ) );

            formatter.Write( stringBuilder, value );
        }
    }
}
