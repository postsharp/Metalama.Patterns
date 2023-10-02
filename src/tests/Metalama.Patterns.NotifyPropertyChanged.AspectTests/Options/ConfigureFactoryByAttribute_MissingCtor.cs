// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.ConfigureFactoryByAttribute_MissingCtor;

public class FactoryWithMissingCtor : IImplementationStrategyFactory
{
    private FactoryWithMissingCtor() { }

    public IImplementationStrategyBuilder GetBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        throw new System.NotImplementedException();
    }
}

[NotifyPropertyChanged( ImplementationStrategyFactoryType = typeof( FactoryWithMissingCtor ) )]
public class ConfigureFactoryByAttribute_MissingCtor
{
}