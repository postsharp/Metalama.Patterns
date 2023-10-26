// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal sealed record DependencyPropertyNamingConventionMatch(
    INamingConvention NamingConvention,
    string? DependencyPropertyName,
    string? RegistrationFieldName,
    DeclarationMatch<IMemberOrNamedType> RegistrationFieldConflictMatch,
    DeclarationMatch<IMethod> PropertyChangingMatch,
    DeclarationMatch<IMethod> PropertyChangedMatch,
    DeclarationMatch<IMethod> ValidateMatch,
    ChangeHandlerSignatureKind PropertyChangingSignatureKind,
    ChangeHandlerSignatureKind PropertyChangedSignatureKind,
    ValidationHandlerSignatureKind ValidationSignatureKind,
    bool RequirePropertyChangingMatch = false,
    bool RequirePropertyChangedMatch = false,
    bool RequireValidateMatch = false ) : INamingConventionMatch
{
    public bool Success
        => !string.IsNullOrWhiteSpace( this.DependencyPropertyName )
           && !string.IsNullOrWhiteSpace( this.RegistrationFieldName )
           && this.RegistrationFieldConflictMatch.Outcome == DeclarationMatchOutcome.Success
           && (this.PropertyChangingMatch.Outcome == DeclarationMatchOutcome.Success
               || (this.RequirePropertyChangingMatch == false && this.PropertyChangingMatch.Outcome == DeclarationMatchOutcome.NotFound))
           && (this.PropertyChangedMatch.Outcome == DeclarationMatchOutcome.Success
               || (this.RequirePropertyChangedMatch == false && this.PropertyChangedMatch.Outcome == DeclarationMatchOutcome.NotFound))
           && (this.ValidateMatch.Outcome == DeclarationMatchOutcome.Success
               || (this.RequireValidateMatch == false && this.ValidateMatch.Outcome == DeclarationMatchOutcome.NotFound));

    private static readonly IReadOnlyList<string> _registrationFieldCategories = new[] { DependencyPropertyAspectBuilder._registrationFieldCategory };
    private static readonly IReadOnlyList<string> _propertyChangingCategories = new[] { DependencyPropertyAspectBuilder._propertyChangingMethodCategory };
    private static readonly IReadOnlyList<string> _propertyChangedCategories = new[] { DependencyPropertyAspectBuilder._propertyChangedMethodCategory };
    private static readonly IReadOnlyList<string> _validateCategories = new[] { DependencyPropertyAspectBuilder._validateMethodCategory };

    public void VisitDeclarationMatches<TVisitor>( in TVisitor visitor )
        where TVisitor : IDeclarationMatchVisitor
    {
        visitor.Visit( this.RegistrationFieldConflictMatch, true, _registrationFieldCategories );
        visitor.Visit( this.PropertyChangingMatch, this.RequirePropertyChangingMatch, _propertyChangingCategories );
        visitor.Visit( this.PropertyChangedMatch, this.RequirePropertyChangedMatch, _propertyChangedCategories );
        visitor.Visit( this.ValidateMatch, this.RequireValidateMatch, _validateCategories );
    }
}