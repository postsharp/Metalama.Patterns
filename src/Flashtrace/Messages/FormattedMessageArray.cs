// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Flashtrace.Messages
{
    /// <summary>
    /// Encapsulates a formatted message with an arbitrary number of parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
    public readonly struct FormattedMessageArray : IMessage
    {
        private readonly string formattingString;
        private readonly object[] args;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
        internal FormattedMessageArray( string formattingString, object[] args )
        {
            this.formattingString = formattingString;
            this.args = args;
        }

        void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
        {
            recordBuilder.BeginWriteItem( item, new CustomLogRecordTextOptions( this.args.Length ) );

            var parser = new FormattingStringParser( this.formattingString );

            for ( var i = 0; i < this.args.Length; i++ )
            {
                recordBuilder.WriteCustomString( parser.GetNextSubstring() );
                var parameter = parser.GetNextParameter();

                if ( parameter.Array == null )
                {
                    throw new InvalidFormattingStringException(
                        string.Format( CultureInfo.InvariantCulture, "The formatting string must have exactly {0} parameters.", this.args.Length ) );
                }

                recordBuilder.WriteCustomParameter( i, parameter, this.args[i], CustomLogParameterOptions.FormattedStringParameter );
            }

            recordBuilder.WriteCustomString( parser.GetNextSubstring() );

            if ( parser.GetNextParameter().Array != null )
            {
                throw new InvalidFormattingStringException(
                    string.Format( CultureInfo.InvariantCulture, "The formatting string must have exactly {0} parameters.", this.args.Length ) );
            }

            recordBuilder.EndWriteItem( item );
        }

        /// <inheritdoc/>
        public override string ToString() => DebugMessageFormatter.Format( this );
    }
}