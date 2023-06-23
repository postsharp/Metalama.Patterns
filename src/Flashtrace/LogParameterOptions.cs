// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Options of the <see cref="ILogRecordBuilder.WriteParameter{T}"/> method.
/// </summary>
[PublicAPI]
public readonly struct LogParameterOptions
{
    [ExplicitCrossPackageInternal]
    internal static readonly LogParameterOptions FormattedStringParameter = new( LogParameterMode.Default );

    internal static readonly LogParameterOptions SemanticParameter = new( LogParameterMode.NameValuePair );

    /// <summary>
    /// Gets rendering mode of the parameter.
    /// </summary>
    public LogParameterMode Mode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogParameterOptions"/> struct.
    /// </summary>
    /// <param name="mode">Determines how the parameter should be rendered.</param>
    public LogParameterOptions( LogParameterMode mode )
    {
        this.Mode = mode;
    }

    // This property could be public and settable in the future.
    [ExplicitCrossPackageInternal]
    internal FormattingOptions FormattingOptions => this.Mode == LogParameterMode.Default ? FormattingOptions.Unquoted : FormattingOptions.Default;
}