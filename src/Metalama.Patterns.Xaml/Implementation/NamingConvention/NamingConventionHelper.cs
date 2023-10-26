// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionHelper
{
    public static DeclarationMatch<TDeclaration> FindValidMatchingDeclaration<TDeclaration, TNameMatchPredicate>(
        this IEnumerable<TDeclaration> declarations,
        in TNameMatchPredicate nameMatchPredicate,
        Func<TDeclaration, InspectedDeclarationsAdder, bool> isValid,
        InspectedDeclarationsAdder inspectedDeclarations )
        where TDeclaration : class, INamedDeclaration
        where TNameMatchPredicate : INameMatchPredicate
    {
        // NB: The loop is not short-circuited becuase validity checking will build a list of
        // inspected declarations which will be used for diagnostics when applicable.

        var candidateCount = 0;
        var eligibleCount = 0;
        TDeclaration? firstEligible = null;

        foreach ( var declaration in declarations )
        {
            if ( nameMatchPredicate.IsMatch( declaration.Name ) )
            {
                ++candidateCount;

                if ( isValid( declaration, inspectedDeclarations ) )
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
                    return DeclarationMatch<TDeclaration>.Invalid();
                }

                break;

            case 1:
                return DeclarationMatch<TDeclaration>.Success( firstEligible );

            case > 1:
                return DeclarationMatch<TDeclaration>.Ambiguous();
        }

        return DeclarationMatch<TDeclaration>.NotFound( nameMatchPredicate );
    }

    public static (DeclarationMatch<TDeclaration> Match, TMetadata? Metadata) FindValidMatchingDeclaration<TDeclaration, TNameMatchPredicate, TMetadata>(
        this IEnumerable<TDeclaration> declarations,
        in TNameMatchPredicate nameMatchPredicate,
        Func<TDeclaration, InspectedDeclarationsAdder, IsValidResult<TMetadata>> isValid,
        InspectedDeclarationsAdder inspectedDeclarations )
        where TDeclaration : class, INamedDeclaration
        where TNameMatchPredicate : INameMatchPredicate
    {
        // NB: The loop is not short-circuited becuase validity checking will build a list of
        // inspected declarations which will be used for diagnostics when applicable.

        var candidateCount = 0;
        var eligibleCount = 0;
        TDeclaration? firstEligible = null;
        TMetadata? firstEligibleMetadata = default;

        foreach ( var declaration in declarations )
        {
            if ( nameMatchPredicate.IsMatch( declaration.Name ) )
            {
                ++candidateCount;
                var result = isValid( declaration, inspectedDeclarations );

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
                    return (DeclarationMatch<TDeclaration>.Invalid(), default);
                }

                break;

            case 1:
                return (DeclarationMatch<TDeclaration>.Success( firstEligible! ), firstEligibleMetadata);

            case > 1:
                return (DeclarationMatch<TDeclaration>.Ambiguous(), default);
        }

        return (DeclarationMatch<TDeclaration>.NotFound( nameMatchPredicate ), default);
    }
}