// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Metalama.Patterns.Caching;

public static class CachingServiceFactory
{
    public static IServiceCollection AddCaching(
        this IServiceCollection serviceCollection,
        Func<IServiceProvider, CachingBackend>? backendFactory = null,
        Func<IServiceProvider, IFormatterRepository, CacheKeyBuilder>? keyBuilderFactory = null )
    {
        serviceCollection.Add(
            ServiceDescriptor.Singleton<ICachingService, CachingService>(
                serviceProvider =>
                {
                    var backend = backendFactory?.Invoke( serviceProvider ) ?? new MemoryCachingBackend();

                    return new CachingService(
                        backend,
                        keyBuilderFactory: keyBuilderFactory == null ? null : formatters => keyBuilderFactory( serviceProvider, formatters ),
                        serviceProvider: serviceProvider );
                } ) );

        return serviceCollection;
    }
}