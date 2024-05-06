// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionHelper
{
    public static MemberMatch<TMember> FindValidMatchingDeclaration<TMember, TNameMatchPredicate>(
        this IEnumerable<TMember> members,
        in TNameMatchPredicate nameMatchPredicate,
        Func<TMember, InspectedMemberAdder, bool> isValid,
        InspectedMemberAdder inspectedMember )
        where TMember : class, IMemberOrNamedType
        where TNameMatchPredicate : INameMatchPredicate
    {
        // NB: The loop is not short-circuited because validity checking will build a list of
        // inspected declarations which will be used for diagnostics when applicable.

        var candidateCount = 0;
        var eligibleCount = 0;
        TMember? firstEligible = null;

        foreach ( var declaration in members )
        {
            if ( nameMatchPredicate.IsMatch( declaration.Name ) )
            {
                ++candidateCount;

                if ( isValid( declaration, inspectedMember ) )
                {
                    ++eligibleCount;
                    firstEligible ??= declaration;
                }
            }
        }

        switch ( eligibleCount )
        {
            case 0:
                if ( candidateCount > 0 )
                {
                    return MemberMatch<TMember>.Invalid();
                }

                break;

            case 1:
                return MemberMatch<TMember>.Success( firstEligible! );

            case > 1:
                return MemberMatch<TMember>.Ambiguous();
        }

        return MemberMatch<TMember>.NotFound( nameMatchPredicate );
    }

    public static (MemberMatch<TMember> Match, TMetadata? Metadata) FindValidMatchingDeclaration<TMember, TNameMatchPredicate, TMetadata>(
        this IEnumerable<TMember> declarations,
        in TNameMatchPredicate nameMatchPredicate,
        Func<TMember, InspectedMemberAdder, IsValidResult<TMetadata>> isValid,
        InspectedMemberAdder inspectedMember )
        where TMember : class, IMemberOrNamedType
        where TNameMatchPredicate : INameMatchPredicate
    {
        // NB: The loop is not short-circuited because validity checking will build a list of
        // inspected declarations which will be used for diagnostics when applicable.

        var candidateCount = 0;
        var eligibleCount = 0;
        TMember? firstEligible = null;
        TMetadata? firstEligibleMetadata = default;

        foreach ( var declaration in declarations )
        {
            if ( nameMatchPredicate.IsMatch( declaration.Name ) )
            {
                ++candidateCount;
                var result = isValid( declaration, inspectedMember );

                if ( result.IsValid )
                {
                    ++eligibleCount;

                    if ( firstEligible == null )
                    {
                        firstEligible = declaration;
                        firstEligibleMetadata = result.Metadata;
                    }
                }
            }
        }

        switch ( eligibleCount )
        {
            case 0:
                if ( candidateCount > 0 )
                {
                    return (MemberMatch<TMember>.Invalid(), default);
                }

                break;

            case 1:
                return (MemberMatch<TMember>.Success( firstEligible! ), firstEligibleMetadata);

            case > 1:
                return (MemberMatch<TMember>.Ambiguous(), default);
        }

        return (MemberMatch<TMember>.NotFound( nameMatchPredicate ), default);
    }
}