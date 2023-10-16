// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Diagnostics.ConfiguredPropertyChangingMethodIsInvalid;

public class ConfiguredPropertyChangingMethodIsInvalid : DependencyObject
{
    [DependencyProperty( PropertyChangingMethod = nameof(Changing) )]
    public int Foo { get; set; }

    private void Changing( DBNull blah ) { }
}