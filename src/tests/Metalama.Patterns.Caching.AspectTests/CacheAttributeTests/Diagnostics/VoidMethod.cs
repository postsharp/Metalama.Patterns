﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.Diagnostics;

public class VoidMethod
{
    [Cache]
    public void Test( in int a ) { }
}