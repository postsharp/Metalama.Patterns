// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingInstance;

public partial class StaticOnChangingInstance : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static void OnFooChanging( StaticOnChangingInstance instance ) { }

    [DependencyProperty]
    public int AcceptsDependencyObject { get; set; }

    private static void OnAcceptsDependencyObjectChanging( DependencyObject instance ) { }

    [DependencyProperty]
    public int AcceptsObject { get; set; }

    private static void OnAcceptsObjectChanging( object instance ) { }
}