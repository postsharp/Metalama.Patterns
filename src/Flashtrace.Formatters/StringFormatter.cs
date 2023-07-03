// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="string"/> values.
/// </summary>
[PublicAPI]
public sealed class StringFormatter : Formatter<string>
{
    private NonQuotingStringFormatter? _nonQuotingStringFormatter;

    public StringFormatter( IFormatterRepository repository ) : base( repository ) { }

    private NonQuotingStringFormatter GetNonQuotingStringFormatter()
    {
        this._nonQuotingStringFormatter ??= new NonQuotingStringFormatter( this.Repository, this );

        return this._nonQuotingStringFormatter;
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, string? value )
    {
        if ( value == null )
        {
            stringBuilder.Append( 'n' );
            stringBuilder.Append( 'u' );
            stringBuilder.Append( 'l' );
            stringBuilder.Append( 'l' );
        }
        else
        {
            stringBuilder.Append( '"' );
            stringBuilder.Append( value );
            stringBuilder.Append( '"' );
        }
    }

    /// <inheritdoc />
    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options.RequiresUnquotedStrings )
        {
            return this.GetNonQuotingStringFormatter();
        }
        else
        {
            return this;
        }
    }

    private sealed class NonQuotingStringFormatter : Formatter<string>
    {
        private readonly StringFormatter _defaultStringFormatter;

        public NonQuotingStringFormatter( IFormatterRepository repository, StringFormatter defaultStringFormatter ) : base( repository )
        {
            this._defaultStringFormatter = defaultStringFormatter;
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

        public override IOptionAwareFormatter WithOptions( FormattingOptions options )
        {
            if ( options.RequiresUnquotedStrings )
            {
                return this;
            }
            else
            {
                return this._defaultStringFormatter;
            }
        }
    }
}