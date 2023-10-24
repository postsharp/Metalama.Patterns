// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INameMatchPredicate
{
    bool IsMatch( string name );

    void GetCandidateNames( out string? singleValue, out IEnumerable<string>? collection );
}