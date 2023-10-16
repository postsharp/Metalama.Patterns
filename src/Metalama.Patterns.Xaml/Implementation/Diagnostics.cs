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
}