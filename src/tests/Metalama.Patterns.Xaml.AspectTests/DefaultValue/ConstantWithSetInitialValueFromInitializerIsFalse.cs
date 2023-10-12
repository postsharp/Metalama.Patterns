// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse;

public class ConstantWithSetInitialValueFromInitializerIsFalse : DependencyObject
{
    [DependencyProperty( SetInitialValueFromInitializer = false )]
    public int Foo { get; set; } = 42;
}