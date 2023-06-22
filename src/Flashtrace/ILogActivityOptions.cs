// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

// TODO: Is ILogActivityOptions actually used?

/// <summary>
/// Exposes the default verbosity of the <see cref="LogSource"/> and <see cref="LogActivity{TActivityDescription}"/> classes when creating and closing activities.
/// </summary>
[PublicAPI]
public interface ILogActivityOptions
{
    // TODO: Review where these are used and update docs:

    /// <summary>
    /// Gets the default severity for custom messages and for entry and success messages of activities.
    /// </summary>
    LogLevel ActivityLevel { get; }

    /// <summary>
    /// Gets the <see cref="LogLevel"/> for failures of custom activities (defined by the XXX method).
    /// </summary>
    LogLevel FailureLevel { get; }

    /// <summary>
    /// Gets the default severity for failure custom messages.
    /// </summary>
    LogLevel ExceptionLevel { get; }
}