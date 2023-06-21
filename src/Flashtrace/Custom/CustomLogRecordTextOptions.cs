// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Options of the <see cref="ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem, in CustomLogRecordTextOptions)"/> method.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815")]
    public readonly struct CustomLogRecordTextOptions
    {
        /// <summary>
        /// Gets the semantic name of the message (<c>null</c> in case of a non-semantic message).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the number of parameters in the message.
        /// </summary>
        public int ParameterCount { get; }

        /// <summary>
        /// Initializes a new <see cref="CustomLogRecordTextOptions"/>.
        /// </summary>
        /// <param name="parameterCount">Number of parameters in the message.</param>
        /// <param name="name">Semantic name of the message (<c>null</c> in case of a non-semantic message).</param>
        public CustomLogRecordTextOptions(int parameterCount, string name = null ) 
        {
            this.Name = name;
            this.ParameterCount = parameterCount;
        }
    }
}
