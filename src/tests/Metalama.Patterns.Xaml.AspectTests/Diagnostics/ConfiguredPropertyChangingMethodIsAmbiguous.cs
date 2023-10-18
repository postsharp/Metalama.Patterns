// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Diagnostics.ConfiguredPropertyChangingMethodIsAmbiguous;

public class ConfiguredPropertyChangingMethodIsAmbiguous : DependencyObject
{
    [DependencyProperty( PropertyChangedMethod = nameof(Changing) )]
    public int Foo { get; set; }

    private void Changing() { }

    private void Changing( int value ) { }
}