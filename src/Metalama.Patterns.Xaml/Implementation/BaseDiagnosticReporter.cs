// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal abstract class BaseDiagnosticReporter : IDiagnosticReporter
{
    private readonly IAspectBuilder _builder;

    protected BaseDiagnosticReporter( IAspectBuilder builder )
    {
        this._builder = builder;
    }

    // ReSharper disable once UnusedParameter.Global

    /// <summary>
    /// Gets a the <c>TargetDeclarationDescription</c> argument for <see cref="Diagnostics.WarningValidCandidateDeclarationIsAmbiguous"/> and <see cref="Diagnostics.WarningInvalidCandidateDeclarationSignature"/>.
    /// </summary>
    protected abstract string GetTargetDeclarationDescription();

    /// <summary>
    /// Gets the <c>InvalidityReason</c> argument for <see cref="Diagnostics.WarningInvalidCandidateDeclarationSignature"/>.
    /// </summary>
    /// <param name="inspectedDeclaration"></param>
    /// <returns></returns>
    protected abstract string GetInvalidityReason( in InspectedDeclaration inspectedDeclaration );

    void IDiagnosticReporter.ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration, bool isRequired )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningValidCandidateDeclarationIsAmbiguous.WithArguments(
                (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    this.GetTargetDeclarationDescription(),
                    this._builder.Target,
                    isRequired ? "as required " : null,
                    namingConvention.Name
                ) ),
            inspectedDeclaration.Declaration );
    }

    void IDiagnosticReporter.ReportConflictingDeclaration(
        INamingConvention namingConvention,
        IDeclaration conflictingDeclaration,
        IEnumerable<string> applicableCategories,
        bool isRequired )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningExistingMemberNameConflict.WithArguments(
                (
                    conflictingDeclaration.DeclarationKind,
                    conflictingDeclaration,
                    (this._builder.Target as IMemberOrNamedType)?.DeclaringType!,
                    isRequired ? "required " : null,
                    applicableCategories.PrettyList( " or " ),
                    namingConvention.Name) ) );
    }

    void IDiagnosticReporter.ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration, bool isRequired )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.WarningInvalidCandidateDeclarationSignature.WithArguments(
                (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    this.GetTargetDeclarationDescription(),
                    this._builder.Target,
                    isRequired ? "as required " : null,
                    namingConvention.Name,
                    this.GetInvalidityReason( inspectedDeclaration )
                ) ),
            inspectedDeclaration.Declaration );
    }

    void IDiagnosticReporter.ReportDeclarationNotFound(
        INamingConvention namingConvention,
        IEnumerable<string> candidateNames,
        IEnumerable<string> applicableCategories,
        bool isRequired )
    {
        var candidateNamesList = candidateNames.PrettyList( " or ", out var candidateNamesPlurality, '\'' );

        this._builder.Diagnostics.Report(
            Diagnostics.WarningCandidateNamesNotFound.WithArguments(
                (
                    applicableCategories.PrettyList( " or " ),
                    isRequired ? "as required by" : "using",
                    namingConvention.Name,
                    candidateNamesPlurality == 2 ? "s" : null,
                    candidateNamesList
                ) ) );
    }

    void IDiagnosticReporter.ReportNoNamingConventionMatched( IEnumerable<INamingConvention> namingConventionsTried )
    {
        this._builder.Diagnostics.Report(
            Diagnostics.ErrorNoNamingConventionMatched.WithArguments(
                (namingConventionsTried.Select( nc => nc.Name ).PrettyList( " and ", out var plurality ), plurality == 2 ? "s" : null) ) );
    }

    void IDiagnosticReporter.ReportNoConfiguredNamingConventions()
    {
        this._builder.Diagnostics.Report( Diagnostics.ErrorNoConfiguredNamingConventions );
    }
}