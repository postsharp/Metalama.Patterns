// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INameMatchPredicate
{
    bool IsMatch( string name );

    /// <summary>
    /// Gets the candidate names used for the match.
    /// </summary>
    /// <param name="singleValue">Set to the single candidate name, if there is exactly one, otherwise <see langword="null"/>.</param>
    /// <param name="collection">Set to the collection of candidate names, if there is more than one, otherwise <see langword="null"/>.</param>
    /// <remarks>
    /// The implementation should set at most one of <paramref name="singleValue"/> or <paramref name="collection"/>, whichever is most
    /// natural for the implementation to provide. Implementations tyically have only one candidate name, but may have several.
    /// </remarks>
    void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection );
}