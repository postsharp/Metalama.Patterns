// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

public sealed class UninitializedCachingBackendBuilder : BuiltCachingBackendBuilder
{
    public override CachingBackend CreateBackend( CreateBackendArgs args ) => new UninitializedCachingBackend();

    internal UninitializedCachingBackendBuilder() { }
}