// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable;

public partial class InstanceOnChangedOldValueAndNewValueAcceptsAssignable : DependencyObject
{
    [DependencyProperty]
    public List<int> Foo { get; set; }

    private void OnFooChanged( IEnumerable<int> oldValue, IEnumerable<int> newValue )
    {
    }
}