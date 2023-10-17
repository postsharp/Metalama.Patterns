// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticValidateDependencyPropertyAndValue;

public partial class StaticValidateDependencyPropertyAndValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static bool ValidateFoo( DependencyProperty d, int value ) => true;

    [DependencyProperty]
    public List<int> AcceptsAssignable { get; set; }

    private static bool ValidateAcceptsAssignable( DependencyProperty d, IEnumerable<int> value ) => true;

    [DependencyProperty]
    public int AcceptsGeneric { get; set; }

    private static bool ValidateAcceptsGeneric<T>( DependencyProperty d, T value ) => true;

    [DependencyProperty]
    public int AcceptsObject { get; set; }

    private static bool ValidateAcceptsObject( DependencyProperty d, object value ) => true;
}