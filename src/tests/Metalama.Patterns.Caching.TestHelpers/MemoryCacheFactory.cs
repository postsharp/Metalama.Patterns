// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends;
using Microsoft.Extensions.Caching.Memory;

namespace Metalama.Patterns.Caching.TestHelpers;

public static class MemoryCacheFactory
{
    public static MemoryCache CreateCache() => new( new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMilliseconds( 10 ) } );

    public static MemoryCachingBackend CreateBackend( IServiceProvider? serviceProvider, string debugName = "test" )
        => new( CreateCache(), new MemoryCachingBackendConfiguration() { ServiceProvider = serviceProvider } ) { DebugName = debugName };
}