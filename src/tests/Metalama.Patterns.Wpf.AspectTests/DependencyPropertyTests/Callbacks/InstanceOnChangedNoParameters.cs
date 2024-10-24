﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Wpf.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangedNoParameters;

public partial class InstanceOnChangedNoParameters : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanged() { }
}