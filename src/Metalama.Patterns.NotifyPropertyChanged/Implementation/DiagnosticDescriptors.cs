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

    public static class NotifyPropertyChanged
    {
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
    }
}