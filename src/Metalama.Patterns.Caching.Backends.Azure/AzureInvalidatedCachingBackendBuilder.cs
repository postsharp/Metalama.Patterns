// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Building;

namespace Metalama.Patterns.Caching.Backends.Azure;

public sealed class AzureInvalidatedCachingBackendBuilder : BuiltCachingBackendBuilder
{
    private readonly MemoryCachingBackendBuilder _underlying;
    private readonly AzureCacheInvalidatorConfiguration _configuration;

    internal AzureInvalidatedCachingBackendBuilder( MemoryCachingBackendBuilder underlying, AzureCacheInvalidatorConfiguration configuration )
    {
        this._underlying = underlying;
        this._configuration = configuration;
    }

    public override CachingBackend CreateBackend( CreateBackendArgs args )
    {
        var underlying = this._underlying.CreateBackend( args );

        return new AzureCacheInvalidator( underlying, this._configuration );
    }
}