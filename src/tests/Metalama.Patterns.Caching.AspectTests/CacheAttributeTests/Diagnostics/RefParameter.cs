// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.Diagnostics;

public class RefParameter
{
    [Cache]
    public int Test( ref int a ) => 42;
}