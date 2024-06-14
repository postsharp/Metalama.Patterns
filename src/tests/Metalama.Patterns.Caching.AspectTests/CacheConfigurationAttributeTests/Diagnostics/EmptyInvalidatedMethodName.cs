﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if TEST_OPTIONS
// @RemoveOutputCode
#endif

namespace Metalama.Patterns.Caching.AspectTests.CacheConfigurationAttributeTests.Diagnostics;

public class EmptyInvalidatedMethodName
{
    [InvalidateCache( "" )]
    public int Test( int a ) => 42;
}