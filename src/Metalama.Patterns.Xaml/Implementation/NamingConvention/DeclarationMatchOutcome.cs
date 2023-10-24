// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal enum DeclarationMatchOutcome
{
    /// <summary>
    /// A single elligble member matched.
    /// </summary>
    Success,

    /// <summary>
    /// No elligible member matched.
    /// </summary>
    NotFound,

    /// <summary>
    /// Multiple elligble members matched.
    /// </summary>
    Ambiguous,

    /// <summary>
    /// The only matches found were invalid.
    /// </summary>
    Invalid
}