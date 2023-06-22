// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Options of the <see cref="ICustomLogRecordBuilder.WriteCustomParameter{T}"/> method.
/// </summary>
[PublicAPI]
public readonly struct CustomLogParameterOptions
{
    [ExplicitCrossPackageInternal]
    internal static readonly CustomLogParameterOptions FormattedStringParameter = new( CustomLogParameterMode.Default );

    internal static readonly CustomLogParameterOptions SemanticParameter = new( CustomLogParameterMode.NameValuePair );

    /// <summary>
    /// Gets rendering mode of the parameter.
    /// </summary>
    public CustomLogParameterMode Mode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomLogParameterOptions"/> struct.
    /// </summary>
    /// <param name="mode">Determines how the parameter should be rendered.</param>
    public CustomLogParameterOptions( CustomLogParameterMode mode )
    {
        this.Mode = mode;
    }

    // This property could be public and settable in the future.
    [ExplicitCrossPackageInternal]
    internal FormattingOptions FormattingOptions => this.Mode == CustomLogParameterMode.Default ? FormattingOptions.Unquoted : FormattingOptions.Default;
}