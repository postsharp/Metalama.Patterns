// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

internal interface ICacheAspect
{
    CacheItemConfiguration BuildTimeConfiguration { get; }

    CacheItemConfiguration RunTimeConfiguration { get; }
}