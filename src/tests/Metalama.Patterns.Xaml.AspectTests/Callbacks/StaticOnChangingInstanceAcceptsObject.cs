﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject;

public partial class StaticOnChangingInstanceAcceptsObject : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private static void OnFooChanging( object instance )
    {
    }
}