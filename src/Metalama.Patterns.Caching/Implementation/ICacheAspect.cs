// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace Metalama.Patterns.Caching.Implementation
{
    internal interface ICacheAspect
    {
        CacheItemConfiguration BuildTimeConfiguration { get; }

        CacheItemConfiguration RunTimeConfiguration { get; }

    }
}
