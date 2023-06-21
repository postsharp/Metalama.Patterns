// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace.Custom
{
    /// <summary>
    /// Options of the <see cref="ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem, in CustomLogRecordTextOptions)"/> method.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815" )]
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
        public CustomLogRecordTextOptions( int parameterCount, string name = null )
        {
            this.Name = name;
            this.ParameterCount = parameterCount;
        }
    }
}