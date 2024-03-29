﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Diagnostics.DependencyPropertyInitializerWillNotBeUsed;

public class DependencyPropertyInitializerWillNotBeUsed : DependencyObject
{
    [DependencyProperty( InitializerProvidesDefaultValue = false, InitializerProvidesInitialValue = false )]
    public List<int> Foo { get; set; } = new( 3 ) { 1, 2, 3 };
}