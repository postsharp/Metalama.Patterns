// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Formatters;
using Metalama.Patterns.Caching.ValueAdapters;

namespace Metalama.Patterns.Caching.Building;

public interface ICachingServiceBuilder
{
    IServiceProvider ServiceProvider { get; }

    ICachingServiceBuilder AddProfile( CachingProfile profile, bool overwrite = false );

    /// <summary>
    /// Registers an <see cref="IValueAdapter"/> instance and explicitly specifies the value type.
    /// </summary>
    /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
    /// <param name="valueAdapter">The adapter.</param>
    ICachingServiceBuilder AddValueAdapter( Type valueType, IValueAdapter valueAdapter );

    /// <summary>
    /// Registers a generic value adapter.
    /// </summary>
    /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
    /// <param name="valueAdapterType">The type of the value adapter. This type must implement the <see cref="IValueAdapter"/>
    /// interface and have the same number of generic parameters as <paramref name="valueType"/>.
    /// </param>
    ICachingServiceBuilder AddValueAdapter( Type valueType, Type valueAdapterType );

    /// <summary>
    /// Registers an <see cref="IValueAdapter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the cached value (typically the return type of the cached method).</typeparam>
    /// <param name="valueAdapter">The adapter.</param>
    ICachingServiceBuilder AddValueAdapter<T>( IValueAdapter<T> valueAdapter );

    ICachingServiceBuilder WithBackend( CachingBackend backend );

    ICachingServiceBuilder WithBackend( Func<CachingBackendBuilder, ConcreteCachingBackendBuilder> action );

    ICachingServiceBuilder ConfigureFormatters( Action<FormatterRepository.Builder> action );

    ICachingServiceBuilder WithKeyBuilder( Func<IFormatterRepository, CacheKeyBuilderOptions, ICacheKeyBuilder> factory );

    ICachingServiceBuilder WithKeyBuilderOptions( CacheKeyBuilderOptions options );
}