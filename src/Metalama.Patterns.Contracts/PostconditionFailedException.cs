// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// The exception that is thrown when a postcondition contract was not fulfilled by a method.
    /// </summary>
    [Serializable]
    public class PostconditionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionFailedException"/> class.
        /// </summary>
        public PostconditionFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public PostconditionFailedException( string message )
            : base( message )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionFailedException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public PostconditionFailedException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionFailedException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected PostconditionFailedException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
    }
}