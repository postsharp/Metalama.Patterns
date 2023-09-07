// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.Metadata;

// TODO: Cleanup before release.
// TODO: This would be good for robustness wrt obfuscation, but that is outside current requirements.

[Obsolete("Not required according to current requirements.", true)]
[AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
public sealed class OriginAttribute : MetadataAttribute
{
    public OriginAttribute( string name ) { }
}