// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Exception thrown upon internal assertion failures in the Flashtrace.Formatters library.
/// </summary>
/// <remarks>
/// Throw <see cref="MetalamaPatternsCachingAssertionFailedException"/> instead of using <see cref="System.Diagnostics.Debug"/>
/// assert methods so that the compiler can track execution flow.
/// </remarks>
[Serializable]
internal sealed class MetalamaPatternsCachingAssertionFailedException
    : ApplicationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetalamaPatternsCachingAssertionFailedException"/> class with the default error message.
    /// </summary>
    public MetalamaPatternsCachingAssertionFailedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetalamaPatternsCachingAssertionFailedException"/> class specifying the error message.
    /// </summary>
    public MetalamaPatternsCachingAssertionFailedException( string message ) : base( message ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetalamaPatternsCachingAssertionFailedException"/> class specifying the error message and the inner <see cref="Exception"/>.
    /// </summary>
    public MetalamaPatternsCachingAssertionFailedException( string message, Exception inner ) : base( message, inner ) { }

    private MetalamaPatternsCachingAssertionFailedException(
        SerializationInfo info,
        StreamingContext context ) : base( info, context ) { }
}