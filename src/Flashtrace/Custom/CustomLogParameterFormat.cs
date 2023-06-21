// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Formatters;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Options of the <see cref="ICustomLogRecordBuilder.WriteCustomParameter{T}(int, in Formatters.CharSpan, T, in CustomLogParameterOptions)"/> method.
    /// </summary>
    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
    public readonly struct CustomLogParameterOptions
    {
        [ExplicitCrossPackageInternal]
        internal static readonly CustomLogParameterOptions FormattedStringParameter = new CustomLogParameterOptions(CustomLogParameterMode.Default);
        internal static readonly CustomLogParameterOptions SemanticParameter = new CustomLogParameterOptions(CustomLogParameterMode.NameValuePair);

        /// <summary>
        /// Determines how the parameter should be rendered.
        /// </summary>
        public CustomLogParameterMode Mode { get; }

        /// <summary>
        /// Initializes a new <see cref="CustomLogParameterOptions"/>.
        /// </summary>
        /// <param name="mode">Determines how the parameter should be rendered.</param>
        public CustomLogParameterOptions(CustomLogParameterMode mode)
        {
            this.Mode = mode;
        }

        // This property could be public and settable in the future.
        [ExplicitCrossPackageInternal]
        internal FormattingOptions FormattingOptions => this.Mode == CustomLogParameterMode.Default ? FormattingOptions.Unquoted : FormattingOptions.Default;
    }

    /// <summary>
    /// Determines how a parameter of a custom record should be rendered by the <see cref="ICustomLogRecordBuilder.WriteCustomParameter{T}(int, in Formatters.CharSpan, T, in CustomLogParameterOptions)"/>
    /// method.
    /// </summary>
    public enum CustomLogParameterMode
    {
        /// <summary>
        /// Only the parameter value is rendered.
        /// </summary>
        Default, 

        /// <summary>
        /// The parameter is rendered in <c>name = value</c> form.
        /// </summary>
        NameValuePair,

        /// <summary>
        /// The parameter is not rendered.
        /// </summary>
        Hidden
    }
}
