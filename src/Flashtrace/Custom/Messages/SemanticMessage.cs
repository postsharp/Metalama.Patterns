// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace.Custom.Messages
{
    /// <summary>
    /// Encapsulates a semantic message without parameter. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
    /// </summary>
    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
    public readonly struct SemanticMessage : IMessage
    {
        private readonly string messageName;

#if AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        internal SemanticMessage( string messageName )
        {
            this.messageName = messageName;
        }

        
        void IMessage.Write( ICustomLogRecordBuilder builder, CustomLogRecordItem item)
        {
            builder.BeginWriteItem(item, new CustomLogRecordTextOptions(0, this.messageName));
            builder.EndWriteItem(item);
        }


        /// <inheritdoc/>
        public override string ToString() => DebugMessageFormatter.Format( this );

    }

}
