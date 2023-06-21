// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using PostSharp.Patterns.Diagnostics.Custom;
using System.Runtime.CompilerServices;

namespace PostSharp.Patterns.Diagnostics.Custom.Messages
{
    /// <summary>
    /// Encapsulates a formatted message without parameter. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
    /// </summary>
    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
    public readonly struct FormattedMessage : IMessage
    {
        private readonly string text;

#if AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        internal FormattedMessage( string text )
        {
            this.text = text;
        }

        
        void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
        {
            recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(0));
            if ( !string.IsNullOrEmpty( this.text ) )
            {
                recordBuilder.WriteCustomString( this.text );
            }
            recordBuilder.EndWriteItem(item);
        }

        /// <inheritdoc/>
        public override string ToString() => this.text ?? "null";
        
    }
}