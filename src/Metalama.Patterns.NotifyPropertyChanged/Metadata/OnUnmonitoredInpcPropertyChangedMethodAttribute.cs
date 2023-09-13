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
    public OnUnmonitoredInpcPropertyChangedMethodAttribute( params string[] propertyPaths ) { }
}