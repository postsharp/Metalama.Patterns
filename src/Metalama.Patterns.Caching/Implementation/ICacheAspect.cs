// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] Pending port of CacheAspect
// ReSharper disable UnusedMember.Global
internal interface ICacheAspect
{
    CacheItemConfiguration BuildTimeConfiguration { get; }

    CacheItemConfiguration RunTimeConfiguration { get; }
}