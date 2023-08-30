﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.ValueAdapters;
using System.Globalization;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// An abstraction of the physical implementation of the cache, where the <see cref="CacheAttribute"/> ends up writing to and reading from.
/// </summary>
[PublicAPI]
public abstract class CachingBackend : ITestableCachingComponent
{
    public CachingBackendConfiguration Configuration { get; }

    private const int _disposeTimeout = 30000;

    private static readonly ValueTask<bool> _falseTaskResult = new( false );
    private static readonly ValueTask<bool> _trueTaskResult = new( true );
    private static readonly ValueTask _completedTask = new( Task.CompletedTask );

    private readonly TaskCompletionSource<bool> _disposeTask = new();

    private CachingBackendFeatures? _features;
    private int _status;
    private EventHandler<CacheItemRemovedEventArgs>? _itemRemoved;
    private EventHandler<CacheDependencyInvalidatedEventArgs>? _dependencyInvalidated;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBackend"/> class.
    /// </summary>
    protected CachingBackend( CachingBackendConfiguration? configuration )
    {
        this.Configuration = configuration ?? new MemoryCachingBackendConfiguration();
        this.LogSource = this.Configuration.ServiceProvider.GetLogSource( this.GetType(), LoggingRoles.Caching );
        this.DebugName = this.Id.ToString();
    }

    internal string DebugName { get; set; }

    /// <summary>
    /// Gets the <see cref="Flashtrace.LogSource"/> that implementations can use to emit
    /// log records.
    /// </summary>
    protected LogSource LogSource { get; }

    /// <summary>
    /// Gets the <see cref="Guid"/> of the current <see cref="CachingBackend"/>.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Gets the factory of value adapters (<see cref="IValueAdapter"/>), which allow for instance to cache things like <see cref="System.IO.Stream"/> or <see cref="System.Collections.IEnumerable"/>,
    /// which could not be otherwise cached.
    /// </summary>
    public ValueAdapterFactory ValueAdapters { get; } = new();

    /// <summary>
    /// Creates a <see cref="CachingBackendFeatures"/> object, which describes set of features implemented by the back-end.
    /// This method is invoked the first time the <see cref="SupportedFeatures"/> property is evaluated. The result is then cached.
    /// </summary>
    /// <returns>A new instance of the <see cref="CachingBackendEnhancerFeatures"/> class.</returns>
    protected virtual CachingBackendFeatures CreateFeatures() => new();

    /// <summary>
    /// Gets the set of features supported by the current <see cref="CachingBackend"/>.
    /// </summary>
    public CachingBackendFeatures SupportedFeatures => this._features ??= this.CreateFeatures();

    private void CheckStatus()
    {
        if ( this.Status != CachingBackendStatus.Default )
        {
            throw new ObjectDisposedException( this.ToString() );
        }
    }

    private void RequireFeature( bool feature, string featureName )
    {
        if ( !feature )
        {
            throw new NotSupportedException(
                string.Format( CultureInfo.InvariantCulture, "Feature \"{0}\" is not supported by {1}.", featureName, this.GetType().Name ) );
        }
    }

    /// <summary>
    /// Sets a cache item. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="item">The cache item.</param>
    protected abstract void SetItemCore( string key, CacheItem item );

    /// <summary>
    /// Asynchronously sets a cache item. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// The default implementation is to call the synchronous <see cref="SetItemCore(string, CacheItem)"/> method.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="item">The cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected virtual ValueTask SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        this.SetItemCore( key, item );

        return new ValueTask( Task.CompletedTask );
    }

    /// <summary>
    /// Sets a cache item.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="item">The cache item.</param>
    public void SetItem( string key, CacheItem item )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( """SetItem( backend = "{Backend}" key = "{Key}" )""", this, key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.Validate( item );

                this.SetItemCore( key, item );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously sets a cache item.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="item">The cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask SetItemAsync( string key, CacheItem item, CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( """SetItemAsync( backend = "{Backend}" key = "{Key}" )""", this, key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.Validate( item );

                cancellationToken.ThrowIfCancellationRequested();

                await this.SetItemAsyncCore( key, item, cancellationToken );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Determines whether the cache contains an item of a given key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <returns><c>true</c> if the cache contains an item whose key is <paramref name="key"/>, otherwise <c>false</c>.</returns>
    protected abstract bool ContainsItemCore( string key );

    /// <summary>
    /// Asynchronously determines whether the cache contains an item of a given key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that evaluates to <c>true</c> if the cache contains an item whose key is <paramref name="key"/>, otherwise <c>false</c>.</returns>
    protected virtual ValueTask<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return this.ContainsItemCore( key ) ? _trueTaskResult : _falseTaskResult;
    }

    /// <summary>
    /// Determines whether the cache contains an item of a given key.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <returns><c>true</c> if the cache contains an item whose key is <paramref name="key"/>, otherwise <c>false</c>.</returns>
    public bool ContainsItem( string key )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "ContainsItem( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                var present = this.ContainsItemCore( key );

                activity.SetResult( present ? "Cache hit." : "Cache miss." );

                return present;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously determines whether the cache contains an item of a given key.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that evaluates to <c>true</c> if the cache contains an item whose key is <paramref name="key"/>, otherwise <c>false</c>.</returns>
    public async ValueTask<bool> ContainsItemAsync( string key, CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( "ContainsItemAsync( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                cancellationToken.ThrowIfCancellationRequested();

                var present = await this.ContainsItemAsyncCore( key, cancellationToken );

                activity.SetResult( present ? "Cache hit." : "Cache miss." );

                return present;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Gets a cache item given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The cache item.</param>
    /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
    /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
    /// <returns>A <see cref="CacheValue"/>, or <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
    protected abstract CacheValue? GetItemCore( string key, bool includeDependencies );

    /// <summary>
    /// Asynchronously gets a cache item given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The cache item.</param>
    /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
    ///     resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> evaluating to a <see cref="CacheValue"/>, or evaluating to <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
    protected virtual ValueTask<CacheValue?> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return new ValueTask<CacheValue?>( this.GetItemCore( key, includeDependencies ) );
    }

    /// <summary>
    /// Gets a cache item given its key.
    /// </summary>
    /// <param name="key">The cache item.</param>
    /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
    /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
    /// <returns>A <see cref="CacheValue"/>, or <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
    public CacheValue? GetItem( string key, bool includeDependencies = true )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( """GetItem( backend = "{Backend}" key = "{Key}" )""", this, key ) ) )
        {
            try
            {
                this.CheckStatus();

                CacheValue? item = null;

                try
                {
                    item = this.GetItemCore( key, includeDependencies );
                }
                catch ( Exception e ) when ( e is InvalidCacheItemException or InvalidCastException )
                {
                    this.LogSource.Warning.Write(
                        Formatted(
                            "The cached object or method source code or caching settings may have changed since the value has been cached. Removing the item. "
                            +
                            "To avoid this warning, change the cache key prefix when you do breaking changes to the source code." ),
                        e );

                    this.RemoveItemCore( key );
                }

                activity.SetResult( item != null ? "Cache hit." : "Cache miss." );

                return item;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously gets a cache item given its key.
    /// </summary>
    /// <param name="key">The cache item.</param>
    /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
    /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> evaluating to a <see cref="CacheValue"/>, or evaluating to <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
    public async ValueTask<CacheValue?> GetItemAsync(
        string key,
        bool includeDependencies = true,
        CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( """GetItemAsync( backend = "{Backend}" key = "{Key}" )""", this, key ) ) )
        {
            try
            {
                this.CheckStatus();
                cancellationToken.ThrowIfCancellationRequested();

                CacheValue? item = null;

                try
                {
                    item = await this.GetItemAsyncCore( key, includeDependencies, cancellationToken );
                }
                catch ( Exception e ) when ( e is InvalidCacheItemException or InvalidCastException )
                {
                    this.LogSource.Info.Write(
                        Formatted(
                            "The cached object or method source code or caching settings may have changed since the value has been cached. Removing the item." ),
                        e );

                    await this.RemoveItemAsyncCore( key, cancellationToken );
                }

                activity.SetResult( item != null ? "Cache hit." : "Cache miss." );

                return item;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Removes a cache item from the cache given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    protected abstract void RemoveItemCore( string key );

    /// <summary>
    /// Asynchronously removes a cache item from the cache given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected virtual ValueTask RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        this.RemoveItemCore( key );

        return _completedTask;
    }

    /// <summary>
    /// Removes a cache item from the cache given its key.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    public void RemoveItem( string key )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "RemoveItem( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.RemoveItemCore( key );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously removes a cache item from the cache given its key.
    /// </summary>
    /// <param name="key">The key of the cache item.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask RemoveItemAsync( string key, CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( "RemoveItemAsync( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                cancellationToken.ThrowIfCancellationRequested();

                await this.RemoveItemAsyncCore( key, cancellationToken );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Removes from the cache all items that have a specific dependency.  This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    protected abstract void InvalidateDependencyCore( string key );

    /// <summary>
    /// Asynchronously removes from the cache all items that have a specific dependency.  This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected virtual ValueTask InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        this.InvalidateDependencyCore( key );

        return _completedTask;
    }

    /// <summary>
    /// Removes from the cache all items that have a specific dependency.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    public void InvalidateDependency( string key )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "InvalidateDependency( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );

                this.InvalidateDependencyCore( key );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously removes from the cache all items that have a specific dependency.  This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask InvalidateDependencyAsync( string key, CancellationToken cancellationToken = default )
    {
        using ( var activity =
               this.LogSource.Default.OpenAsyncActivity( Formatted( "InvalidateDependencyAsync( Backend=\"{Backend}\", Key=\"{Key}\" )", this, key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );

                cancellationToken.ThrowIfCancellationRequested();

                await this.InvalidateDependencyAsyncCore( key, cancellationToken );

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Determines whether the cache contains a given dependency.  This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <returns><c>true</c> if the cache contains the dependency <paramref name="key"/>, otherwise <c>false</c>.</returns>
    protected abstract bool ContainsDependencyCore( string key );

    /// <summary>
    /// Asynchronously determines whether the cache contains a given dependency.  This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> evaluating to <c>true</c> if the cache contains the dependency <paramref name="key"/>, otherwise <c>false</c>.</returns>
    protected virtual ValueTask<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        return this.ContainsDependencyCore( key ) ? _trueTaskResult : _falseTaskResult;
    }

    /// <summary>
    /// Determines whether the cache contains a given dependency.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <returns><c>true</c> if the cache contains the dependency <paramref name="key"/>, otherwise <c>false</c>.</returns>
    public bool ContainsDependency( string key )
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "ContainsDependency( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();
                this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );
                this.RequireFeature( this.SupportedFeatures.ContainsDependency, nameof(this.SupportedFeatures.ContainsDependency) );

                var present = this.ContainsDependencyCore( key );

                activity.SetResult( present ? "Cache hit." : "Cache miss." );

                return present;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously determines whether the cache contains a given dependency.
    /// </summary>
    /// <param name="key">The dependency key.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> evaluating to <c>true</c> if the cache contains the dependency <paramref name="key"/>, otherwise <c>false</c>.</returns>
    public async ValueTask<bool> ContainsDependencyAsync( string key, CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( "ContainsDependencyAsync( \"{Key}\" )", key ) ) )
        {
            try
            {
                this.CheckStatus();

                this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );
                this.RequireFeature( this.SupportedFeatures.ContainsDependency, nameof(this.SupportedFeatures.ContainsDependency) );

                cancellationToken.ThrowIfCancellationRequested();

                var present = await this.ContainsDependencyAsyncCore( key, cancellationToken );

                activity.SetResult( present ? "Cache hit." : "Cache miss." );

                return present;
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Clears the cache. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. 
    /// </summary>
    protected abstract void ClearCore();

    /// <summary>
    /// Asynchronously clears the cache. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected virtual ValueTask ClearAsyncCore( CancellationToken cancellationToken )
    {
        this.ClearCore();

        return _completedTask;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public void Clear()
    {
        using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "Clear()" ) ) )
        {
            try
            {
                this.CheckStatus();
                this.ClearCore();

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Asynchronously clears the cache.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask ClearAsync( CancellationToken cancellationToken = default )
    {
        using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( "ClearAsync()" ) ) )
        {
            try
            {
                this.CheckStatus();
                cancellationToken.ThrowIfCancellationRequested();

                await this.ClearAsyncCore( cancellationToken );
            }
            catch ( Exception e )
            {
                activity.SetException( e );

                throw;
            }
        }
    }

    /// <summary>
    /// Gets the status of the current <see cref="CachingBackend"/> (<see cref="CachingBackendStatus.Default"/>,
    /// <see cref="CachingBackendStatus.Disposing"/> or <see cref="CachingBackendStatus.Disposed"/>).
    /// </summary>
    public CachingBackendStatus Status => (CachingBackendStatus) this._status;

    private bool TryChangeStatus( CachingBackendStatus expectedStatus, CachingBackendStatus newStatus )
        => Interlocked.CompareExchange( ref this._status, (int) newStatus, (int) expectedStatus ) == (int) expectedStatus;

    /// <inheritdoc />
    public override string ToString()
        => string.Format( CultureInfo.InvariantCulture, "{{{0} Id={1}, Status={2}}}", this.GetType().Name, this.DebugName, this.Status );

    /// <summary>
    /// Returns a <see cref="Task"/> that is signaled to the complete state when all background tasks
    /// have completed. 
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/> that is signaled to the complete state when all background tasks
    /// have completed.</returns>
    /// <remarks>
    /// <para>
    /// Other background tasks may possibly be enqueued between the time
    /// the returned <see cref="Task"/> has been signaled and the time the signal is processed
    /// by the caller.
    /// </para>
    /// </remarks>
    public virtual Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken ) => Task.CompletedTask;

    /// <summary>
    /// Event raised when a cache item is removed from the cache. Check the <see cref="CachingBackendFeatures.Events"/>
    /// property to see if the current <see cref="CachingBackend"/> supports events.
    /// </summary>
    public event EventHandler<CacheItemRemovedEventArgs> ItemRemoved
    {
        add
        {
            if ( !this.SupportedFeatures.Events )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "{0} does not support events.", this.GetType().Name ) );
            }

            this._itemRemoved += value;
        }
        remove => this._itemRemoved -= value;
    }

    /// <summary>
    /// Events raised when a dependency is invalidated. Check the <see cref="CachingBackendFeatures.Events"/>
    /// property to see if the current <see cref="CachingBackend"/> supports events.
    /// </summary>
    public event EventHandler<CacheDependencyInvalidatedEventArgs> DependencyInvalidated
    {
        add
        {
            if ( !this.SupportedFeatures.Events )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "{0} does not support events.", this.GetType().Name ) );
            }

            if ( !this.SupportedFeatures.Dependencies )
            {
                throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "{0} does not support dependencies.", this.GetType().Name ) );
            }

            this._dependencyInvalidated += value;
        }
        remove => this._dependencyInvalidated -= value;
    }

    private void Validate( CacheItem cacheItem )
    {
        if ( cacheItem.Configuration != null )
        {
            if ( cacheItem.Configuration.AbsoluteExpiration.HasValue && cacheItem.Configuration.SlidingExpiration.HasValue )
            {
                throw new ArgumentException( "The absolute and sliding expiration values are mutually exclusive. They cannot be set both at the same time." );
            }

            if ( (cacheItem.Configuration.AbsoluteExpiration.HasValue || cacheItem.Configuration.SlidingExpiration.HasValue)
                 && cacheItem.Configuration.Priority == CacheItemPriority.NotRemovable )
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The priority cannot be set to '{0}' when either the absolute expiration or sliding expiration is set as well.",
                        CacheItemPriority.NotRemovable ) );
            }
        }

        if ( !this.SupportedFeatures.Dependencies && cacheItem.Dependencies?.Count > 0 )
        {
            this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );
        }
    }

    /// <summary>
    /// Raises the <see cref="ItemRemoved"/> event given a <see cref="CacheItemRemovedEventArgs"/>.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected void OnItemRemoved( CacheItemRemovedEventArgs args )
    {
        this.LogSource.Debug.Write( Formatted( "OnItemRemoved( backend=\"{Backend}\", Reason={Reason}, Key=\"{Key}\"", this, args.RemovedReason, args.Key ) );
        this._itemRemoved?.Invoke( this, args );
    }

    /// <summary>
    /// Raises the <see cref="ItemRemoved"/> event, but lazily instantiates the <see cref="CacheItemRemovedEventArgs"/>
    /// if there is an event client.
    /// </summary>
    /// <param name="key">The key of the removed cache item.</param>
    /// <param name="reason">The reason of the removal.</param>
    /// <param name="sourceId">The <see cref="Guid"/> of the <see cref="CachingBackend"/> instance that removed the item,.
    /// or <see cref="Guid.Empty"/> if the source <see cref="CachingBackend"/> cannot be determined.</param>
    protected void OnItemRemoved( string key, CacheItemRemovedReason reason, Guid sourceId )
    {
        this.LogSource.Debug.Write( Formatted( "OnItemRemoved( Backend=\"{Backend}\", Reason={Reason}, Key=\"{Key}\"", this, reason, key ) );
        this._itemRemoved?.Invoke( this, new CacheItemRemovedEventArgs( key, reason, sourceId ) );
    }

    /// <summary>
    /// Raises the <see cref="DependencyInvalidated"/> event , but lazily instantiates the <see cref="CacheDependencyInvalidatedEventArgs"/>
    /// if there is an event client.
    /// </summary>
    /// <param name="key">The key of the removed cache item.</param>
    /// <param name="sourceId">The <see cref="Guid"/> of the <see cref="CachingBackend"/> instance that removed the item,.
    /// or <see cref="Guid.Empty"/> if the source <see cref="CachingBackend"/> cannot be determined.</param>
    protected void OnDependencyInvalidated( string key, Guid sourceId )
    {
        this.LogSource.Debug.Write( Formatted( "OnDependencyInvalidated( Backend=\"{Backend}\", source={SourceId}, Key=\"{Key}\"", this, sourceId, key ) );
        this._dependencyInvalidated?.Invoke( this, new CacheDependencyInvalidatedEventArgs( key, sourceId ) );
    }

    /// <summary>
    /// Raises the <see cref="DependencyInvalidated"/> event given a <see cref="CacheDependencyInvalidatedEventArgs"/>.
    /// </summary>
    /// <param name="args">A <see cref="CacheDependencyInvalidatedEventArgs"/>.</param>
    protected void OnDependencyInvalidated( CacheDependencyInvalidatedEventArgs args )
    {
        this.LogSource.Debug.Write(
            Formatted( "OnDependencyInvalidated( Backend=\"{Backend}\", source={SourceId}, Key=\"{Key}\"", this, args.SourceId, args.Key ) );

        this._dependencyInvalidated?.Invoke( this, args );
    }

    /// <summary>
    /// Synchronously disposes the current <see cref="CachingBackend"/>.
    /// </summary>
    public void Dispose() => this.Dispose( true );

    /// <summary>
    /// Synchronously disposes the current <see cref="CachingBackend"/>, with a parameter instructing whether this method is called because
    /// of a call to the <see cref="Dispose()"/> method or because of object finalizing.
    /// </summary>
    /// <param name="disposing"><c>true</c> if this method is called because the <see cref="Dispose()"/> method has been called, or <c>false</c> if the object is being finalized.</param>
    protected void Dispose( bool disposing )
    {
        if ( this.TryChangeStatus( CachingBackendStatus.Default, CachingBackendStatus.Disposing ) )
        {
            using ( var activity = this.LogSource.Default.OpenActivity( Formatted( "Disposing" ) ) )
            {
                try
                {
                    this.DisposeCore( disposing );

                    if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.Disposed ) )
                    {
#if DEBUG
                        throw new CachingAssertionFailedException();
#else
                            this.LogSource.Error.Write( Formatted( "Cannot dispose back-end: cannot change the status to Disposed." ) );
                            return;
#endif
                    }

                    this._disposeTask.SetResult( true );
                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.DisposeFailed ) )
                    {
#if DEBUG
                        throw new CachingAssertionFailedException();
#else
                            this.LogSource.Error.Write( Formatted( "Cannot dispose back-end: cannot change the status to DisposeFailed." ) );
                            return;
#endif
                    }

                    this._disposeTask.SetException( e );
                    activity.SetException( e );

                    throw;
                }
            }
        }
        else
        {
            this._disposeTask.Task.Wait();
        }
    }

    /// <summary>
    /// Synchronously disposes the current <see cref="CachingBackend"/>. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
    /// </summary>
    protected virtual void DisposeCore( bool disposing )
    {
        if ( !this.WhenBackgroundTasksCompleted( CancellationToken.None ).Wait( _disposeTimeout ) )
        {
            throw new TimeoutException( "Timeout when waiting for background tasks to complete." );
        }
    }

    ValueTask IAsyncDisposable.DisposeAsync() => this.DisposeAsync();

    /// <summary>
    /// Asynchronously dispose the current <see cref="CachingBackend"/>.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public async ValueTask DisposeAsync( CancellationToken cancellationToken = default )
    {
        if ( this.TryChangeStatus( CachingBackendStatus.Default, CachingBackendStatus.Disposing ) )
        {
            using ( var activity = this.LogSource.Default.OpenAsyncActivity( Formatted( "Disposing" ) ) )
            {
                try
                {
                    await this.DisposeAsyncCore( cancellationToken );

                    if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.Disposed ) )
                    {
                        throw new CachingAssertionFailedException();
                    }

                    this._disposeTask.SetResult( true );
                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.DisposeFailed ) )
                    {
                        throw new CachingAssertionFailedException();
                    }

                    this._disposeTask.SetException( e );
                    activity.SetException( e );

                    throw;
                }
            }
        }
        else
        {
            await this._disposeTask.Task;
        }
    }

    /// <summary>
    /// Asynchronously disposes the current <see cref="CachingBackend"/>. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    protected virtual async ValueTask DisposeAsyncCore( CancellationToken cancellationToken )
    {
        var delay = Task.Delay( _disposeTimeout, cancellationToken );
        var task = await Task.WhenAny( this.WhenBackgroundTasksCompleted( cancellationToken ), delay );

        if ( task == delay )
        {
            throw new TimeoutException( "Timeout when waiting for background tasks to complete." );
        }
    }

    // Was [ExplicitCrossPackageInternal]. Used by Redis backend. Making protected for now.
    protected virtual int BackgroundTaskExceptions => 0;

    int ITestableCachingComponent.BackgroundTaskExceptions => this.BackgroundTaskExceptions;
}