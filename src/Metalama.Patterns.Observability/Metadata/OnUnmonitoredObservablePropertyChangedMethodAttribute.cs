// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.Observability.Metadata;

/// <summary>
/// Indicates compliance with the "OnUnmonitoredObservablePropertyChanged" contract expected by the <see cref="ObservableAttribute"/> aspect.
/// This attribute should only be applied automatically by the <see cref="ObservableAttribute"/> aspect.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
public sealed class OnUnmonitoredObservablePropertyChangedMethodAttribute : MetadataAttribute
{
    // ReSharper disable once UnusedParameter.Local
    /// <summary>
    /// Initializes a new instance of the <see cref="OnUnmonitoredObservablePropertyChangedMethodAttribute"/> class.
    /// </summary>
    /// <param name="propertyPaths">
    /// The property paths which the declaring class monitors for reference changes only and for which
    /// <c>OnUnmonitoredObservablePropertyChanged</c> will be called.
    /// </param>
    public OnUnmonitoredObservablePropertyChangedMethodAttribute( params string[] propertyPaths ) { }
}