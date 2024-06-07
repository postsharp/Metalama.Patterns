// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Collections.Immutable;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal sealed record DependencyPropertyNamingConventionMatch(
    INamingConvention NamingConvention,
    string? DependencyPropertyName,
    string? RegistrationFieldName,
    MemberMatch<IMemberOrNamedType, DefaultMatchKind> RegistrationFieldConflictMatch,
    MemberMatch<IMethod, ChangeHandlerSignatureKind> PropertyChangingMatch,
    MemberMatch<IMethod, ChangeHandlerSignatureKind> PropertyChangedMatch,
    MemberMatch<IMethod, ValidationHandlerSignatureKind> ValidateMatch,
    bool RequirePropertyChangingMatch = false,
    bool RequirePropertyChangedMatch = false,
    bool RequireValidateMatch = false ) : NamingConventionMatch( NamingConvention )
{
    public override bool Success
        => !string.IsNullOrWhiteSpace( this.DependencyPropertyName )
           && !string.IsNullOrWhiteSpace( this.RegistrationFieldName )
           && this.RegistrationFieldConflictMatch.Outcome == MemberMatchOutcome.Success
           && (this.PropertyChangingMatch.Outcome == MemberMatchOutcome.Success
               || (this.RequirePropertyChangingMatch == false && this.PropertyChangingMatch.Outcome == MemberMatchOutcome.NotFound))
           && (this.PropertyChangedMatch.Outcome == MemberMatchOutcome.Success
               || (this.RequirePropertyChangedMatch == false && this.PropertyChangedMatch.Outcome == MemberMatchOutcome.NotFound))
           && (this.ValidateMatch.Outcome == MemberMatchOutcome.Success
               || (this.RequireValidateMatch == false && this.ValidateMatch.Outcome == MemberMatchOutcome.NotFound));

    protected override ImmutableArray<MemberMatchDiagnosticInfo> GetMembers()
        => ImmutableArray.Create(
            new MemberMatchDiagnosticInfo( this.RegistrationFieldConflictMatch, true, _registrationFieldCategories ),
            new MemberMatchDiagnosticInfo( this.PropertyChangingMatch, this.RequirePropertyChangingMatch, _propertyChangingCategories ),
            new MemberMatchDiagnosticInfo( this.PropertyChangedMatch, this.RequirePropertyChangedMatch, _propertyChangedCategories ),
            new MemberMatchDiagnosticInfo( this.ValidateMatch, this.RequireValidateMatch, _validateCategories ) );

    private static readonly ImmutableArray<string> _registrationFieldCategories =
        ImmutableArray.Create( DependencyPropertyAspectBuilder.RegistrationFieldCategory );

    private static readonly ImmutableArray<string> _propertyChangingCategories =
        ImmutableArray.Create( DependencyPropertyAspectBuilder.PropertyChangingMethodCategory );

    private static readonly ImmutableArray<string> _propertyChangedCategories =
        ImmutableArray.Create( DependencyPropertyAspectBuilder.PropertyChangedMethodCategory );

    private static readonly ImmutableArray<string> _validateCategories = ImmutableArray.Create( DependencyPropertyAspectBuilder.ValidateMethodCategory );
}