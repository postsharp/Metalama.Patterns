// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INamingConventionEvaluationResult<TMatch>
    where TMatch : INamingConventionMatch
{
    TMatch? SuccessfulMatch { get; }

    IEnumerable<UnsuccesfulNamingConventionMatch<TMatch>>? UnsuccessfulMatches { get; }
}