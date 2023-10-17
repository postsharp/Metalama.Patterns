// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticValidateValue;

public partial class StaticValidateValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static bool ValidateFoo( int value ) => true;

    [DependencyProperty]
    public List<int> AcceptsAssignable { get; set; }

    private static bool ValidateAcceptsAssignable( IEnumerable<int> value ) => true;

    [DependencyProperty]
    public int AcceptsGeneric { get; set; }

    private static bool ValidateAcceptsGeneric<T>( T value ) => true;

    [DependencyProperty]
    public int AcceptsObject { get; set; }

    private static bool ValidateAcceptsObject( object value ) => true;
}