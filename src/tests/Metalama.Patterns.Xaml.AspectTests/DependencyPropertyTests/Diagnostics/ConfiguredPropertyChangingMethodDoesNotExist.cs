﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Diagnostics.ConfiguredPropertyChangingMethodDoesNotExist;

public class ConfiguredPropertyChangingMethodDoesNotExist : DependencyObject
{
    [DependencyProperty( PropertyChangingMethod = "DoesNotExist" )]
    public int Foo { get; set; }
}