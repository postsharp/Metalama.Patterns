// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Patterns.Xaml.Options;
using static Metalama.Framework.Diagnostics.Severity;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal static class Diagnostics
{
    private const string _category = "Metalama.Patterns.Xaml";

    // Reserved range 5200-5220

    /// <summary>
    /// Class {0} does not contain a method named '{1}' as configured by the {2} option.
    /// </summary>
    public static readonly DiagnosticDefinition<(INamedType DeclaringType, string MethodName, string OptionName)> WarningConfiguredHandlerMethodNotFound =
        new(
            "LAMA5200",
            Warning,
            "Class {0} does not contain a method named '{1}' as configured by the {2} option.",
            "Configured handler method not found.",
            _category );

    /// <summary>
    /// Property {0} has an initializer, but it will not be used because options InitializerProvidesDefaultValue and InitializerProvidesInitialValue are both configured as false.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> WarningDependencyPropertyInitializerWillNotBeUsed =
        new(
            "LAMA5201",
            Warning,
            "Property {0} has an initializer, but it will not be used because options " + nameof(DependencyPropertyOptions.InitializerProvidesDefaultValue) +
            " and " + nameof(DependencyPropertyOptions.InitializerProvidesInitialValue) + " are both configured as false.",
            "Initializer will not be used.",
            _category );

    /// <summary>
    /// Class {0} has more than one method named '{1}'. Resolve the ambiguity or configure a different method name using the {2} option.
    /// </summary>
    public static readonly DiagnosticDefinition<(INamedType DeclaringType, string MethodName, string OptionName)> ErrorHandlerMethodIsAmbiguous =
        new(
            "LAMA5202",
            Error,
            "Class {0} has more than one method named '{1}'. Resolve the ambiguity or configure a different method name using the {2} option.",
            "Handler method is ambiguous.",
            _category );

    /// <summary>
    /// The signature of the {0} handler method for dependency property {1} is not valid. Refer to documentation for valid signatures.
    /// </summary>
    public static readonly DiagnosticDefinition<(string OptionName, IProperty TargetProperty)> ErrorHandlerMethodIsInvalid =
        new(
            "LAMA5203",
            Error,

            // TODO: The list of valid signatures is quite lengthy. Should they be listed in the diagnostic message? At the moment they are not.
            "The signature of the {0} handler method for dependency property {1} is not valid. Refer to documentation for valid signatures.",
            "Handler method signature is invalid.",
            _category );

    /// <summary>
    /// The name of existing member {0} that is defined in or inherited by class {1} conflicts with the required dependency property field name {2}.
    /// </summary>
    public static readonly DiagnosticDefinition<(IMemberOrNamedType Member, INamedType DeclaringType, string FieldName)>
        ErrorRequiredDependencyPropertyFieldNameIsAlreadyUsed =
            new(
                "LAMA5204",
                Error,
                "The name of existing member {0}, that is defined in or inherited by class {1}, conflicts with the required dependency property field name {2}.",
                "Required dependency property field name is already used.",
                _category );

    /// <summary>
    /// The CanExecuteMethod and CanExecuteProperty properties cannot both be defined at the same time.
    /// </summary>
    public static readonly DiagnosticDefinition ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty =
        new(
            "LAMA5205",
            Error,
            "The " + nameof( CommandAttribute.CanExecuteMethod) + " and " + nameof(CommandAttribute.CanExecuteProperty)
            + " properties cannot both be defined at the same time.",
            "Invalid " + nameof(CommandAttribute) + " properties." );

    /// <summary>
    /// The can-execute property for command method {0} is not public, and INotifyPropertyChanged integration is enabled and applicable.
    /// Because the can-execute property is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.
    /// </summary>
    public static readonly DiagnosticDefinition<IMethod> WarningCommandNotifiableCanExecutePropertyIsNotPublic =
        new(
            "LAMA5206",
            Warning,
            "The can-execute property for command method {0} is not public, and INotifyPropertyChanged integration is enabled and applicable. " +
            "Because the can-execute property is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.",
            "Notifiable can-execute property is not public." );

    /// <summary>
    /// The name of existing member {0} that is defined in or inherited by class {1} conflicts with the required command property name {2}.
    /// </summary>
    public static readonly DiagnosticDefinition<(IMemberOrNamedType Member, INamedType DeclaringType, string FieldName)>
        ErrorRequiredCommandPropertyNameIsAlreadyUsed =
            new(
                "LAMA5207",
                Error,
                "The name of existing member {0}, that is defined in or inherited by class {1}, conflicts with the required command property name {2}.",
                "Required command property name is already used.",
                _category );

    /// <summary>
    /// To be appplied to each invalid member:
    /// The {0} was identified as candidate {1} for {2}{3} by the {4} naming convention, but the signature is not valid.{5}
    /// For example, "The `method` was a candidate `can-execute method` for `[Command] method ``Foo()`, but the signature is not valid.` The method must blah blah.`".
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind DiagnosticTargetDeclaration, string? CandidateDescription, string TargetDeclarationDescription, IDeclaration TargetDeclaration, string NamingConvention, string? InvalidityReason)>
        WarningInvalidCandidateDeclarationSignature =
            new(
                "LAMA5208",
                Warning,
                "The {0} was identified as a candidate {1} for {2}{3} by the {4} naming convention, but the signature is not valid.{5}",
                "Invalid candidate member signature.",
                _category );

    /// <summary>
    /// The {0} was identified as a valid candidate {1} for {2}{3} by the {4} naming convention, but other members also matched.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind DiagnosticTargetDeclaration, string? CandidateDescription, string TargetDeclarationDescription, IDeclaration TargetDeclaration, string NamingConvention)>
        WarningValidCandidateDeclarationIsAmbiguous =
        new(
            "LAMA5209",
            Warning,
            "The {0} was identified as a valid candidate {1} for {2}{3} by the {4} naming convention, but other members also matched.",
            "Ambiguous candidate member.",
            _category );

    /// <summary>
    /// No {0} was found using the {1} naming convention candidate member names {2}.
    /// </summary>
    public static readonly DiagnosticDefinition<(string CandidateDescription, string NamingConvention, string CandidateNames)>
        WarningCandidateNamesNotFound =
        new(
            "LAMA5210",
            Warning,
            "No {0} was found using the {1} naming convention candidate member names {2}.",
            "Optional member not found.",
            _category );

    /// <summary>
    /// No configured naming convention matched, see other warnings for details. Tried naming conventions {0}.
    /// </summary>
    public static readonly DiagnosticDefinition<string>
        ErrorNoNamingConventionMatched =
        new(
            "LAMA5211",
            Error,
            "No configured naming convention matched, see other warnings for details. Tried naming conventions {0}",
            "No configured naming convention matched.",
            _category );

    /// <summary>
    /// No naming conventions are configured. At least one naming convention must be configured.
    /// </summary>
    public static readonly DiagnosticDefinition
        ErrorNoConfiguredNamingConventions =
        new(
            "LAMA5212",
            Error,
            "No naming conventions are configured. At least one naming convention must be configured.",
            "No configured naming conventions.",
            _category );
}