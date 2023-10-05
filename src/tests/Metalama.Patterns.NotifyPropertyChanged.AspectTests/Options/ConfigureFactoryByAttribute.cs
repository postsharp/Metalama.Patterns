// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.Include;

// @Include(Include/DummyFactoryAndBuilder.cs)

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.ConfigureFactoryByAttribute;

// <target>
[NotifyPropertyChanged( ImplementationStrategyFactoryType = typeof(DummyFactory) )]
public class ConfigureFactoryByAttribute { }