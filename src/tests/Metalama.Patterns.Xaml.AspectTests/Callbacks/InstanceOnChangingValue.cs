// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue;

public partial class InstanceOnChangingValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging( int value )
    {
    }

    [DependencyProperty]
    public List<int> AcceptAssignable { get; set; }

    private void OnAcceptAssignableChanging( IEnumerable<int> value )
    {
    }

    [DependencyProperty]
    public int AcceptGeneric { get; set; }

    private void OnAcceptGenericChanging<T>( T value )
    {
    }

    [DependencyProperty]
    public int AcceptObject { get; set; }

    private void OnAcceptObjectChanging( object value )
    {
    }

}