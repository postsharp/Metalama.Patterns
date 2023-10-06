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
    public static CachingService Default { get; set; } = CreateUninitialized();

    internal IServiceProvider? ServiceProvider { get; }

    public FormatterRepository Formatters { get; } = new CachingFormatterRepository();

    internal AutoReloadManager AutoReloadManager { get; }

    internal CachingFrontend Frontend { get; }

    public ValueAdapterFactory ValueAdapters { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingService"/> class.
    /// </summary>
    /// <param name="backend">The default back-end. If null, a new <see cref="MemoryCachingBackend"/> is created.</param>
    /// <param name="serviceProvider">An optional <see cref="IServiceProvider"/>.</param>
    public CachingService(
        CachingBackend? backend = null,
        IEnumerable<CachingProfile>? profiles = null,
        Func<IFormatterRepository, CacheKeyBuilder>? keyBuilderFactory = null,
        IServiceProvider? serviceProvider = null )
    {
        var profilesDictionary = ImmutableDictionary.CreateBuilder<string, CachingProfile>( StringComparer.Ordinal );
        this.DefaultBackend = backend ?? new MemoryCachingBackend();

        if ( profiles != null )
        {
            foreach ( var profile in profiles )
            {
                profilesDictionary.Add( profile.Name, profile );
                profile.Initialize( this.DefaultBackend );
            }
        }

        if ( !profilesDictionary.ContainsKey( CachingProfile.DefaultName ) )
        {
            var profile = new CachingProfile();
            profilesDictionary.Add( CachingProfile.DefaultName, profile );
            profile.Initialize( this.DefaultBackend );
        }

        this.ServiceProvider = serviceProvider;
        this.KeyBuilder = new CacheKeyBuilder( this.Formatters );
        this.Profiles = new CachingProfileRegistry( profilesDictionary.ToImmutable() );
        this.Frontend = new CachingFrontend( this );
        this.AutoReloadManager = new AutoReloadManager( this );
        this.KeyBuilder = keyBuilderFactory?.Invoke( this.Formatters ) ?? new CacheKeyBuilder( this.Formatters );
        this._defaultLogger = serviceProvider.GetFlashtraceSource( this.GetType(), FlashtraceRoles.Caching );
    }

    internal static CachingService CreateUninitialized( IServiceProvider? serviceProvider = null )
        => new( new UninitializedCachingBackend(), serviceProvider: serviceProvider );

    /// <summary>
    /// Gets the set of distinct backends used in the service.
    /// </summary>
    public ImmutableHashSet<CachingBackend> AllBackends => this.Profiles.AllBackends;

    public void AddDependency( string key ) => CachingContext.Current.AddDependency( key );

    public void AddDependencies( IEnumerable<string> keys ) => CachingContext.Current.AddDependencies( keys );

    public IDisposable SuspendDependencyPropagation() => CachingContext.OpenSuspendedCacheContext();

    string ICachingService.GetDependencyKey( object o ) => this.KeyBuilder.BuildDependencyKey( o );

    /// <summary>
    /// Gets the <see cref="CacheKeyBuilder"/> used to generate caching keys, i.e. to serialize objects into a <see cref="string"/>.
    /// </summary>
    [AllowNull]
    public CacheKeyBuilder KeyBuilder { get; }

    /// <summary>
    /// Gets default <see cref="CachingBackend"/>, i.e. the physical storage of cache items.
    /// </summary>
    [AllowNull]
    public CachingBackend DefaultBackend { get; }

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