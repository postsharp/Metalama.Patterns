﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Observability.AspectTests.Diagnostics.IgnoreWarnings;

// @RemoveOutputCode

[Observable]
public class AccessProtectedFieldOfTargetClass
{
    protected int _x;

    [SuppressObservabilityWarnings]
    public int X => this._x;
}