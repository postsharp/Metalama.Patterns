// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionEvaluationResultExtensions
{
    private static void ReportDiagnostics(
        INamingConvention namingConvention,
        NamingConventionMatchMember member,
        IDiagnosticReporter reporter,
        IEnumerable<InspectedDeclaration> inspectedDeclarations )
    {
        switch ( member.Match.Outcome )
        {
            case DeclarationMatchOutcome.Ambiguous:

                foreach ( var inspectedDeclaration in inspectedDeclarations )
                {
                    if ( inspectedDeclaration.IsValid && member.Categories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        reporter.ReportAmbiguousDeclaration( namingConvention, inspectedDeclaration, member.IsRequired );
                    }
                }

                break;

            case DeclarationMatchOutcome.Invalid:

                foreach ( var inspectedDeclaration in inspectedDeclarations )
                {
                    if ( !inspectedDeclaration.IsValid && member.Categories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        reporter.ReportInvalidDeclaration( namingConvention, inspectedDeclaration, member.IsRequired );
                    }
                }

                break;

            case DeclarationMatchOutcome.NotFound:

                if ( member.Match.HasCandidateNames )
                {
                    reporter.ReportDeclarationNotFound( namingConvention, member.Match.CandidateNames, member.Categories, member.IsRequired );
                }

                break;

            case DeclarationMatchOutcome.Conflict:

                reporter.ReportConflictingDeclaration( namingConvention, member.Match.Declaration!, member.Categories, member.IsRequired );

                break;
        }
    }

    public static void ReportDiagnostics<TMatch, TDiagnosticReporter>(
        this INamingConventionEvaluationResult<TMatch> evaluationResult,
        in TDiagnosticReporter diagnosticReporter )
        where TMatch : NamingConventionMatch
        where TDiagnosticReporter : IDiagnosticReporter
    {
        if ( evaluationResult.Success )
        {
            var match = evaluationResult.SuccessfulMatch.Value.Match;

            foreach ( var member in match.Members )
            {
                ReportDiagnostics( match.NamingConvention, member, diagnosticReporter, evaluationResult.SuccessfulMatch.Value.InspectedDeclarations );
            }
        }
        else
        {
            if ( evaluationResult.UnsuccessfulMatches != null )
            {
                diagnosticReporter.ReportNoNamingConventionMatched( evaluationResult.UnsuccessfulMatches.Select( um => um.Match.NamingConvention ) );

                foreach ( var unsuccessfulMatch in evaluationResult.UnsuccessfulMatches )
                {
                    foreach ( var member in unsuccessfulMatch.Match.Members )
                    {
                        ReportDiagnostics( unsuccessfulMatch.Match.NamingConvention, member, diagnosticReporter, unsuccessfulMatch.InspectedDeclarations );
                    }
                }
            }
            else
            {
                diagnosticReporter.ReportNoConfiguredNamingConventions();
            }
        }
    }
}