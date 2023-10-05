// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.Include;
using Metalama.Patterns.NotifyPropertyChanged.Options;

// @Include(Include/DummyFactoryAndBuilder.cs)

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.ConfigureFactoryByFabric;

public sealed class Fabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureNotifyPropertyChanged( b => b.ImplementationStrategyFactory = new DummyFactory() );
    }
}

// <target>
[NotifyPropertyChanged]
public class ConfigureFactoryByFabric { }