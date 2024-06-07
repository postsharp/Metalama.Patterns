﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Observability.AspectTests;

[Observable]
public sealed class SealedNonInpcAutoProperty
{
    public int X { get; set; }
}