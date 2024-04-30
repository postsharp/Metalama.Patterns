// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability;

[AttributeUsage( AttributeTargets.Class )]
[Inheritable]
public sealed class ObservableAttribute : Attribute, IAspect<INamedType>
{
    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var options = builder.Target.Enhancements().GetOptions<ObservabilityOptions>();
        var strategyBuilder = options.ImplementationStrategyFactory!.GetBuilder( builder );

        strategyBuilder.BuildAspect();
    }
}