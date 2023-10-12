// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric;

public partial class InstanceOnChangedOldValueAndNewValueAcceptsGeneric : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanged<T>( T oldValue, T newValue )
    {
    }
}