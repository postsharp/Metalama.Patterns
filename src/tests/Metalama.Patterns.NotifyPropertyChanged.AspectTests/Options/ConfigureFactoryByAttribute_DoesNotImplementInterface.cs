// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.ConfigureFactoryByAttribute_DoesNotImplementInterface;

[CompileTime]
public class FactoryDoesNotImplementInterface { }

[NotifyPropertyChanged( ImplementationStrategyFactoryType = typeof( FactoryDoesNotImplementInterface ) )]
public class ConfigureFactoryByAttribute_DoesNotImplementInterface
{
}