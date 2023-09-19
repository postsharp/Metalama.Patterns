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

    public static class NotifyPropertyChanged
    {
        // Reserved range 5150-5189

        /// <summary>
        /// Class {0} implements INotifyPropertyChanged but does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName).
        /// </summary>
        public static readonly DiagnosticDefinition<INamedType> ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged =
            new(
                "LAMA5150",
                Error,
                "Class {0} implements INotifyPropertyChanged but does not define a public or protected OnPropertyChanged method with the following signature: " +
                "virtual void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged.",
                "OnPropertyChanged is not defined.",
                _category );

        /// <summary>
        /// {0} {1} has an initializer expression and and is of a type that implements INotifyPropertyChanged. This is not supported. Explicit initialization from a constructor is supported.
        /// </summary>
        public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty)> ErrorFieldOrPropertyHasAnInitializerExpression =
            new(
                "LAMA5151",
                Error,
                "{0} {1} has an initializer expression and is of a type that implements INotifyPropertyChanged. This is not supported. Explicit initialization from a constructor is supported.",
                "INotifyPropertyChanged auto-property has an initializer expression.",
                _category );

        /// <summary>
        /// The type {2} of {0} {1} is a struct implementing INotifyPropertyChanged. Structs implementing INotifyPropertyChanged are not supported.
        /// </summary>
        public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty, IType ParameterType)> ErrorFieldOrPropertyTypeIsStructImplementingINPC =
            new(
                "LAMA5152",
                Error,
                "The type {2} of {0} {1} is a struct implementing INotifyPropertyChanged. Structs implementing INotifyPropertyChanged are not supported.",
                "Property type is struct implementing INotifyPropertyChanged.",
                _category );

        /// <summary>
        /// The type {2} of {0} {1} is an unconstrained generic parameter. The generic parameter must at least be constrained to 'class', 'struct' or 'class, INotitfyPropertyChanged'.
        /// </summary>
        public static readonly DiagnosticDefinition<(DeclarationKind Kind, IFieldOrProperty FieldOrProperty, IType ParameterType)> ErrorFieldOrPropertyTypeIsUnconstrainedGeneric =
            new(
                "LAMA5153",
                Error,
                "The type {2} of {0} {1} is an unconstrained generic parameter. The generic parameter must at least be constrained to 'class', 'struct' or 'class, INotifyPropertyChanged'.",
                "Property type is struct implementing INotifyPropertyChanged.",
                _category );
    }

    public static class DependencyGraph
    {
        // Reserved range 5190-5199

        /// <summary>
        /// {0} expressions are not supported for dependency analysis.
        /// </summary>
        public static readonly DiagnosticDefinition<string> ErrorMiscUnsupportedExpression =
            new(
                "LAMA5190",
                Error,
                "{0} expressions are not supported for dependency analysis.",
                "Expression not supported for dependency analysis.",
                _category );

        /// <summary>
        /// The identifier '{0}' of kind {1} is not supported for dependency analysis.
        /// </summary>
        public static readonly DiagnosticDefinition<(string Identifier, string Kind)> ErrorMiscUnsupportedIdentifier =
            new(
                "LAMA5191",
                Error,
                "The identifier '{0}' of kind {1} is not supported for dependency analysis.",
                "Identifier not supported for dependency analysis.",
                _category );
    }
}