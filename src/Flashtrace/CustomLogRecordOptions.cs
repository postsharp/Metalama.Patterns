// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Options of the <see cref="IContextLocalLogger.GetRecordBuilder"/> method.
/// 
/// </summary>
[PublicAPI]
public readonly struct CustomLogRecordOptions
{
    /// <summary>
    /// Gets the severity of the log record.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets the kind of log the record (typically <see cref="LogRecordKind.CustomActivityEntry"/>, <see cref="LogRecordKind.CustomActivityExit"/>
    /// or <see cref="LogRecordKind.CustomRecord"/>).
    /// </summary>
    public LogRecordKind Kind { get; }

    /// <summary>
    /// Gets flags indicating how the log record will be used.
    /// </summary>
    public CustomLogRecordAttributes Attributes { get; }

    /// <summary>
    /// Gets the <see cref="LogEventData"/> for the current record.
    /// </summary>
    public LogEventData Data { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLogRecordOptions"/> struct.
    /// Initializes a new <see cref="CustomLogRecordOptions"/>.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="kind"></param>
    /// <param name="attributes"></param>
    /// <param name="data"></param>
    internal CustomLogRecordOptions( LogLevel level, LogRecordKind kind, CustomLogRecordAttributes attributes, in LogEventData data )
    {
        this.Level = level;
        this.Kind = kind;
        this.Attributes = attributes;
        this.Data = data;
    }
}