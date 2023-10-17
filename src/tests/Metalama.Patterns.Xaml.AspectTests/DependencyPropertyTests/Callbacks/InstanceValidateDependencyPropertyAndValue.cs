// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceValidateDependencyPropertyAndValue;

public partial class InstanceValidateDependencyPropertyAndValue : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private bool ValidateFoo( DependencyProperty d, int value ) => true;

    [DependencyProperty]
    public List<int> AcceptAssignable { get; set; }

    private bool ValidateAcceptAssignable( DependencyProperty d, IEnumerable<int> value ) => true;

    [DependencyProperty]
    public int AcceptGeneric { get; set; }

    private bool ValidateAcceptGeneric<T>( DependencyProperty d, T value ) => true;

    [DependencyProperty]
    public int AcceptObject { get; set; }

    private bool ValidateAcceptObject( DependencyProperty d, object value ) => true;
}