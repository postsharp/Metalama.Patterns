// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

/// <summary>
/// Indicates compliance with the "OnChildPropertyChanged" contract expected by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// This attribute should only be applied automatically by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
public sealed class OnChildPropertyChangedMethodAttribute : MetadataAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnChildPropertyChangedMethodAttribute"/> class.
    /// </summary>
    /// <param name="parentPropertyPaths">
    /// The parent property paths which the declaring class monitors for reference and child property changes,
    /// and for which <c>OnChildPropertyChanged</c> will be called.
    /// </param>
    public OnChildPropertyChangedMethodAttribute( params string[] parentPropertyPaths ) { }
}