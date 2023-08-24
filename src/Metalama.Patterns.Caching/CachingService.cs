// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Caching;

public sealed class CachingService : IDisposable, IAsyncDisposable
{
    private volatile CacheKeyBuilder _keyBuilder;
    private volatile CachingBackend _backend = new UninitializedCachingBackend();

    public FormatterRepository Formatters { get; } = new CachingFormatterRepository();

    internal AutoReloadManager AutoReloadManager { get; }

    internal CachingFrontend Frontend { get; }

    public CachingService()
    {
        this.Invalidation = new CacheInvalidationService( this );
        this._keyBuilder = new CacheKeyBuilder( this.Formatters );
        this.Profiles = new CachingProfileRegistry( this );
        this.Lookup = new CacheLookupService( this );
        this.Frontend = new CachingFrontend( this );
        this.AutoReloadManager = new AutoReloadManager( this );
    }

    /// <summary>
    /// Gets the set of distinct backends used in the service.
    /// </summary>
    public ImmutableHashSet<CachingBackend> AllBackends => this.Profiles.AllBackends;

    /// <summary>
    /// Gets or sets the <see cref="CacheKeyBuilder"/> used to generate caching keys, i.e. to serialize objects into a <see cref="string"/>.
    /// </summary>
    [AllowNull]
    public CacheKeyBuilder KeyBuilder
    {
        get => this._keyBuilder;
        set => this._keyBuilder = value ?? new CacheKeyBuilder( this.Formatters );
    }

    /// <summary>
    /// Gets or sets the default <see cref="CachingBackend"/>, i.e. the physical storage of cache items.
    /// </summary>
    [AllowNull]
    public CachingBackend DefaultBackend
    {
        get => this._backend;
        set
        {
            if ( this._backend == value )
            {
                return;
            }

            this._backend = value ?? new NullCachingBackend();
        }
    }

    /// <summary>
    /// Gets the repository of caching profiles (<see cref="CachingProfile"/>).
    /// </summary>
    public CachingProfileRegistry Profiles { get; }

    public CacheInvalidationService Invalidation { get; }

    public CacheLookupService Lookup { get; }

    public void Dispose()
    {
        this.AutoReloadManager.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await this.AutoReloadManager.DisposeAsync();
    }
}