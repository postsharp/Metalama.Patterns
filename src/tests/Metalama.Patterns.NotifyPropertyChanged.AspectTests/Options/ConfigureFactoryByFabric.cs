// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.ConfigureFactoryByFabric;

public class Factory : IImplementationStrategyFactory
{
    public IImplementationStrategyBuilder GetBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        return new Builder( aspectBuilder );
    }
}

public class Builder : IImplementationStrategyBuilder, ITemplateProvider
{
    private readonly IAspectBuilder<INamedType> _aspectBuilder;

    public Builder( IAspectBuilder<INamedType> aspectBuilder )
    {
        this._aspectBuilder = aspectBuilder;
    }

    public void BuildAspect()
    {
        this._aspectBuilder.Advice.WithTemplateProvider( this ).IntroduceMethod( this._aspectBuilder.Target, nameof( this.Hello ) );
    }

    [Template]
    public void Hello()
    {
    }
}

public sealed class Fabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureNotifyPropertyChanged( b => b.ImplementationStrategyFactory = new Factory() );
    }
}

// <target>
[NotifyPropertyChanged]
public class ConfigureFactoryByFabric
{
}