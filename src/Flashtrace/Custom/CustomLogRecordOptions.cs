// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Options of the <see cref="ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem, in CustomLogRecordTextOptions)"/> method.
    /// </summary>
    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
    public readonly struct CustomLogRecordOptions
    {
        /// <summary>
        /// Severity of the log record.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Kind of log record (typically <see cref="LogRecordKind.CustomActivityEntry"/>, <see cref="LogRecordKind.CustomActivityExit"/>
        /// or <see cref="LogRecordKind.CustomRecord"/>).
        /// </summary>
        public LogRecordKind Kind { get; }

        /// <summary>
        /// Describes how the <see cref="ICustomLogRecordBuilder"/> will be used.
        /// </summary>
        public CustomLogRecordAttributes Attributes { get; }

        /// <summary>
        /// Gets the <see cref="LogEventData"/> for the current record.
        /// </summary>
        public LogEventData Data { get;  }

        /// <summary>
        /// Initializes a new <see cref="CustomLogRecordOptions"/>.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="kind"></param>
        /// <param name="attributes"></param>
        /// <param name="data"></param>
        internal CustomLogRecordOptions(LogLevel level, LogRecordKind kind, CustomLogRecordAttributes attributes, in LogEventData data )
        {
            this.Level = level;
            this.Kind = kind;
            this.Attributes = attributes;
            this.Data = data;
        }

    }

}


