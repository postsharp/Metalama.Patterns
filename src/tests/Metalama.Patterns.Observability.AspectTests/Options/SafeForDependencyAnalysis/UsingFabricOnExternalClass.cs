﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability.AspectTests.Options.SafeForDependencyAnalysis.UsingFabricOnExternalClass;

public sealed class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        var t = (INamedType) TypeFactory.GetType( typeof(ExternalClass) );
        amender.Outbound.Select( c => t.ForCompilation( c ) ).ConfigureDependencyAnalysis( b => b.IsSafeToCall = true );
    }
}

// <target>
[Observable]
public class UsingFabricOnExternalClass
{
    public int Count => ExternalClass.Foo();
}