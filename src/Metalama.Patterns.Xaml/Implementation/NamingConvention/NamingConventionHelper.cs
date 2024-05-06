// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal enum DefaultMatchKind
{
    Default
}

[CompileTime]
internal static class NamingConventionHelper
{
    public static MemberMatch<TMember, TKind> FindValidMatchingDeclaration<TMember, TKind>(
        this IEnumerable<TMember> members,
        INameMatchPredicate nameMatchPredicate,
        Func<TMember, TKind?> classifyMember,
        InspectedMemberAdder inspectedMember,
        string category )
        where TMember : class, IMemberOrNamedType
        where TKind : struct
    {
        // NB: The loop is not short-circuited because validity checking will build a list of
        // inspected declarations which will be used for diagnostics when applicable.

        var candidateCount = 0;
        var eligibleCount = 0;
        TMember? firstEligible = null;
        TKind? firstEligibleKind = null;

        foreach ( var declaration in members )
        {
            if ( nameMatchPredicate.IsMatch( declaration.Name ) )
            {
                ++candidateCount;

                var kind = classifyMember( declaration );

                if ( kind != null )
                {
                    ++eligibleCount;
                    firstEligible ??= declaration;
                    firstEligibleKind ??= kind;
                }

                inspectedMember.Add( declaration, kind != null, category );
            }
        }

        switch ( eligibleCount )
        {
            case 0:
                if ( candidateCount > 0 )
                {
                    return MemberMatch<TMember, TKind>.Invalid();
                }

                break;

            case 1:
                return MemberMatch<TMember, TKind>.Success( firstEligible!, firstEligibleKind!.Value );

            case > 1:
                return MemberMatch<TMember, TKind>.Ambiguous();
        }

        return MemberMatch<TMember, TKind>.NotFound( nameMatchPredicate.Candidates );
    }
}