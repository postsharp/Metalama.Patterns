// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

/// <summary>
/// Identifies a generated "On{name}Changed" method. Do not apply this attribute. It must 
/// only be applied automatically by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
public sealed class OnChangedAttribute : MetadataAttribute
{
    public OnChangedAttribute( string name ) { }
}