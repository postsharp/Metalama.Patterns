// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;

namespace Metalama.Patterns.Caching.Building;

public sealed class NonBlockingCachingBackendBuilder : BuiltCachingBackendBuilder
{
    private readonly BuiltCachingBackendBuilder _underlying;

    internal NonBlockingCachingBackendBuilder( BuiltCachingBackendBuilder underlying )
    {
        this._underlying = underlying;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args ) => new NonBlockingCachingBackendEnhancer( this._underlying.CreateBackend( args ) );
}