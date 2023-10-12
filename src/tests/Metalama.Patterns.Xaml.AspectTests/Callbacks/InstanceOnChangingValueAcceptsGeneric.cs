// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric;

public partial class InstanceOnChangingValueAcceptsGeneric : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging<T>( T value )
    {
    }
}