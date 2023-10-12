﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject;

public partial class StaticOnChangedInstanceAcceptsObject : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static void OnFooChanged( object instance )
    {
    }
}