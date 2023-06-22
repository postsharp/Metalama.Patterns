// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Messages;
using Flashtrace.Messages;
using System.Runtime.CompilerServices;

namespace Flashtrace
{
    /// <summary>
    /// Creates messages based on a human-readable formatted string. These messages are suitable for structured logging are not optimal for machine analysis.
    /// For more succinct code, consider including the <c>using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder</c> statement.
    /// </summary>
    public static partial class FormattedMessageBuilder
    {
        /// <summary>
        /// Creates a formatted string with an arbitrary number of parameters.
        /// </summary>
        /// <param name="formattingString">The formatting string.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
        public static FormattedMessageArray Formatted(
            string formattingString,

            // Intentionally removing the params modifier to prevent the C# compiler to pick this overload unintentionally:
            object[] args )
            => new( formattingString, args );

        /// <summary>
        /// Creates a text message with no formatting string parameter.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
        public static FormattedMessage Formatted( string text ) => new( text );
    }
}