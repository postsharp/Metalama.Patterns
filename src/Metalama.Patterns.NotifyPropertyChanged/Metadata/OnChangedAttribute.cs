// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

// TODO: Remove if unused.

/// <summary>
/// Identifies a generated "On{name}Changed" method. Do not apply this attribute. It must 
/// only be applied automatically by the <see cref="NotifyPropertyChangedAttribute"/> aspect.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
[Obsolete( "Currently not used.", true )]
public sealed class OnChangedAttribute : MetadataAttribute
{
    public OnChangedAttribute( string name ) { }
}