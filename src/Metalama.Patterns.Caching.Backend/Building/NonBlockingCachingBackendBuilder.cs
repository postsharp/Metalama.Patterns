// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

/// <summary>
/// A <see cref="CachingBackendBuilder"/> that modifies the underlying <see cref="OutOfProcessCachingBackendBuilder"/> by making
/// all write and invalidate operations non-blocking, i.e. these methods return before completion of the operation.
/// </summary>
public sealed class NonBlockingCachingBackendBuilder : OutOfProcessCachingBackendBuilder
{
    private readonly OutOfProcessCachingBackendBuilder _underlying;

    internal NonBlockingCachingBackendBuilder( OutOfProcessCachingBackendBuilder underlying )
    {
        this._underlying = underlying;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        var underlyingBackend = this._underlying.CreateBackend( args );

        return underlyingBackend as NonBlockingCachingBackendEnhancer ?? new NonBlockingCachingBackendEnhancer( underlyingBackend );
    }
}