﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Observability.AspectTests.CallMissingMethod;

// @RemoveOutputCode

[Observable]
public class CallMissingMethod
{
#if METALAMA
    public int X => Foo();
#endif
}