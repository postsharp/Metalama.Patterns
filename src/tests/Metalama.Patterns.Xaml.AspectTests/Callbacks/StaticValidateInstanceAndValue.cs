// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue;

public partial class StaticValidateInstanceAndValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static bool ValidateFoo( StaticValidateInstanceAndValue instance, int value ) => true;

    [DependencyProperty]
    public List<int> AcceptsAssignableForValue { get; set; }

    private static bool ValidateAcceptsAssignableForValue( StaticValidateInstanceAndValue instance, IEnumerable<int> value ) => true;

    [DependencyProperty]
    public int AcceptsGenericForValue { get; set; }

    private static bool ValidateAcceptsGenericForValue<T>( StaticValidateInstanceAndValue instance, T value ) => true;

    [DependencyProperty]
    public int AcceptsObjectForValue { get; set; }

    private static bool ValidateAcceptsObjectForValue( StaticValidateInstanceAndValue instance, object value ) => true;

    [DependencyProperty]
    public int AcceptsDependencyObjectForInstance { get; set; }

    private static bool ValidateAcceptsDependencyObjectForInstance( DependencyObject instance, int value ) => true;

    [DependencyProperty]
    public int AcceptsObjectForInstance { get; set; }

    private static bool ValidateAcceptsObjectForInstance( object instance, int value ) => true;
}