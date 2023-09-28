// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;
using System.ComponentModel;

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
        // TODO: Special case for design time. Consider a factory of IImplementationStrategyBuilder.

        IImplementationStrategyBuilder strategyBuilder = new ClassicImplementationStrategyBuilder( builder );

        strategyBuilder.BuildAspect();

        (strategyBuilder as IDisposable)?.Dispose();
    }

    // TODO: Remove workaround to #33870
    // Remove this member, use from Classic.Templates instead.
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;
}