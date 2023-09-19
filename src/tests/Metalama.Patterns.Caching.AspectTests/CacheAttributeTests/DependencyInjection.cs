// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.DependencyInjection;

public class C
{
    
    [Cache]
    public int M() => 5;
    
}