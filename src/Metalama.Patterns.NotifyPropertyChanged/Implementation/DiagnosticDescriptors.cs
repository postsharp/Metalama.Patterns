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
        // TODO: Review and remove unused.

        /// <summary>
        /// Class {0} implements INotifyPropertyChanged but does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName).
        /// </summary>
        public static readonly DiagnosticDefinition<INamedType> ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged =
            new(
                "LAMA5150",
                Error,
                "Class {0} implements INotifyPropertyChanged but does not define a public or protected OnPropertyChanged method with the following signature: " +
                "void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged.",
                "OnPropertyChanged is not defined.",
                _category );

        /// <summary>
        /// Class {0} defines a PropertyChanged event which is not compatible with implicit implementation of INotifyPropertyChanged.PropertyChanged. The event must be public and be of type PropertyChangedEventHandler.
        /// </summary>
        public static readonly DiagnosticDefinition<INamedType> ErrorClassDefinesIncompatiblePropertyChangedEvent =
            new(
                "LAMA5151",
                Error,
                "Class {0} defines a PropertyChanged event which is not compatible with implicit implementation of INotifyPropertyChanged.PropertyChanged. " +
                "The event must be public and be of type PropertyChangedEventHandler.",
                "PropertyChanged event is not compatible.",
                _category );

        // TODO: Remove if not used.
        /// <summary>
        /// Class {0} defines a {1:method|field|...} named PropertyChanged which prevents implicit implementation of INotifyPropertyChanged.PropertyChanged.
        /// </summary>
        [Obsolete("Let IntroduceXXX report failure.", true)]
        public static readonly DiagnosticDefinition<(INamedType Target, string MemberKind)> ErrorClassDefinesNonEventPropertyChangedMember =
            new(
                "LAMA5152",
                Error,
                "Class {0} defines a {1} named PropertyChanged which prevents implicit implementation of INotifyPropertyChanged.PropertyChanged.",
                "PropertyChanged member prevents implementation of INotifyPropertyChanged.",
                _category );

        /// <summary>
        /// Class {0} defines a PropertyChanged event that is compatible with implicit implementation of INotifyPropertyChanged.PropertyChanged, but
        /// does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName). The method name can
        /// also be NotifyOfPropertyChange or RaisePropertyChanged.
        /// </summary>
        public static readonly DiagnosticDefinition<INamedType> ErrorClassDefinesEventButDoesNotDefineOnPropertyChanged =
            new(
                "LAMA5153",
                Error,
                "Class {0} defines a PropertyChanged event that is compatible with implicit implementation of INotifyPropertyChanged.PropertyChanged, but" +
                "does not define a public or protected OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName). The method name can " +
                "also be NotifyOfPropertyChange or RaisePropertyChanged.",
                "OnPropertyChanged is not defined.",
                _category );
    }
}