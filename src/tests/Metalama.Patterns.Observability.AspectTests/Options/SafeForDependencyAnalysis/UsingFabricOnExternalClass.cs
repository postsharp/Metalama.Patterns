// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability.AspectTests.Options.IgnoreUnsupportedDependencies.UsingFabricOnExternalClass;

public sealed class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender
            .SelectReflectionType( typeof(ExternalClass) )
            .ConfigureObservability( b => b.IgnoreUnsupportedDependencies = true );
    }
}

// <target>
[Observable]
public class UsingFabricOnExternalClass
{
    public int Count => ExternalClass.Foo();
}