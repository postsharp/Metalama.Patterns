// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionHelper
{
    public static (DeclarationMatchOutcome Outcome, TDeclaration? Declaration) FindValidMatchingDeclaration<TDeclaration, TNameMatchPredicate>(
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

        TDeclaration? useDeclaration = null;
        var outcome = DeclarationMatchOutcome.NotFound;

        switch ( eligibleCount )
        {
            case 0:
                if ( candidateCount > 0 )
                {
                    outcome = DeclarationMatchOutcome.Invalid;
                }
                break;

            case 1:
                useDeclaration = firstEligible;
                outcome = DeclarationMatchOutcome.Success;
                break;

            case > 1:
                outcome = DeclarationMatchOutcome.Ambiguous;
                break;
        }

        return (outcome, useDeclaration);
    }
}