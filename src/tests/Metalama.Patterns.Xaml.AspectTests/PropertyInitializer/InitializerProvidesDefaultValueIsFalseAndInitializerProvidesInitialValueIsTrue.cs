// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue;

public class InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue : DependencyObject
{
    [DependencyProperty( InitializerProvidesDefaultValue = false, InitializerProvidesInitialValue = true )]
    public List<int> Foo { get; set; } = new( 3 ) { 1, 2, 3 };
}