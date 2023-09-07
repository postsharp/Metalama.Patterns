// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

// TODO: Cleanup. Remove if unused.

[Obsolete("Probably not needed.", true)]
[AttributeUsage( AttributeTargets.Method )]
public sealed class UpdateAttribute : MetadataAttribute
{
    public UpdateAttribute( string name ) { }
}