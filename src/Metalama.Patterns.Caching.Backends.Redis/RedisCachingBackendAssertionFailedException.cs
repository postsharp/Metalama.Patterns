// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Backends.Redis;

/// <summary>
/// Exception thrown upon internal assertion failures in the Metalama.Patterns.Caching.Backends.Redis library.
/// </summary>
/// <remarks>
/// Throw <see cref="RedisCachingBackendAssertionFailedException"/> instead of using <see cref="System.Diagnostics.Debug"/>
/// assert methods so that the compiler can track execution flow.
/// </remarks>
[Serializable]
internal sealed class RedisCachingBackendAssertionFailedException
    : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCachingBackendAssertionFailedException"/> class with the default error message.
    /// </summary>
    public RedisCachingBackendAssertionFailedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCachingBackendAssertionFailedException"/> class specifying the error message.
    /// </summary>
    public RedisCachingBackendAssertionFailedException( string message ) : base( message ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCachingBackendAssertionFailedException"/> class specifying the error message and the inner <see cref="Exception"/>.
    /// </summary>
    public RedisCachingBackendAssertionFailedException( string message, Exception inner ) : base( message, inner ) { }

    private RedisCachingBackendAssertionFailedException(
        SerializationInfo info,
        StreamingContext context ) : base( info, context ) { }
}