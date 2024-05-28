// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation;

/// <summary>
/// Introduces the minimum set of observable members, without bodies, to suit the Metalama design-time execution scenario.
/// </summary>
[CompileTime]
internal class DesignTimeObservabilityStrategy : IObservabilityStrategy, ITemplateProvider
{
    public virtual void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        // Introduce the INotifyPropertyChanged if it's not already implemented.
        builder.ImplementInterface( typeof( INotifyPropertyChanged ), OverrideStrategy.Ignore );
        builder.Advice.WithTemplateProvider( this ).IntroduceEvent( builder.Target, nameof(this.PropertyChanged), whenExists: OverrideStrategy.Ignore );
    }

    // ReSharper disable once EventNeverSubscribedTo.Global
    [Template]
#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
}