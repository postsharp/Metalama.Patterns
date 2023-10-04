// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using static Metalama.Framework.Diagnostics.Severity;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal static class DiagnosticDescriptors
{
    private const string _category = "Metalama.Patterns.NotifyPropertyChanged";

    // Reserved range 5150-5199

    /// <summary>
    /// Class {0} implements INotifyPropertyChanged but does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName).
    /// </summary>
    public static readonly DiagnosticDefinition<INamedType> ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged =
        new(
            "LAMA5150",
            Error,
            "Class {0} implements INotifyPropertyChanged but does not define a public or protected OnPropertyChanged method with the following signature: "
            +
            "virtual void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged.",
            "OnPropertyChanged is not defined.",
            _category );

    /// <summary>
    /// The project property '{0}' has invalid value '{1}' and will be ignored. The value must {2}.
    /// </summary>
    public static readonly DiagnosticDefinition<(string PropertyName, string PropertyValue, string Reason)> WarningInvalidProjectPropertyValueWillBeIgnored =
        new(
            "LAMA5151",
            Warning,
            "The project property '{0}' has invalid value '{1}' and will be ignored. The value must {2}.",
            "Invalid project property.",
            _category );

    /// <summary>
    /// The type {2} of {0} {1} is a struct implementing INotifyPropertyChanged. Structs implementing INotifyPropertyChanged are not supported.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty, IType ParameterType)>
        ErrorFieldOrPropertyTypeIsStructImplementingInpc =
            new(
                "LAMA5152",
                Error,
                "The type {2} of {0} {1} is a struct implementing INotifyPropertyChanged. Structs implementing INotifyPropertyChanged are not supported.",
                "Property type is struct implementing INotifyPropertyChanged.",
                _category );

    /// <summary>
    /// The type {2} of {0} {1} is an unconstrained generic parameter. The generic parameter must at least be constrained to 'class', 'struct' or 'class, INotifyPropertyChanged'.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty, IType ParameterType)>
        ErrorFieldOrPropertyTypeIsUnconstrainedGeneric =
            new(
                "LAMA5153",
                Error,
                "The type {2} of {0} {1} is an unconstrained generic parameter. The generic parameter must at least be constrained to 'class', 'struct' or 'class, INotifyPropertyChanged'.",
                "Property type is struct implementing INotifyPropertyChanged.",
                _category );

    /// <summary>
    /// The {0} {1} is virtual. This is not supported.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty)> ErrorVirtualMemberIsNotSupported =
        new(
            "LAMA5154",
            Error,
            "The {0} {1} is virtual. This is not supported.",
            "Virtual member is not supported.",
            _category );

    /// <summary>
    /// The {0} {1} is 'new'. This is not supported.
    /// </summary>
    public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty)> ErrorNewMemberIsNotSupported =
        new(
            "LAMA5155",
            Error,
            "The {0} {1} is 'new'. This is not supported.",
            "'new' member is not supported.",
            _category );

    // TODO: Split into multiple diagnostics or keep as one? Which gives the best user experience wrt warnings-as-errors or suppressing warnings?
    /// <summary>
    /// [no fixed message] - use messages like `Only method arguments of primary types are supported`.
    /// </summary>
    public static readonly DiagnosticDefinition<string> WarningNotSupportedForDependencyAnalysis =
        new(
            "LAMA5156",
            Warning,
            "{0}",
            "Not supported for dependency analysis.",
            _category );

    /// <summary>
    /// The type specified for {0} must have a public parameterless constructor.
    /// </summary>
    public static readonly DiagnosticDefinition<string> ErrorTypeMustHaveAPublicParameterlessConstructor =
        new(
            "LAMA5158",
            Error,
            "The type specified for {0} must have a public parameterless constructor.",
            "Type must have a public parameterless constructor.",
            _category );

    /// <summary>
    /// The type specified for {0} must implement {1}.
    /// </summary>
    public static readonly DiagnosticDefinition<(string MemberName, string InterfaceName)> ErrorTypeMustImplementInterface =
        new(
            "LAMA5159",
            Error,
            "The type specified for {0} must implement {1}.",
            "Type must implement the required interface.",
            _category );

    /// <summary>
    /// Handling for this syntax is not implemented and is not supported for dependency analysis (analyzer reference {0}).
    /// </summary>
    public static readonly DiagnosticDefinition<string> WarningNotImplementedForDependencyAnalysis =
        new(
            "LAMA5160",
            Warning,
            "Handling for this syntax is not implemented and is not supported for dependency analysis (analyzer reference {0}).",
            "Not implemented for dependency analysis.",
            _category );
}