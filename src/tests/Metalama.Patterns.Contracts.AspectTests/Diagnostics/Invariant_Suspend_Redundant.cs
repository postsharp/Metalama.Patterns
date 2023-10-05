// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;

namespace Metalama.Patterns.Contracts.AspectTests.Diagnostics.Invariant_Suspend_Redundant;

public class BaseClass
{
    public int A { get; set; }

    public int B { get; set; }

    [SuspendInvariants]
    public void ExecuteWithoutInvariants()
    {
        this.A = -5;
        this.B = 5;
    }
}

public class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender )
    {
        amender.Outbound.SetOptions( new ContractOptions { SupportsInvariantSuspension = true } );
    }
}