// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Flashtrace.Formatters;

/// <summary>
/// Exception thrown upon internal assertion failures in the Flashtrace.Formatters library.
/// </summary>
/// <remarks>
/// Throw <see cref="FlashtraceFormattersAssertionFailedException"/> instead of using <see cref="System.Diagnostics.Debug"/>
/// assert methods so that the compiler can track execution flow.
/// </remarks>
[Serializable]
internal sealed class FlashtraceFormattersAssertionFailedException
    : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceFormattersAssertionFailedException"/> class with the default error message.
    /// </summary>
    public FlashtraceFormattersAssertionFailedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceFormattersAssertionFailedException"/> class specifying the error message.
    /// </summary>
    public FlashtraceFormattersAssertionFailedException( string message ) : base( message ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashtraceFormattersAssertionFailedException"/> class specifying the error message and the inner <see cref="Exception"/>.
    /// </summary>
    public FlashtraceFormattersAssertionFailedException( string message, Exception inner ) : base( message, inner ) { }

    private FlashtraceFormattersAssertionFailedException(
        SerializationInfo info,
        StreamingContext context ) : base( info, context ) { }
}