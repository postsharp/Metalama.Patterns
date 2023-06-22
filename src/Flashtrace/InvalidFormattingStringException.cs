// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Flashtrace
{
    /// <summary>
    /// Exception thrown by the <see cref="FormattingStringParser"/> and by the <c>Logger</c> class
    /// when user code provides an invalid formatting string.
    /// </summary>
    [Serializable]
    public class InvalidFormattingStringException : FormatException
    {
        /// <summary>
        /// Initializes a new <see cref="InvalidFormattingStringException"/> with the default error message.
        /// </summary>
        public InvalidFormattingStringException() : base( "Invalid formatting string." ) { }

        /// <summary>
        /// Initializes a new <see cref="InvalidFormattingStringException"/> and specifies the error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidFormattingStringException( string message ) : base( message ) { }

        /// <summary>
        /// Initializes a new <see cref="InvalidFormattingStringException"/> and specifies the error message and
        /// the inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InvalidFormattingStringException( string message, Exception inner ) : base( message, inner ) { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected InvalidFormattingStringException(
            SerializationInfo info,
            StreamingContext context ) : base( info, context ) { }
    }
}