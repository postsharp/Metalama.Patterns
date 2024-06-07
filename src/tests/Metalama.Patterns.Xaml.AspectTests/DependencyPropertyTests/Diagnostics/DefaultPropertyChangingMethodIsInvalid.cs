// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Diagnostics.DefaultPropertyChangingMethodIsInvalid;

public class DefaultPropertyChangingMethodIsInvalid : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging( DBNull blah ) { }
}