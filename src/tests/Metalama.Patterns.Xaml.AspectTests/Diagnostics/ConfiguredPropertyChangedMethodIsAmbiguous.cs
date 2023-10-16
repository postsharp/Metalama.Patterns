// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Diagnostics.ConfiguredPropertyChangedMethodIsAmbiguous;

public class ConfiguredPropertyChangedMethodIsAmbiguous : DependencyObject
{
    [DependencyProperty( PropertyChangedMethod = nameof(Changed) )]
    public int Foo { get; set; }

    private void Changed() { }

    private void Changed( int value ) { }
}