﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.
    InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue;

public class InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue : DependencyObject
{
    private static List<int> InitMethod() => new( 3 ) { 1, 2, 3 };

    [DependencyProperty]
    public List<int> Foo { get; set; } = InitMethod();
}