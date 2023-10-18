// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance;

public partial class StaticOnChangingDependencyPropertyAndInstance : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static void OnFooChanging( DependencyProperty d, StaticOnChangingDependencyPropertyAndInstance instance ) { }

    [DependencyProperty]
    public int AcceptsDependencyObjectForInstance { get; set; }

    private static void OnAcceptsDependencyObjectForInstanceChanging( DependencyProperty d, DependencyObject instance ) { }

    [DependencyProperty]
    public int AcceptsObjectForInstance { get; set; }

    private static void OnAcceptsObjectForInstanceChanging( DependencyProperty d, object instance ) { }
}