// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace
{
    /// <summary>
    /// Options of the <see cref="ICustomLogRecordBuilder.WriteCustomParameter{T}(int, in Formatters.CharSpan, T, in CustomLogParameterOptions)"/> method.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
    public readonly struct CustomLogParameterOptions
    {
        [ExplicitCrossPackageInternal]
        internal static readonly CustomLogParameterOptions FormattedStringParameter = new( CustomLogParameterMode.Default );

        internal static readonly CustomLogParameterOptions SemanticParameter = new( CustomLogParameterMode.NameValuePair );

        /// <summary>
        /// Determines how the parameter should be rendered.
        /// </summary>
        public CustomLogParameterMode Mode { get; }

        /// <summary>
        /// Initializes a new <see cref="CustomLogParameterOptions"/>.
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