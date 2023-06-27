// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using PostSharp.Patterns.Contracts;

namespace PostSharp.Patterns.Caching
{
    /// <summary>
    /// Exception thrown by a caching back-end during cache item retrieval (e.g. when the cached data cannot be serialized by the current object model).
    /// Throwing this exception causes removal of the invalid item.
    /// </summary>
#if SERIALIZABLE
    [Serializable]
#endif
    public class InvalidCacheItemException : CachingException
    {
        /// <summary>
        /// Initializes a new <see cref="InvalidCacheItemException"/> with the default error message.
        /// </summary>
        public InvalidCacheItemException()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="InvalidCacheItemException"/> with a given error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidCacheItemException( [Required] string message ) : base( message )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="InvalidCacheItemException"/> with a given error message and inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidCacheItemException( [Required] string message, Exception inner ) : base( message, inner )
        {
        }

#if SERIALIZABLE
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InvalidCacheItemException(
            SerializationInfo info,
            StreamingContext context ) : base( info, context )
        {
        }
#endif
    }
}
