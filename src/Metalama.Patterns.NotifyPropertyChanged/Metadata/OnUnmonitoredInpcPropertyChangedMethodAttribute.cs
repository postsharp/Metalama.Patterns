// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

/// <summary>
/// Indicates compliance with the "OnUnmonitoredInpcPropertyChanged" contract expected by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// This attribute should only be applied automatically by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
public sealed class OnUnmonitoredInpcPropertyChangedMethodAttribute : MetadataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnUnmonitoredInpcPropertyChangedMethodAttribute"/> class.
    /// </summary>
    /// <param name="propertyPaths">
    /// The property paths which the declaring class monitors for reference changes only and for which
    /// <c>OnUnmonitoredInpcPropertyChanged</c> will be called.
    /// </param>
    public OnUnmonitoredInpcPropertyChangedMethodAttribute( params string[] propertyPaths ) { }
}