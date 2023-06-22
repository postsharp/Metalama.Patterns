// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Attributes of the <see cref="CustomLogParameterOptions"/> class. Describes how the <see cref="ICustomLogRecordBuilder"/> will be used.
/// </summary>
[PublicAPI]
[Flags]
public enum CustomLogRecordAttributes
{
    /// <summary>
    /// Legacy value set by <see cref="ILogger"/> implementations. No information is provided by the caller.
    /// </summary>
    None = 0,

    /// <summary>
    /// The <see cref="ICustomLogRecordBuilder"/> will be used to write the context description.
    /// </summary>
    WriteActivityDescription = 1,

    /// <summary>
    /// The <see cref="ICustomLogRecordBuilder"/> will be used to write the context outcome.
    /// </summary>
    WriteActivityOutcome = 2,

    /// <summary>
    /// The <see cref="ICustomLogRecordBuilder"/> will be used to write the context description and outcome.
    /// </summary>
    WriteActivityDescriptionAndOutcome = WriteActivityDescription | WriteActivityOutcome,

    /// <summary>
    /// The <see cref="ICustomLogRecordBuilder"/> will be used to write a standalone message.
    /// </summary>
    WriteMessage = 4
}