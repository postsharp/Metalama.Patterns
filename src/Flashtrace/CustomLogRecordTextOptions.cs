// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Options of the <see cref="ICustomLogRecordBuilder.BeginWriteItem"/> method.
/// </summary>
[PublicAPI]
public readonly struct CustomLogRecordTextOptions
{
    /// <summary>
    /// Gets the semantic name of the message, or <c>null</c> for a non-semantic message.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the number of parameters in the message.
    /// </summary>
    public int ParameterCount { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLogRecordTextOptions"/> struct.
    /// </summary>
    /// <param name="parameterCount">Number of parameters in the message.</param>
    /// <param name="name">Semantic name of the message, or <c>null</c> for a non-semantic message.</param>
    public CustomLogRecordTextOptions( int parameterCount, string? name = null )
    {
        this.Name = name;
        this.ParameterCount = parameterCount;
    }
}