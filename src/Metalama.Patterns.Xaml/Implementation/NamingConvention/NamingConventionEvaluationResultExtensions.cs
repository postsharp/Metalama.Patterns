// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionEvaluationResultExtensions
{
    [CompileTime]
    private readonly struct ReportDiagnosticsVisitor : IDeclarationMatchVisitor
    {
        public IDiagnosticReporter DiagnosticReporter { get; init; }

        public IEnumerable<InspectedDeclaration> InspectedDeclarations { get; init; }

        public INamingConvention NamingConvention { get; init; }

        void IDeclarationMatchVisitor.Visit<TDeclaration>(
            in DeclarationMatch<TDeclaration> match,
            bool isRequired,
            IReadOnlyList<string> applicableCategories )
        {
            switch ( match.Outcome )
            {
                case DeclarationMatchOutcome.Ambiguous:

                    foreach ( var inspectedDeclaration in this.InspectedDeclarations )
                    {
                        if ( inspectedDeclaration.IsValid && applicableCategories.Any( c => c == inspectedDeclaration.Category ) )
                        {
                            this.DiagnosticReporter.ReportAmbiguousDeclaration( this.NamingConvention, inspectedDeclaration, isRequired );
                        }
                    }

                    break;

                case DeclarationMatchOutcome.Invalid:

                    foreach ( var inspectedDeclaration in this.InspectedDeclarations )
                    {
                        if ( !inspectedDeclaration.IsValid && applicableCategories.Any( c => c == inspectedDeclaration.Category ) )
                        {
                            this.DiagnosticReporter.ReportInvalidDeclaration( this.NamingConvention, inspectedDeclaration, isRequired );
                        }
                    }

                    break;

                case DeclarationMatchOutcome.NotFound:

                    if ( match.HasCandidateNames )
                    {
                        this.DiagnosticReporter.ReportDeclarationNotFound( this.NamingConvention, match.CandidateNames, applicableCategories, isRequired );
                    }

                    break;

                case DeclarationMatchOutcome.Conflict:

                    this.DiagnosticReporter.ReportConflictingDeclaration( this.NamingConvention, match.Declaration!, applicableCategories, isRequired );

                    break;
            }
        }
    }

    public static void ReportDiagnostics<TMatch, TDiagnosticReporter>(
        this INamingConventionEvaluationResult<TMatch> evaluationResult,
        in TDiagnosticReporter diagnosticReporter )
        where TMatch : INamingConventionMatch
        where TDiagnosticReporter : IDiagnosticReporter
    {
        if ( evaluationResult.Success )
        {
            var visitor = new ReportDiagnosticsVisitor()
            {
                InspectedDeclarations = evaluationResult.SuccessfulMatch.Value.InspectedDeclarations,
                NamingConvention = evaluationResult.SuccessfulMatch.Value.Match.NamingConvention,
                DiagnosticReporter = diagnosticReporter
            };

            evaluationResult.SuccessfulMatch.Value.Match.VisitDeclarationMatches( visitor );
        }
        else
        {
            if ( evaluationResult.UnsuccessfulMatches != null )
            {
                diagnosticReporter.ReportNoNamingConventionMatched( evaluationResult.UnsuccessfulMatches.Select( um => um.Match.NamingConvention ) );

                foreach ( var unsuccessfulMatch in evaluationResult.UnsuccessfulMatches )
                {
                    var visitor = new ReportDiagnosticsVisitor()
                    {
                        InspectedDeclarations = unsuccessfulMatch.InspectedDeclarations,
                        NamingConvention = unsuccessfulMatch.Match.NamingConvention,
                        DiagnosticReporter = diagnosticReporter
                    };

                    unsuccessfulMatch.Match.VisitDeclarationMatches( visitor );
                }
            }
            else
            {
                diagnosticReporter.ReportNoConfiguredNamingConventions();
            }
        }
    }
}