// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Formatters;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ValueAdapters;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Caching;

public sealed partial class CachingService : IDisposable, IAsyncDisposable, ICachingService
{
    public static CachingService Default { get; set; } = new();

    internal IServiceProvider? ServiceProvider { get; }

    private volatile CacheKeyBuilder _keyBuilder;
    private volatile CachingBackend _backend = new UninitializedCachingBackend();

    public FormatterRepository Formatters { get; } = new CachingFormatterRepository();

    internal AutoReloadManager AutoReloadManager { get; }

    internal CachingFrontend Frontend { get; }

    public ValueAdapterFactory ValueAdapters { get; } = new();

    public CachingService( IServiceProvider? serviceProvider = null )
    {
        this.ServiceProvider = serviceProvider;
        this._keyBuilder = new CacheKeyBuilder( this.Formatters );
        this.Profiles = new CachingProfileRegistry( this );
        this.Frontend = new CachingFrontend( this );
        this.AutoReloadManager = new AutoReloadManager( this );
        this._defaultLogger = serviceProvider.GetLogSource( this.GetType(), LoggingRoles.Caching );
    }

    /// <summary>
    /// Gets the set of distinct backends used in the service.
    /// </summary>
    public ImmutableHashSet<CachingBackend> AllBackends => this.Profiles.AllBackends;

    public void AddDependency( string key ) => CachingContext.Current.AddDependency( key );

    public void AddDependencies( IEnumerable<string> keys ) => CachingContext.Current.AddDependencies( keys );

    public IDisposable SuspendDependencyPropagation() => CachingContext.OpenSuspendedCacheContext();

    string ICachingService.GetDependencyKey( object o ) => this.KeyBuilder.BuildDependencyKey( o );

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
            this.Profiles.OnChange();
        }
    }

    /// <summary>
    /// Gets the repository of caching profiles (<see cref="CachingProfile"/>).
    /// </summary>
    public CachingProfileRegistry Profiles { get; }

    public void Dispose()
    {
        this.AutoReloadManager.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await this.AutoReloadManager.DisposeAsync();
    }
}