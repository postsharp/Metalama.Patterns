// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

[AttributeUsage( AttributeTargets.Method )]
public sealed class PropertyPathsAttribute : MetadataAttribute
{
    public PropertyPathsAttribute( params string[] paths )
    { }
}