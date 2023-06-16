// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="string"/> values.
/// </summary>
public sealed class StringFormatter : Formatter<string>
{
    private NonQuotingStringFormatter _nonQuotingStringFormatter;

    public StringFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    private NonQuotingStringFormatter NonQuotingStringFormatter
    {
        get
        {
            this._nonQuotingStringFormatter ??= new NonQuotingStringFormatter( this.Repository );
            return this._nonQuotingStringFormatter;
        }
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, string? value )
    {
        if (value == null)
        {
            stringBuilder.Append('n');
            stringBuilder.Append('u');
            stringBuilder.Append('l');
            stringBuilder.Append('l');
        }
        else
        {
            stringBuilder.Append('"');
            stringBuilder.Append(value);
            stringBuilder.Append('"');
        }
    }

    /// <inheritdoc />
    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options.RequiresUnquotedStrings )
        {
            return this.NonQuotingStringFormatter;
        }
        else
        {
            return this;
        }
    }
}

internal sealed class NonQuotingStringFormatter : Formatter<string>
{
    public NonQuotingStringFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    public override void Write( UnsafeStringBuilder stringBuilder, string? value )
    {
        if ( value == null )
        {
            // We don't differentiate empty strings and null strings with this formatter.
        }
        else
        {
            stringBuilder.Append( value );
        }
    }
}