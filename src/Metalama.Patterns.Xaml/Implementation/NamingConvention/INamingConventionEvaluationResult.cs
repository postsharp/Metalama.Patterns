// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INamingConventionEvaluationResult<TMatch>
    where TMatch : NamingConventionMatch
{
    [MemberNotNullWhen( true, nameof(SuccessfulMatch) )]
    bool Success { get; }

    InspectedNamingConventionMatch<TMatch>? SuccessfulMatch { get; }

    IEnumerable<InspectedNamingConventionMatch<TMatch>>? UnsuccessfulMatches { get; }
}