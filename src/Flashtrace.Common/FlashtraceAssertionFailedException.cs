// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Flashtrace;

/// <summary>
/// Exception thrown upon internal assertion failures in PostSharp Pattern Libraries.
/// </summary>
[Serializable]
public sealed class FlashtraceAssertionFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceAssertionFailedException"/> class with the default error message.
    /// </summary>
    public FlashtraceAssertionFailedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceAssertionFailedException"/> class specifying the error message.
    /// </summary>
    public FlashtraceAssertionFailedException( string message ) : base( message ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceAssertionFailedException"/> class specifying the error message and the inner <see cref="Exception"/>.
    /// </summary>
    public FlashtraceAssertionFailedException( string message, Exception inner ) : base( message, inner ) { }

    private FlashtraceAssertionFailedException(
        SerializationInfo info,
        StreamingContext context ) : base( info, context ) { }
}