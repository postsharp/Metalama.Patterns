// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Custom.Messages
{
    /// <summary>
    /// Represents a message that can be used with the <see cref="LogSource"/> class. To create a message 
    /// instance, you would typically use the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Renders the current message into a given <see cref="ICustomLogRecordBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="ICustomLogRecordBuilder"/>.</param>
        /// <param name="kind">The situation in which the message is rendered (to be passed to <see cref="ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem, in CustomLogRecordTextOptions)"/>)..</param>
        /// <remarks>
        /// <para>The <see cref="IMessage"/> implementation is responsible for invoking <see cref="ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem, in CustomLogRecordTextOptions)"/>
        /// and <see cref="ICustomLogRecordBuilder.EndWriteItem(CustomLogRecordItem)"/>.
        /// </para>
        /// </remarks>
        void Write( ICustomLogRecordBuilder builder, CustomLogRecordItem kind );
    }
}