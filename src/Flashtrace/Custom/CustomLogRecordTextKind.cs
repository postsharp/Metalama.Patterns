// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Messages;
using Flashtrace.Messages;

namespace Flashtrace
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