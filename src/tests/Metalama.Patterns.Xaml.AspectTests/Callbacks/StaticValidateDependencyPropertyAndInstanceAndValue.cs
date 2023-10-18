// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue;

public partial class StaticValidateDependencyPropertyAndInstanceAndValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static bool ValidateFoo( DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, int value ) => true;

    [DependencyProperty]
    public List<int> AcceptsAssignableForValue { get; set; }

    private static bool ValidateAcceptsAssignableForValue(
        DependencyProperty d,
        StaticValidateDependencyPropertyAndInstanceAndValue instance,
        IEnumerable<int> value )
        => true;

    [DependencyProperty]
    public int AcceptsGenericForValue { get; set; }

    private static bool ValidateAcceptsGenericForValue<T>( DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, T value )
        => true;

    [DependencyProperty]
    public int AcceptsObjectForValue { get; set; }

    private static bool ValidateAcceptsObjectForValue( DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, object value )
        => true;

    [DependencyProperty]
    public int AcceptsDependencyObjectForInstance { get; set; }

    private static bool ValidateAcceptsDependencyObjectForInstance( DependencyProperty d, DependencyObject instance, int value ) => true;

    [DependencyProperty]
    public int AcceptsObjectForInstance { get; set; }

    private static bool ValidateAcceptsObjectForInstance( DependencyProperty d, object instance, int value ) => true;
}