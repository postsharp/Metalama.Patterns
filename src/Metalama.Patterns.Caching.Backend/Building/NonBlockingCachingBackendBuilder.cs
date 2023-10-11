// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

public sealed class NonBlockingCachingBackendBuilder : DistributedCachingBackendBuilder
{
    private readonly DistributedCachingBackendBuilder _underlying;

    internal NonBlockingCachingBackendBuilder( DistributedCachingBackendBuilder underlying )
    {
        this._underlying = underlying;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        var underlyingBackend = this._underlying.CreateBackend( args );

        return underlyingBackend as NonBlockingCachingBackendEnhancer ?? new NonBlockingCachingBackendEnhancer( underlyingBackend );
    }
}