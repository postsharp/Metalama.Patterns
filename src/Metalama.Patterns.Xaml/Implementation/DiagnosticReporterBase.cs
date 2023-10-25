// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal abstract class DiagnosticReporterBase : IDiagnosticReporter
{
    private readonly IAspectBuilder _builder;

    protected DiagnosticReporterBase(IAspectBuilder builder)
    {
        this._builder = builder;
    }

    /// <summary>
    /// Gets a the <c>TargetDeclarationDescription</c> argument for <see cref="Diagnostics.WarningValidCandidateDeclarationIsAmbiguous"/> and <see cref="Diagnostics.WarningInvalidCandidateDeclarationSignature"/>.
    /// </summary>
    protected abstract string GetTargetDeclarationDescription( in InspectedDeclaration inspectedDeclaration );

    /// <summary>
    /// Gets the <c>InvalidityReason</c> argument for <see cref="Diagnostics.WarningInvalidCandidateDeclarationSignature"/>.
    /// </summary>
    /// <param name="inspectedDeclaration"></param>
    /// <returns></returns>
    protected abstract string GetInvalidityReason( in InspectedDeclaration inspectedDeclaration );

    void IDiagnosticReporter.ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningValidCandidateDeclarationIsAmbiguous.WithArguments(
                (
                inspectedDeclaration.Declaration.DeclarationKind,
                inspectedDeclaration.Category,
                this.GetTargetDeclarationDescription( inspectedDeclaration ),
                this._builder.Target,
                namingConvention.DiagnosticName
                ) ),
            inspectedDeclaration.Declaration );
    }

    void IDiagnosticReporter.ReportConflictingDeclaration( INamingConvention namingConvention, IDeclaration conflictingDeclaration, IEnumerable<string> applicableCategories )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningExistingMemberNameConflict.WithArguments(
                (conflictingDeclaration.DeclarationKind, conflictingDeclaration, (this._builder.Target as IMemberOrNamedType)?.DeclaringType!, applicableCategories.PrettyList( " or " ), namingConvention.DiagnosticName) ) );
    }

    void IDiagnosticReporter.ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningInvalidCandidateDeclarationSignature.WithArguments(
                (
                inspectedDeclaration.Declaration.DeclarationKind,
                inspectedDeclaration.Category,
                this.GetTargetDeclarationDescription( inspectedDeclaration ),
                this._builder.Target,
                namingConvention.DiagnosticName,
                this.GetInvalidityReason( inspectedDeclaration )
                ) ),
            inspectedDeclaration.Declaration );
    }

    void IDiagnosticReporter.ReportDeclarationNotFound( INamingConvention namingConvention, IEnumerable<string> candidateNames, IEnumerable<string> applicableCategories )
    {
        var candidateNamesList = candidateNames.PrettyList( " or ", out var candidateNamesPlurality, '\'' );
        
        this._builder.Diagnostics.Report(
            Diagnostics.WarningCandidateNamesNotFound.WithArguments(
                (
                    applicableCategories.PrettyList( " or " ),
                    namingConvention.DiagnosticName,
                    candidateNamesPlurality == 2 ? "s" : null,
                    candidateNamesList
                ) ) );
    }

    void IDiagnosticReporter.ReportNoNamingConventionMatched( IEnumerable<INamingConvention> namingConventionsTried )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.ErrorNoNamingConventionMatched.WithArguments( (namingConventionsTried.Select( nc => nc.DiagnosticName ).PrettyList( " and ", out var plurality ), plurality == 2 ? "s" : null) ) );
    }

    void IDiagnosticReporter.ReportNoConfiguredNamingConventions()
    {
        this._builder.Diagnostics.Report( Diagnostics.ErrorNoConfiguredNamingConventions );
    }
}
