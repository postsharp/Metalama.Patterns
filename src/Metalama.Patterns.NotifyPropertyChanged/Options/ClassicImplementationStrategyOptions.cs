// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

/// <summary>
/// Options specific to the "natural" implementation.
/// </summary>
[PublicAPI]
[CompileTime]
internal sealed record ClassicImplementationStrategyOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>,
                                                            IHierarchicalOptions<INamedType>
{
    /// <summary>
    /// Gets a value indicating whether the <c>OnUnmonitoredObservablePropertyChanged</c> method should be introduced.
    /// </summary>
    /// <remarks>
    /// The <c>OnUnmonitoredObservablePropertyChanged</c> method allows a derived class to efficiently add subscribe/unsubscribe functionality
    /// to a base class property, where the base class itself does not need to observe changes to children of the property.
    /// This only applies to properties where the property type implements <see cref="INotifyPropertyChanged"/>. The introduced
    /// method allows the current target class to use the feature if provided by a base class, and allows the current target class
    /// to provide the feature to derived classes for applicable properties.
    /// </remarks>
    public bool? EnableOnUnmonitoredObservablePropertyChangedMethod { get; init; }

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (ClassicImplementationStrategyOptions) changes;

        return new ClassicImplementationStrategyOptions
        {
            EnableOnUnmonitoredObservablePropertyChangedMethod = other.EnableOnUnmonitoredObservablePropertyChangedMethod
                                                                 ?? this.EnableOnUnmonitoredObservablePropertyChangedMethod
        };
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
        => new ClassicImplementationStrategyOptions() { EnableOnUnmonitoredObservablePropertyChangedMethod = true };
}