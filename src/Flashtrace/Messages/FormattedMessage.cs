// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Messages;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flashtrace.Messages
{
    /// <summary>
    /// Encapsulates a formatted message without parameter. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
    public readonly struct FormattedMessage : IMessage
    {
        private readonly string text;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
        internal FormattedMessage( string text )
        {
            this.text = text;
        }

        void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
        {
            recordBuilder.BeginWriteItem( item, new CustomLogRecordTextOptions( 0 ) );

            if ( !string.IsNullOrEmpty( this.text ) )
            {
                recordBuilder.WriteCustomString( this.text );
            }

            recordBuilder.EndWriteItem( item );
        }

        /// <inheritdoc/>
        public override string ToString() => this.text ?? "null";
    }
}