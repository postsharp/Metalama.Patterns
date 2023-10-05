// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

public sealed class SuspendInvariantsAttribute : OverrideMethodAspect
{
    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        var contractOptions = builder.Target.DeclaringType.GetContractOptions();

        // Skip the aspect if invariants are not enabled.
        if ( contractOptions.AreInvariantsEnabled == false )
        {
            builder.SkipAspect();

            return;
        }

        // Report an error if invariant suspension is not supported for this type.
        var supportsInvariantSuspension = contractOptions.SupportsInvariantSuspension == true;

        if ( !supportsInvariantSuspension )
        {
            builder.Diagnostics.Report( ContractDiagnostics.SuspensionNotSupported.WithArguments( (builder.Target, builder.Target.DeclaringType) ) );

            return;
        }

        // Report a warning and skip if there is no invariant on this type.
        if ( !builder.Target.DeclaringType.Enhancements().HasAspect<CheckInvariantsAspect>() )
        {
            builder.Diagnostics.Report( ContractDiagnostics.SuspensionRedundant.WithArguments( (builder.Target, builder.Target.DeclaringType) ) );

            builder.SkipAspect();

            return;
        }

        base.BuildAspect( builder );
    }

    public override dynamic? OverrideMethod()
    {
        using ( meta.This.SuspendInvariants() )
        {
            return meta.Proceed();
        }
    }
}