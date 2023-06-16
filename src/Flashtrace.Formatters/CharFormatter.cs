// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// A formatter for <see cref="char"/> values.
    /// </summary>
    public sealed class CharFormatter : Formatter<char>
    {
        /// <summary>
        /// The singleton instance of <see cref="CharFormatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly CharFormatter Instance = new CharFormatter();

        private CharFormatter()
        {
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
                return NonQuotingCharFormatter.Instance;
            }
            else
            {
                return this;
            }
        }
    }

    internal sealed class NonQuotingCharFormatter : Formatter<char>
    {
        public static readonly NonQuotingCharFormatter Instance = new NonQuotingCharFormatter();

        private NonQuotingCharFormatter()
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
}