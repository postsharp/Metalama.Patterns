﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject;

public partial class InstanceOnChangingValueAcceptsObject : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging( object value )
    {
    }
}