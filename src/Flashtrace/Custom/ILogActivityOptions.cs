// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Flashtrace.Custom
{
    /// <summary>
    /// Exposes the default verbosity of the <see cref="Logger"/> and <see cref="LogActivity"/> classes when creating and closing activities.
    /// </summary>
    public interface ILogActivityOptions
    {
        /// <summary>
        /// Gets or sets the default severity for custom messages and for entry and success messages of activities.
        /// </summary>
        LogLevel ActivityLevel { get; }

        /// <summary>
        /// Gets or sets the <see cref="LogLevel"/> for failures of custom activities (defined by the <see cref="LogActivity.SetFailure(string)"/> method).
        /// </summary>
        LogLevel FailureLevel { get; }

        /// <summary>
        /// Gets or sets the default severity for failure custom messages.
        /// </summary>
        LogLevel ExceptionLevel { get; }
    }
}


