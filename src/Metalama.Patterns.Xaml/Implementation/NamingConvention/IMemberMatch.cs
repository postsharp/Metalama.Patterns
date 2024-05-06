// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface IMemberMatch
{
    MemberMatchOutcome? Outcome { get; }

    IMemberOrNamedType? Member { get; }

    bool HasCandidateNames { get; }

    IEnumerable<string>? CandidateNames { get; }
}