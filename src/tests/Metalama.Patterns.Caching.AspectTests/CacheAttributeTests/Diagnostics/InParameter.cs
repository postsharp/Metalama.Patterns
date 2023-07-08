// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.Diagnostics;

public class InParameter
{
    [Cache]
    public int Test( in int a ) => 42;
}