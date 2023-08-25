// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Creates instances of <see cref="IRoleLoggerFactory"/> for a specified role.
/// </summary>
[PublicAPI]
public interface ILoggerFactory
{
    /// <summary>
    /// Gets an instance of the <see cref="IRoleLoggerFactory"/> interface.
    /// </summary>
    /// <param name="role">The role for which the logger is requested.</param>
    /// <returns></returns>
    IRoleLoggerFactory ForRole( string role );
}