// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Contracts;
using System;
#if SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace PostSharp.Patterns.Caching
{

    /// <summary>
    /// Exception thrown by <c>PostSharp.Patterns.Caching</c>.
    /// </summary>
#if SERIALIZABLE
    [Serializable]
#endif
    public class CachingException : Exception
    {


        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with the default error message.
        /// </summary>
        public CachingException()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with a given error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public CachingException( [Required] string message ) : base( message )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="CachingException"/> with a given error message and inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        public CachingException( [Required] string message, Exception inner ) : base( message, inner )
        {
        }

        #if SERIALIZABLE
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CachingException(
            SerializationInfo info,
            StreamingContext context ) : base( info, context )
        {
        }
        #endif
    }

    
}