﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DesignTimeStrategy;

/// <summary>
/// Introduces the minimum set of observable members, without bodies, to suit the Metalama design-time execution scenario.
/// </summary>
[CompileTime]
internal class DesignTimeImplementationStrategyBuilder : IImplementationStrategyBuilder, ITemplateProvider
{
    protected IAspectBuilder<INamedType> Builder { get; }
    
    public DesignTimeImplementationStrategyBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        this.Builder = aspectBuilder;
    }

    protected virtual void BuildAspect()
    {
        // Introduce the INotifyPropertyChanged if it's not already implemented.
        // TODO: Remove workaround to #33870
        this.Builder.Advice/*.WithTemplateProvider( this )*/.ImplementInterface( this.Builder.Target, typeof( INotifyPropertyChanged ), OverrideStrategy.Ignore );
    }

    void IImplementationStrategyBuilder.BuildAspect()
    {
        this.BuildAspect();
    }

    // TODO: Remove workaround to #33870
    // ReSharper disable once EventNeverSubscribedTo.Global
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;
}