// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingNoParameters;

public partial class StaticOnChangingNoParameters : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static void OnFooChanging() { }
}