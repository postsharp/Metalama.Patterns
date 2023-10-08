// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

public abstract class BuiltCachingBackendBuilder : BaseCachingBackendBuilder
{
    public abstract CachingBackend CreateBackend( CreateBackendArgs args );
}

public sealed record CreateBackendArgs
{
    public IServiceProvider? ServiceProvider { get; internal init; }

    public int Layer { get; internal init; }
}