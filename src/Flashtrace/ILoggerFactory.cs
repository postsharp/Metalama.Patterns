// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Creates instances of the <see cref="ILogger"/> interface. An instance of this interface must be registered into the <see cref="LoggingServiceLocator"/>.
/// </summary>
[PublicAPI]
public interface ILoggerFactory
{
    /// <summary>
    /// Gets an instance of the <see cref="ILogger"/> for a specific <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type of the source code that will emit the records.</param>
    /// <returns>An instance of the <see cref="ILogger"/> interface for <paramref name="type"/>.</returns>
    ILogger GetLogger( Type type );

    /// <summary>
    /// Gets an instance of the <see cref="ILogger"/> interface for a specific <paramref name="sourceName"/>. The name will
    /// usually, but not always, be a type name.
    /// </summary>
    /// <param name="sourceName">Name identifying the returned logger. The backend creates a logger based on this name.</param>
    ILogger GetLogger( string sourceName );
}