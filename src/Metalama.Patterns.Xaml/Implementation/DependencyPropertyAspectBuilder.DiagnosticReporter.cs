// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation;

internal sealed partial class DependencyPropertyAspectBuilder
{
    private readonly struct DiagnosticReporter : IDiagnosticReporter
    {
        public IAspectBuilder<IProperty> Builder { get; init; }

        public void ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningValidCandidateDeclarationIsAmbiguous.WithArguments(
                    (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    "[DependencyProperty] property ",
                    this.Builder.Target,
                    namingConvention.DiagnosticName
                    ) ),
                inspectedDeclaration.Declaration );
        }

        public void ReportConflictingDeclaration( INamingConvention namingConvention, IDeclaration conflictingDeclaration, IEnumerable<string> applicableCategories )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningExistingMemberNameConflict.WithArguments(
                    (conflictingDeclaration.DeclarationKind, conflictingDeclaration, this.Builder.Target.DeclaringType, applicableCategories.PrettyList( " or " ), namingConvention.DiagnosticName) ) );
        }

        public void ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningInvalidCandidateDeclarationSignature.WithArguments(
                    (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    "[DependencyProperty] property ",
                    this.Builder.Target,
                    namingConvention.DiagnosticName,
                    " Refer to documentation for supported method signatures."
                    ) ),
                inspectedDeclaration.Declaration );
        }

        public void ReportDeclarationNotFound( INamingConvention namingConvention, IEnumerable<string> candidateNames, IEnumerable<string> applicableCategories )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningCandidateNamesNotFound.WithArguments(
                    (
                        applicableCategories.PrettyList( " or " ),
                        namingConvention.DiagnosticName,
                        candidateNames.PrettyList( " or ", '\'' )
                    ) ) );
        }

        public void ReportNoNamingConventionMatched( IEnumerable<INamingConvention> namingConventionsTried )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.ErrorNoNamingConventionMatched.WithArguments( namingConventionsTried.Select( nc => nc.DiagnosticName ).PrettyList( " and " ) ) );
        }

        public void ReportNoConfiguredNamingConventions()
        {
            this.Builder.Diagnostics.Report( Diagnostics.ErrorNoConfiguredNamingConventions );
        }
    }
}