// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="char"/> values.
/// </summary>
public sealed class CharFormatter : Formatter<char>
{
    private NonQuotingCharFormatter? _nonQuotingCharFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharFormatter"/> class.
    /// </summary>
    /// <param name="repository"></param>
    public CharFormatter( IFormatterRepository repository ) : base( repository )
    {        
    }

    private NonQuotingCharFormatter NonQuotingCharFormatter
    {
        get
        {
            this._nonQuotingCharFormatter ??= new NonQuotingCharFormatter( this.Repository );
            return this._nonQuotingCharFormatter;
        }
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, char value )
    {
        stringBuilder.Append('\'', value, '\'');
    }

    /// <inheritdoc />
    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options.RequiresUnquotedStrings )
        {
            return this.NonQuotingCharFormatter;
        }
        else
        {
            return this;
        }
    }
}

internal sealed class NonQuotingCharFormatter : Formatter<char>
{
    public NonQuotingCharFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    public override void Write( UnsafeStringBuilder stringBuilder, char value )
    {
        if ( value == '\0' )
        {
            // Don't emit anything for \0. We would need another escaping formatter.
        }
        else
        {
            stringBuilder.Append( value );
        }
    }
}