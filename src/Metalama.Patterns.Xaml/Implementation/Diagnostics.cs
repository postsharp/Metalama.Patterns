// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Patterns.Xaml.Configuration;
using static Metalama.Framework.Diagnostics.Severity;

namespace Metalama.Patterns.Xaml.Implementation;
    
// ReSharper disable InconsistentNaming
[CompileTime]
internal static class Diagnostics
{
    private const string _category = "Metalama.Patterns.Xaml";

    // Reserved range 5200-5220

    /// <summary>
    /// Property {0} has an initializer, but it will not be used because options InitializerProvidesDefaultValue and InitializerProvidesInitialValue are both configured as false.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> WarningDependencyPropertyInitializerWillNotBeUsed =
        new(
            "LAMA5200",
            Warning,
            "Property {0} has an initializer, but it will not be used because options " + nameof(DependencyPropertyOptions.InitializerProvidesDefaultValue) +
            " and " + nameof(DependencyPropertyOptions.InitializerProvidesInitialValue) + " are both configured as false.",
            "Initializer will not be used.",
            _category );

    /// <summary>
    /// The CanExecuteMethod and CanExecuteProperty properties cannot both be defined at the same time.
    /// </summary>
    public static readonly DiagnosticDefinition ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty =
        new(
            "LAMA5201",
            Error,
            "The " + nameof(CommandAttribute.CanExecuteMethod) + " and " + nameof(CommandAttribute.CanExecuteProperty)
            + " properties cannot both be defined at the same time.",
            "Invalid " + nameof(CommandAttribute) + " properties." );

    /// <summary>
    /// The can-execute property for command method {0} is not public, and INotifyPropertyChanged integration is enabled and applicable.
    /// Because the can-execute property is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.
    /// </summary>
    public static readonly DiagnosticDefinition<IMethod> WarningCommandNotifiableCanExecutePropertyIsNotPublic =
        new(
            "LAMA5202",
            Warning,
            "The can-execute property for command method {0} is not public, and INotifyPropertyChanged integration is enabled and applicable. " +
            "Because the can-execute property is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.",
            "Notifiable can-execute property is not public." );

    /// <summary>
    /// The name of existing {0} {1}, that is defined in or inherited by class {2}, conflicts with the {3}{4} name determined by the {5} naming convention.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind ConflictingDeclarationKind, IDeclaration ConflictingDeclaration, INamedType DeclaringType,
            string? Required, string IntroducedMemberDescription, string NamingConvention)>
        WarningExistingMemberNameConflict =
            new(
                "LAMA5203",
                Warning,
                "The name of existing {0} {1}, that is defined in or inherited by class {2}, conflicts with the {3}{4} name determined by the {5} naming convention.",
                "Existing member conflicts with member to be introduced.",
                _category );

    /// <summary>
    /// To be applied to each invalid member:
    /// The {0} was identified as a candidate {1} for {2}{3} {4}by the {5} naming convention, but the signature is not valid.{6}
    /// For example, "The `method` was a candidate `can-execute method` for `[Command] method ``Foo()`, but the signature is not valid.` The method must blah blah.`".
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind DiagnosticTargetDeclaration, string? CandidateDescription, string TargetDeclarationDescription,
            IDeclaration TargetDeclaration, string? AsRequired, string NamingConvention, string? InvalidityReason)>
        WarningInvalidCandidateDeclarationSignature =
            new(
                "LAMA5204",
                Warning,
                "The {0} was identified as a candidate {1} for {2}{3} {4}by the {5} naming convention, but the signature is not valid.{6}",
                "Invalid candidate member signature.",
                _category );

    /// <summary>
    /// The {0} was identified as a valid candidate {1} for {2}{3} {4}by the {5} naming convention, but other members also matched.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind DiagnosticTargetDeclaration, string? CandidateDescription, string TargetDeclarationDescription,
            IDeclaration TargetDeclaration, string? AsRequired, string NamingConvention)>
        WarningValidCandidateDeclarationIsAmbiguous =
            new(
                "LAMA5205",
                Warning,
                "The {0} was identified as a valid candidate {1} for {2}{3} {4}by the {5} naming convention, but other members also matched.",
                "Ambiguous candidate member.",
                _category );

    /// <summary>
    /// No {0} was found {1} the {2} naming convention, with candidate member name{3} {4}.
    /// </summary>
    public static readonly DiagnosticDefinition<(string CandidateDescription, string UsingOrAsRequiredBy, string NamingConvention, string?
            CandidateNamesPluralSuffix, string CandidateNames)>
        WarningCandidateNamesNotFound =
            new(
                "LAMA5206",
                Warning,
                "No {0} was found {1} the {2} naming convention, with candidate member name{3} {4}.",
                "Optional member not found.",
                _category );

    /// <summary>
    /// No match was found using the {0} naming convention{1}. See other warnings for details.
    /// </summary>
    public static readonly DiagnosticDefinition<(string NamingConventions, string? NamingConventionsPluralSuffix)>
        ErrorNoNamingConventionMatched =
            new(
                "LAMA5207",
                Error,
                "No match was found using the {0} naming convention{1}. See other warnings for details.",
                "No configured naming convention matched.",
                _category );

    /// <summary>
    /// No naming conventions are configured. At least one naming convention must be configured.
    /// </summary>
    public static readonly DiagnosticDefinition
        ErrorNoConfiguredNamingConventions =
            new(
                "LAMA5208",
                Error,
                "No naming conventions are configured. At least one naming convention must be configured.",
                "No configured naming conventions.",
                _category );
}