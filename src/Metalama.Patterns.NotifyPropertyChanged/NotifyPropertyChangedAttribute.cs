// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

namespace Metalama.Patterns.NotifyPropertyChanged;

[AttributeUsage( AttributeTargets.Class )]
[Inheritable]
public sealed class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
{
    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // TODO: For future use, select the desired implementation strategy by configuration.

        builder.Outbound.AddAspect<NaturalAspect>();
    }
}