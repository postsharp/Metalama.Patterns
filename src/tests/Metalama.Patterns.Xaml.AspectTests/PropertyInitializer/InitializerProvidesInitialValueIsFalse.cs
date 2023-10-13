﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse;

public class InitializerProvidesInitialValueIsFalse : DependencyObject
{
    [DependencyProperty( InitializerProvidesInitialValue = false )]
    public List<int> Foo { get; set; } = new List<int>( 3 ) { 1, 2, 3 };
}