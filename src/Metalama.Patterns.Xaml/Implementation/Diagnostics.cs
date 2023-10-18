// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Patterns.Xaml.Options;
using System.Windows.Navigation;
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
    /// Options CanExecuteMethod and CanExecuteProperty cannot both be defined at the same time.
    /// </summary>
    public static readonly DiagnosticDefinition ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty =
        new(
            "LAMA5205",
            Error,
            "Options " + nameof( CommandOptions.CanExecuteMethod ) + " and " + nameof( CommandOptions.CanExecuteProperty ) + " cannot both be defined at the same time.",
            "Invalid " + nameof( CommandOptions ) + "." );

    /// <summary>
    /// The {0} ExecuteMethod and {1} CanExecuteMethod cannot both have the same value '{2}'.
    /// </summary>
    public static readonly DiagnosticDefinition<(string ExecuteConfigurationSource, string CanExecuteConfigurationSource, string Value)> ErrorCommandExecuteAndCanExecuteCannotBeTheSame =
        new(
            "LAMA5206",
            Error,
            "The {0} " + nameof( CommandOptions.ExecuteMethod ) + " and {1} " + nameof( CommandOptions.CanExecuteMethod ) + " cannot both have the same value '{2}'.",
            "Invalid " + nameof( CommandOptions ) + "." );

    /// <summary>
    /// The CanExecuteProperty for command property {0} must be of type bool.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> ErrorCommandCanExecutePropertyIsNotValid =
        new(
            "LAMA5207",
            Error,
            "The " + nameof( CommandOptions.CanExecuteProperty ) + " for command property {0} must be of type bool and have a getter.",
            "Invalid " + nameof( CommandOptions.CanExecuteProperty ) + " type." );

    /// <summary>
    /// The configured CanExecuteProperty '{0}' was not found.
    /// </summary>
    public static readonly DiagnosticDefinition<string> ErrorCommandConfiguredCanExecutePropertyNotFound =
        new(
            "LAMA5208",
            Error,
            "The configured " + nameof( CommandOptions.CanExecuteProperty ) + " '{0}' was not found.",
            "Missing " + nameof( CommandOptions.CanExecuteProperty ) + "." );

    /// <summary>
    /// The CanExecuteMethod for command property {0} must return bool and may optionally have a single parameter of any type, and which must not be a ref or out parameter.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> ErrorCommandCanExecuteMethodIsNotValid =
        new(
            "LAMA5209",
            Error,
            "The " + nameof( CommandOptions.CanExecuteMethod ) + " for command property {0} must not be generic, must return bool and may optionally have a single parameter of any type, and which must not be a ref or out parameter.",
            "Invalid " + nameof( CommandOptions.CanExecuteMethod ) + "." );

    /// <summary>
    /// The configured CanExecuteMethod '{0}' was not found.
    /// </summary>
    public static readonly DiagnosticDefinition<string> ErrorCommandConfiguredCanExecuteMethodNotFound =
        new(
            "LAMA5210",
            Error,
            "The configured " + nameof( CommandOptions.CanExecuteMethod ) + " '{0}' was not found.",
            "Missing " + nameof( CommandOptions.CanExecuteMethod ) + "." );

    /// <summary>
    /// The type {0} contains more than one method named {1}.
    /// </summary>
    public static readonly DiagnosticDefinition<(IType DeclaringType, string MethodName)> ErrorCommandCanExecuteMethodIsAmbiguous =
        new(
            "LAMA5211",
            Error,
            "The type {0} contains more than one method named {1}.",
            "Ambiguous " + nameof( CommandOptions.CanExecuteMethod ) + "." );

    /// <summary>
    /// The ExecuteMethod named {0} was not found.
    /// </summary>
    public static readonly DiagnosticDefinition<string> ErrorCommandExecuteMethodNotFound =
        new(
            "LAMA5212",
            Error,
            "The " + nameof( CommandOptions.ExecuteMethod ) + " named {0} was not found.",
            "Missing " + nameof( CommandOptions.ExecuteMethod ) + "." );

    /// <summary>
    /// The ExecuteMethod for command property {0} must return bool and may optionally have a single parameter of any type, and which must not be a ref or out parameter.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> ErrorCommandExecuteMethodIsNotValid =
        new(
            "LAMA5213",
            Error,
            "The " + nameof( CommandOptions.ExecuteMethod ) + " for command property {0} must not be generic, must return bool and may optionally have a single parameter of any type, and which must not be a ref or out parameter.",
            "Invalid " + nameof( CommandOptions.ExecuteMethod ) + "." );

    /// <summary>
    /// The type {0} contains more than one method named {1}.
    /// </summary>
    public static readonly DiagnosticDefinition<(IType DeclaringType, string MethodName)> ErrorCommandExecuteMethodIsAmbiguous =
        new(
            "LAMA5214",
            Error,
            "The type {0} contains more than one method named {1}.",
            "Ambiguous " + nameof( CommandOptions.ExecuteMethod ) + "." );

    /// <summary>
    /// The CanExecuteProperty for command property {0} is not public, and INotifyPropertyChanged integration is enabled and applicable.
    /// Because the CanExecuteProperty is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.
    /// </summary>
    public static readonly DiagnosticDefinition<IProperty> WarniningCommandNotifiableCanExecutePropertyIsNotPublic =
        new(
            "LAMA5215",
            Warning,
            "The " + nameof(CommandOptions.CanExecuteProperty) + " for command property {0} is not public, and INotifyPropertyChanged integration is enabled and applicable. " +
            "Because the " + nameof(CommandOptions.CanExecuteProperty) + " is not public, INotifyPropertyChanged.PropertyChanged events might not be raised depending on the INotifyPropertyChanged implementation.",
            "Notifiable " + nameof( CommandOptions.CanExecuteProperty) + " is not public." );
}