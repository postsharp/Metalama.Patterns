// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Custom.Messages;

namespace Flashtrace.Custom
{
    /// <summary>
    /// Enumerates the situations in which an <see cref=" IMessage"/> can be rendered.
    /// </summary>
    public enum CustomLogRecordItem
    {
        /// <summary>
        /// Message.
        /// </summary>
        Message,

        /// <summary>
        /// Description of an activity.
        /// </summary>
        ActivityDescription,

        /// <summary>
        /// Outcome of an activity.
        /// </summary>
        ActivityOutcome
    }
}
