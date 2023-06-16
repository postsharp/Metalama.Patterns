// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// A formatter for <see cref="string"/> values.
    /// </summary>
    public sealed class StringFormatter : Formatter<string>
    {
        /// <summary>
        /// The singleton instance of <see cref="StringFormatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly StringFormatter Instance = new StringFormatter();

        private StringFormatter()
        {
            
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, string value )
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
                return NonQuotingStringFormatter.Instance;
            }
            else
            {
                return this;
            }
        }
    }

    internal sealed class NonQuotingStringFormatter : Formatter<string>
    {
        public static readonly NonQuotingStringFormatter Instance = new NonQuotingStringFormatter();

        private NonQuotingStringFormatter()
        {

        }

        public override void Write( UnsafeStringBuilder stringBuilder, string value )
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


}