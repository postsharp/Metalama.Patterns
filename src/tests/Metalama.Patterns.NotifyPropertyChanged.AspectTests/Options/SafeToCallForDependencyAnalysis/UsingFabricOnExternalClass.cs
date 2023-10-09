// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.SafeToCallForDependencyAnalysis.UsingFabricOnExternalClass;

public sealed class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Outbound.SelectMany( c => c.Types.OfName( nameof(ExternalClass) ) ).ConfigureDependencyAnalysis( b => b.IsSafeToCall = true );
    }
}

// <target>
[NotifyPropertyChanged]
public class UsingFabricOnExternalClass
{
    public int Count => ExternalClass.Foo();
}