﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Options;

/// <summary>
/// Applies options specific to the "natural" implementation of the <see cref="ObservableAttribute"/> aspect.
/// </summary>
[PublicAPI]
[RunTimeOrCompileTime]
[AttributeUsage( AttributeTargets.Assembly | AttributeTargets.Class )]
public sealed class ClassicImplementationStrategyOptionsAttribute
    : Attribute, IHierarchicalOptionsProvider
{
    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        return new[]
        {
            new ClassicImplementationStrategyOptions
            {
                EnableOnUnmonitoredObservablePropertyChangedMethod = this._enableOnUnmonitoredObservablePropertyChangedMethod
            }
        };
    }

    private bool? _enableOnUnmonitoredObservablePropertyChangedMethod;

    /// <summary>
    /// Gets or sets a value indicating whether the <c>OnUnmonitoredObservablePropertyChanged</c> method should be introduced.
    /// </summary>
    /// <remarks>
    /// The <c>OnUnmonitoredObservablePropertyChanged</c> method allows a derived class to efficiently add subscribe/unsubscribe functionality
    /// to a base class property, where the base class itself does not need to observe changes to children of the property.
    /// This only applies to properties where the property type implements <see cref="INotifyPropertyChanged"/>. The introduced
    /// method allows the current target class to use the feature if provided by a base class, and allows the current target class
    /// to provide the feature to derived classes for applicable properties.
    /// </remarks>
    public bool EnableOnUnmonitoredObservablePropertyChangedMethod
    {
        get => this._enableOnUnmonitoredObservablePropertyChangedMethod ?? true;

        set => this._enableOnUnmonitoredObservablePropertyChangedMethod = value;
    }
}