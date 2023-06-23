// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Creates instances of <see cref="ILoggerFactory"/>. An instance of this interface must be registered into the <see cref="ServiceLocator"/>.
/// </summary>
[PublicAPI]
public interface ILoggerFactoryProvider
{
    /// <summary>
    /// Gets an instance of the <see cref="ILoggerFactory"/> interface.
    /// </summary>
    /// <param name="role">The role for which the logger is requested.</param>
    /// <returns></returns>
    ILoggerFactory GetLoggerFactory( string role );
}