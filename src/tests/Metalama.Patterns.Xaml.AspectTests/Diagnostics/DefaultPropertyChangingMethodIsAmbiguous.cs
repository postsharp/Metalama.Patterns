// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Diagnostics.DefaultPropertyChangingMethodIsAmbiguous;

public class DefaultPropertyChangingMethodIsAmbiguous : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging() { }

    private void OnFooChanging( int value ) { }
}