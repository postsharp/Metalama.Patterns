// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Backends.Azure;

/// <summary>
/// Exception thrown upon internal assertion failures in the Flashtrace.Formatters library.
/// </summary>
/// <remarks>
/// Throw <see cref="AzureCachingBackendAssertionFailedException"/> instead of using <see cref="System.Diagnostics.Debug"/>
/// assert methods so that the compiler can track execution flow.
/// </remarks>
[Serializable]
internal sealed class AzureCachingBackendAssertionFailedException
    : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCachingBackendAssertionFailedException"/> class with the default error message.
    /// </summary>
    public AzureCachingBackendAssertionFailedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCachingBackendAssertionFailedException"/> class specifying the error message.
    /// </summary>
    public AzureCachingBackendAssertionFailedException( string message ) : base( message ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCachingBackendAssertionFailedException"/> class specifying the error message and the inner <see cref="Exception"/>.
    /// </summary>
    public AzureCachingBackendAssertionFailedException( string message, Exception inner ) : base( message, inner ) { }

    private AzureCachingBackendAssertionFailedException(
        SerializationInfo info,
        StreamingContext context ) : base( info, context ) { }
}