// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;
using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching
{
    /// <summary>
    /// Exception thrown by <c>PostSharp.Patterns.Caching</c>.
    /// </summary>
    [Serializable]
    public class CachingException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with the default error message.
        /// </summary>
        public CachingException() { }

        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with a given error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CachingException( [Required] string message ) : base( message ) { }

        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with a given error message and inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        public CachingException( [Required] string message, Exception inner ) : base( message, inner ) { }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CachingException(
            SerializationInfo info,
            StreamingContext context ) : base( info, context ) { }
    }
}