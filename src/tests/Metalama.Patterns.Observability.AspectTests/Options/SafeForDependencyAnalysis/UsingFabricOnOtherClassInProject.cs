// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability.AspectTests.Options.SafeForDependencyAnalysis.UsingFabricOnOtherClassInProject;

public sealed class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.SelectMany( c => c.Types.OfName( nameof(OtherClass) ) ).ConfigureDependencyAnalysis( b => b.IsSafeToCall = true );
    }
}

// ReSharper disable once MemberCanBeInternal
public static class OtherClass
{
    public static int Foo() => 42;
}

// <target>
[Observable]
public class UsingFabricOnOtherClassInProject
{
    public int X => OtherClass.Foo();
}