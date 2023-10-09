﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.SafeToCallForDependencyAnalysis.UsingAttributeOnMethodOfTargetClass;

[NotifyPropertyChanged]
public class UsingAttributeOnMethodOfTargetClass
{
    public int X => this.Foo();

    [SafeToCallForDependencyAnalysis]
    private int Foo() => 42;
}