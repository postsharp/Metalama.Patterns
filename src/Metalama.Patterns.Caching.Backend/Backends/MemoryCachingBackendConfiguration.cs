// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends;

public sealed record MemoryCachingBackendConfiguration : CachingBackendConfiguration
{
    public Func<CacheItem, long>? SizeCalculator { get; init; }
}