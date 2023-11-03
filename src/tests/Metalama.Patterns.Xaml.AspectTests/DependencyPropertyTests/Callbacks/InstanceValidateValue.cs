// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceValidateValue;

public partial class InstanceValidateValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private bool ValidateFoo( int value ) => true;

    [DependencyProperty]
    public List<int> AcceptAssignable { get; set; }

    private bool ValidateAcceptAssignable( IEnumerable<int> value ) => true;

    [DependencyProperty]
    public int AcceptGeneric { get; set; }

    private bool ValidateAcceptGeneric<T>( T value ) => true;

    [DependencyProperty]
    public int AcceptObject { get; set; }

    private bool ValidateAcceptObject( object value ) => true;
}