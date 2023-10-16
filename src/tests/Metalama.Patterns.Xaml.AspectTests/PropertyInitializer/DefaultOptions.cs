// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions;

public class DefaultOptions : DependencyObject
{
    [DependencyProperty]
    public List<int> Foo { get; set; } = new( 3 ) { 1, 2, 3 };
}