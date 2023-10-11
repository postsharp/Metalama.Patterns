// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Building;

namespace Metalama.Patterns.Caching.Backends.Azure;

[PublicAPI]
public static class AzureCachingBackendFactory
{
    public static AzureInvalidatedCachingBackendBuilder WithAzureInvalidation(
        this MemoryCachingBackendBuilder builder,
        string connectionString )
        => builder.WithAzureInvalidation( new AzureCacheInvalidatorConfiguration( connectionString ) );

    public static AzureInvalidatedCachingBackendBuilder WithAzureInvalidation(
        this MemoryCachingBackendBuilder builder,
        AzureCacheInvalidatorConfiguration configuration )
        => new( builder, configuration );
}