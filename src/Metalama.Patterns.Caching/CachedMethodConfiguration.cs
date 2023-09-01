// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Caching;

[RunTimeOrCompileTime]
public record CachedMethodConfiguration : CacheItemConfiguration
{
    public static CachedMethodConfiguration Empty { get; } = new();

    public bool? IgnoreThisParameter { get; init; }
}