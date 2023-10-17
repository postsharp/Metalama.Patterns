// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation.DesignTimeStrategy;

/// <summary>
/// Introduces the minimum set of observable members, without bodies, to suit the Metalama design-time execution scenario.
/// </summary>
[CompileTime]
internal class DesignTimeImplementationStrategyBuilder : IImplementationStrategyBuilder, ITemplateProvider
{
    protected IAspectBuilder<INamedType> Builder { get; }

    protected DesignTimeImplementationStrategyBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        this.Builder = aspectBuilder;
    }

    protected virtual void BuildAspect()
    {
        // Introduce the INotifyPropertyChanged if it's not already implemented.
        this.Builder.Advice.WithTemplateProvider( this ).ImplementInterface( this.Builder.Target, typeof(INotifyPropertyChanged), OverrideStrategy.Ignore );
    }

    void IImplementationStrategyBuilder.BuildAspect()
    {
        this.BuildAspect();
    }

    // ReSharper disable once EventNeverSubscribedTo.Global
    [InterfaceMember]
#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067
}