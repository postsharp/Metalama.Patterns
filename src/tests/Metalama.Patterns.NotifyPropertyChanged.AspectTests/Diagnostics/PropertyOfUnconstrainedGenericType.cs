﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics;

[NotifyPropertyChanged]
public class PropertyOfUnconstrainedGenericType<T>
{
    public T C1 { get; set; }
}