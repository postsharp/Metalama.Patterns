// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Diagnostics.ConfiguredPropertyChangedMethodDoesNotExist;

public class ConfiguredPropertyChangedMethodDoesNotExist : DependencyObject
{
    [DependencyProperty( PropertyChangedMethod = "DoesNotExist" )]
    public int Foo { get; set; }
}