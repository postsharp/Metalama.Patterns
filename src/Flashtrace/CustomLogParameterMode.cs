// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Determines how a parameter of a custom record should be rendered.
/// method.
/// </summary>
[PublicAPI]
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