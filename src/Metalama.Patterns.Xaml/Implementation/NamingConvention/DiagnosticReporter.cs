// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal abstract class DiagnosticReporter
{
    private readonly IAspectBuilder _builder;

    protected DiagnosticReporter( IAspectBuilder builder )
    {
        this._builder = builder;
    }

    // ReSharper disable once UnusedParameter.Global

    /// <summary>
    /// Gets a the <c>TargetDeclarationDescription</c> argument for <see cref="Diagnostics.ValidCandidateDeclarationIsAmbiguous"/> and <see cref="Diagnostics.InvalidCandidateDeclarationSignature"/>.
    /// </summary>
    protected abstract string GetTargetDeclarationDescription();

    /// <summary>
    /// Gets the <c>InvalidityReason</c> argument for <see cref="Diagnostics.InvalidCandidateDeclarationSignature"/>.
    /// </summary>
    /// <param name="addInspectedMember"></param>
    /// <returns></returns>
    protected abstract string GetInvalidityReason( in InspectedMember addInspectedMember );

    private void ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedMember addInspectedMember, bool isRequired )
        => this._builder.Diagnostics.Report(
            Diagnostics.ValidCandidateDeclarationIsAmbiguous.WithArguments(
                (
                    addInspectedMember.Member.DeclarationKind,
                    addInspectedMember.Category,
                    this.GetTargetDeclarationDescription(),
                    this._builder.Target,
                    isRequired ? "as required " : null,
                    namingConvention.Name
                ) ),
            addInspectedMember.Member );

    private void ReportConflictingDeclaration(
        INamingConvention namingConvention,
        IDeclaration conflictingDeclaration,
        IEnumerable<string> applicableCategories,
        bool isRequired )
        => this._builder.Diagnostics.Report(
            Diagnostics.ExistingMemberNameConflict.WithArguments(
                (
                    conflictingDeclaration.DeclarationKind,
                    conflictingDeclaration,
                    (this._builder.Target as IMemberOrNamedType)?.DeclaringType!,
                    isRequired ? "required " : null,
                    applicableCategories.PrettyList( " or " ),
                    namingConvention.Name) ) );

    private void ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedMember addInspectedMember, bool isRequired )
        => this._builder.Diagnostics.Report(
            Diagnostics.InvalidCandidateDeclarationSignature.WithArguments(
                (
                    addInspectedMember.Member.DeclarationKind,
                    addInspectedMember.Category,
                    this.GetTargetDeclarationDescription(),
                    this._builder.Target,
                    isRequired ? "as required " : null,
                    namingConvention.Name,
                    this.GetInvalidityReason( addInspectedMember )
                ) ),
            addInspectedMember.Member );

    private void ReportDeclarationNotFound(
        INamingConvention namingConvention,
        IEnumerable<string> candidateNames,
        IEnumerable<string> applicableCategories )
    {
        var candidateNamesList = candidateNames.PrettyList( " or ", out var candidateNamesPlurality, '\'' );

        this._builder.Diagnostics.Report(
            Diagnostics.CandidateNamesNotFound.WithArguments(
                (
                    applicableCategories.PrettyList( " or " ),
                    namingConvention.Name,
                    candidateNamesPlurality == 2 ? "s" : null,
                    candidateNamesList
                ) ) );
    }

    private void ReportNoNamingConventionMatched( IEnumerable<INamingConvention> namingConventionsTried )
        => this._builder.Diagnostics.Report(
            Diagnostics.NoNamingConventionMatched.WithArguments(
                (namingConventionsTried.Select( nc => nc.Name ).PrettyList( " and ", out var plurality ), plurality == 2 ? "s" : null) ) );

    private void ReportNoConfiguredNamingConventions() => this._builder.Diagnostics.Report( Diagnostics.NoConfiguredNamingConventions );

    private void ReportDiagnostics(
        INamingConvention namingConvention,
        MemberMatchDiagnosticInfo member,
        IEnumerable<InspectedMember> inspectedDeclarations )
    {
        switch ( member.Match.Outcome )
        {
            case MemberMatchOutcome.Ambiguous:

                foreach ( var inspectedDeclaration in inspectedDeclarations )
                {
                    if ( inspectedDeclaration.IsValid && member.Categories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        this.ReportAmbiguousDeclaration( namingConvention, inspectedDeclaration, member.IsRequired );
                    }
                }

                break;

            case MemberMatchOutcome.Invalid:

                foreach ( var inspectedDeclaration in inspectedDeclarations )
                {
                    if ( !inspectedDeclaration.IsValid && member.Categories.Any( c => c == inspectedDeclaration.Category ) )
                    {
                        this.ReportInvalidDeclaration( namingConvention, inspectedDeclaration, member.IsRequired );
                    }
                }

                break;

            case MemberMatchOutcome.NotFound:

                if ( !member.Match.CandidateNames.IsEmpty && member.IsRequired )
                {
                    this.ReportDeclarationNotFound( namingConvention, member.Match.CandidateNames, member.Categories );
                }

                break;

            case MemberMatchOutcome.Conflict:

                this.ReportConflictingDeclaration( namingConvention, member.Match.Member!, member.Categories, member.IsRequired );

                break;
        }
    }

    public void ReportDiagnostics<TMatch>( INamingConventionEvaluationResult<TMatch> evaluationResult )
        where TMatch : NamingConventionMatch
    {
        if ( evaluationResult.Success )
        {
            var match = evaluationResult.SuccessfulMatch.Value.Match;

            foreach ( var member in match.Members )
            {
                this.ReportDiagnostics( match.NamingConvention, member, evaluationResult.SuccessfulMatch.Value.InspectedDeclarations );
            }
        }
        else
        {
            if ( evaluationResult.UnsuccessfulMatches != null )
            {
                this.ReportNoNamingConventionMatched( evaluationResult.UnsuccessfulMatches.Select( um => um.Match.NamingConvention ) );

                foreach ( var unsuccessfulMatch in evaluationResult.UnsuccessfulMatches )
                {
                    foreach ( var member in unsuccessfulMatch.Match.Members )
                    {
                        this.ReportDiagnostics( unsuccessfulMatch.Match.NamingConvention, member, unsuccessfulMatch.InspectedDeclarations );
                    }
                }
            }
            else
            {
                this.ReportNoConfiguredNamingConventions();
            }
        }
    }
}