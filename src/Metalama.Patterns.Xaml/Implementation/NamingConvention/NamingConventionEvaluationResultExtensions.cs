// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionEvaluationResultExtensions
{
    [CompileTime]
    private readonly struct DeclarationMatchVisitor : IDeclarationMatchVisitor
    {
        public IDeclarationDiagnosticReporter DiagnosticReporter { get; init; }

        public IEnumerable<InspectedDeclaration> InspectedDeclarations { get; init; }

        public INamingConvention NamingConvention { get; init; }

        void IDeclarationMatchVisitor.Visit<TDeclaration>( in DeclarationMatch<TDeclaration> match, bool isRequired, IReadOnlyList<string> applicableCategories )
        {
            if ( match.Outcome == DeclarationMatchOutcome.Ambiguous )
            {
                // Report the ambiguous (valid) matches.
                foreach ( var inspectedDeclaration in this.InspectedDeclarations )
                {
                    if ( inspectedDeclaration.IsValid && applicableCategories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        this.DiagnosticReporter.ReportAmbiguousDeclaration( this.NamingConvention, inspectedDeclaration );
                    }
                }
            }
            else if ( match.Outcome == DeclarationMatchOutcome.Invalid )
            {
                // Report invalid inspections, as these are strong candidates for being intended matches.

                foreach ( var inspectedDeclaration in this.InspectedDeclarations )
                {
                    if ( !inspectedDeclaration.IsValid && applicableCategories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        this.DiagnosticReporter.ReportInvalidDeclaration( this.NamingConvention, inspectedDeclaration );
                    }
                }
            }
            else if ( match.Outcome == DeclarationMatchOutcome.NotFound && match.HasCandidateNames )
            {
                this.DiagnosticReporter.ReportNotFound( this.NamingConvention, match.CandidateNames, applicableCategories );
            }
        }
    }

    public static void ReportUnsuccessfulMatchDiagnostics<TMatch, TDiagnosticReporter>( this INamingConventionEvaluationResult<TMatch> evaluationResult, in TDiagnosticReporter diagnosticReporter )
        where TMatch : INamingConventionMatch
        where TDiagnosticReporter : IDeclarationDiagnosticReporter
    {
        if ( evaluationResult.UnsuccessfulMatches != null )
        {
            foreach ( var unsuccessfulMatch in evaluationResult.UnsuccessfulMatches )
            {
                var visitor = new DeclarationMatchVisitor()
                {
                    InspectedDeclarations = unsuccessfulMatch.InspectedDeclarations,
                    NamingConvention = unsuccessfulMatch.Match.NamingConvention,
                    DiagnosticReporter = diagnosticReporter
                };

                unsuccessfulMatch.Match.VisitDeclarationMatches( visitor );
            }
        }
    }
}