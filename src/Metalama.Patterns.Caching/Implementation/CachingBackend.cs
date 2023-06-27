// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.ValueAdapters;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#pragma warning disable 420

namespace Metalama.Patterns.Caching.Implementation
{

    /// <summary>
    /// An abstraction of the physical implementation of the cache, where the <see cref="CacheAttribute"/> ends up writing to and reading from.
    /// </summary>
    public abstract class CachingBackend : ITestableCachingComponent
    {
        private static readonly Task<bool> falseTaskResult = Task.FromResult( false );
        private static readonly Task<bool> trueTaskResult = Task.FromResult( true );
        private CachingBackendFeatures features;
        private int status;
        private const int disposeTimeout = 30000;
        private readonly TaskCompletionSource<bool> disposeTask = new TaskCompletionSource<bool>();
        private EventHandler<CacheItemRemovedEventArgs> itemRemoved;
        private EventHandler<CacheDependencyInvalidatedEventArgs> dependencyInvalidated;
        private BackgroundTaskScheduler backgroundTaskScheduler;

        private BackgroundTaskScheduler GetLegacyBackgroundTaskScheduler()
        {
            return LazyInitializer.EnsureInitialized( ref this.backgroundTaskScheduler );
        }

        

        /// <summary>
        /// Gets the <see cref="PostSharp.Patterns.Diagnostics.Logger"/> that implementations can use to emit
        /// log records.
        /// </summary>
        protected Logger Logger { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> of the current <see cref="CachingBackend"/>.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the factory of value adapters (<see cref="IValueAdapter"/>), which allow for instance to cache things like <see cref="System.IO.Stream"/> or <see cref="System.Collections.IEnumerable"/>,
        /// which could not be otherwise cached.
        /// </summary>
        public ValueAdapterFactory ValueAdapters { get; } = new ValueAdapterFactory();

        /// <summary>
        /// Creates a <see cref="CachingBackendFeatures"/> object, which describes set of features implemented by the back-end.
        /// This method is invoked the first time the <see cref="SupportedFeatures"/> property is evaluated. The result is then cached.
        /// </summary>
        /// <returns>A new instance of the <see cref="CachingBackendEnhancerFeatures"/> class.</returns>
        protected virtual CachingBackendFeatures CreateFeatures() => new CachingBackendFeatures();

        /// <summary>
        /// Gets the set of features supported by the current <see cref="CachingBackend"/>.
        /// </summary>
        public CachingBackendFeatures SupportedFeatures => this.features ?? (this.features = this.CreateFeatures());

        private void RequireFeature( bool feature, string featureName )
        {
            if ( !feature )
            {
                throw new NotSupportedException( string.Format(CultureInfo.InvariantCulture, "Feature \"{0}\" is not supported by {1}.", featureName, this.GetType().Name ) );
            }
        }

        #region SetItem
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
        protected virtual Task SetItemAsyncCore(string key, CacheItem item, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.SetItemCore(key, item);
            return trueTaskResult;
        }

        /// <summary>
        /// Sets a cache item.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <param name="item">The cache item.</param>
        public void SetItem( [Required] string key, [Required] CacheItem item )
        {
            using ( LogActivity activity = this.Logger.OpenActivity( "SetItem( \"{Key}\" )", key ) )
            {
                try
                {
                    this.Validate( item );

                    this.SetItemCore( key, item );

                    activity.SetSuccess();
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
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
        public async Task SetItemAsync( [Required] string key, [Required] CacheItem item, CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("SetItemAsync( \"{Key}\")", key)) 
            {
                try
                {
                    this.Validate( item );

                    cancellationToken.ThrowIfCancellationRequested();
                    
                    await this.SetItemAsyncCore( key, item, cancellationToken );

                    activity.SetSuccess();
                }
                catch ( Exception e ) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion

        #region ContainsItem

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
        protected virtual Task<bool> ContainsItemAsyncCore(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.ContainsItemCore(key) ? trueTaskResult : falseTaskResult;
        }

        /// <summary>
        /// Determines whether the cache contains an item of a given key.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        /// <returns><c>true</c> if the cache contains an item whose key is <paramref name="key"/>, otherwise <c>false</c>.</returns>
        public bool ContainsItem( [Required] string key )
        {
            using ( LogActivity activity = this.Logger.OpenActivity( "ContainsItem( \"{Key}\" )", key ) )
            {
                try
                {
                    bool present = this.ContainsItemCore( key );

                    activity.SetSuccess( present ? "Cache hit." : "Cache miss." );

                    return present;
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
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
        public async Task<bool> ContainsItemAsync([Required] string key, CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("ContainsItemAsync( \"{Key}\" )", key))
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    bool present = await this.ContainsItemAsyncCore( key, cancellationToken );

                    activity.SetSuccess( present ? "Cache hit." : "Cache miss." );

                    return present;
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion

        #region GetItem

        /// <summary>
        /// Gets a cache item given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
        /// </summary>
        /// <param name="key">The cache item.</param>
        /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
        /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
        /// <returns>A <see cref="CacheValue"/>, or <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
        protected abstract CacheValue GetItemCore( string key, bool includeDependencies );

        /// <summary>
        /// Asynchronously gets a cache item given its key. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. Arguments are already validated by the consumer API.
        /// </summary>
        /// <param name="key">The cache item.</param>
        /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
        /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> evaluating to a <see cref="CacheValue"/>, or evaluating to <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
        protected virtual Task<CacheValue> GetItemAsyncCore(string key, bool includeDependencies, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(this.GetItemCore(key, includeDependencies));
        }

        /// <summary>
        /// Gets a cache item given its key.
        /// </summary>
        /// <param name="key">The cache item.</param>
        /// <param name="includeDependencies"><c>true</c> if the <see cref="CacheValue.Dependencies"/> properties of the
        /// resulting <see cref="CacheValue"/> should be populated, otherwise <c>false</c>.</param>
        /// <returns>A <see cref="CacheValue"/>, or <c>null</c> if there is no item in cache of the given <paramref name="key"/>.</returns>
        public CacheValue GetItem( [Required] string key, bool includeDependencies = true )
        {
            using ( LogActivity activity = this.Logger.OpenActivity( "GetItem( \"{Key}\" )", key ) )
            {
                try
                {
                    CacheValue item = null;

                    try
                    {
                        item = this.GetItemCore( key, includeDependencies );
                    }
                    catch ( Exception e ) when ( e is InvalidCacheItemException || e is InvalidCastException )
                    {
                        activity.WriteException( LogLevel.Warning, e,
                                                 "The cached object or method source code or caching settings may have changed since the value has been cached. Removing the item. " +
                                                 "To avoid this warning, change the cache key prefix when you do breaking changes to the source code." );
                        this.RemoveItemCore( key );
                    }

                    activity.SetSuccess( item != null ? "Cache hit." : "Cache miss." );

                    return item;
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
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
        public async Task<CacheValue> GetItemAsync( [Required] string key, bool includeDependencies = true,
                                                    CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("GetItemAsync( \"{Key}\" )", key))
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    CacheValue item = null;

                    try
                    {
                        item = await this.GetItemAsyncCore( key, includeDependencies, cancellationToken );
                    }
                    catch ( Exception e ) when ( e is InvalidCacheItemException || e is InvalidCastException )
                    {
                        activity.WriteException( LogLevel.Info, e,
                                                 "The cached object or method source code or caching settings may have changed since the value has been cached. Removing the item." );
                        await this.RemoveItemAsyncCore( key, cancellationToken );
                    }

                    activity.SetSuccess( item != null ? "Cache hit." : "Cache miss." );

                    return item;
                }
                catch ( Exception e ) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion

        #region RemoveItem

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
        protected virtual Task RemoveItemAsyncCore(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.RemoveItemCore(key);
            return trueTaskResult;
        }



        /// <summary>
        /// Removes a cache item from the cache given its key.
        /// </summary>
        /// <param name="key">The key of the cache item.</param>
        public void RemoveItem( [Required] string key )
        {
            using (LogActivity activity = this.Logger.OpenActivity("RemoveItem( \"{Key}\" )", key))
            {
                try
                {
                    this.RemoveItemCore( key );

                    activity.SetSuccess();
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
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
        public async Task RemoveItemAsync( [Required] string key, CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("RemoveItemAsync( \"{Key}\" )", key))
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.RemoveItemAsyncCore( key, cancellationToken );

                    activity.SetSuccess();
                }
                catch ( Exception e ) when (activity.SetException(e) )
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion


        #region InvalidateDependency
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
        protected virtual Task InvalidateDependencyAsyncCore(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.InvalidateDependencyCore(key);
            return trueTaskResult;
        }

        /// <summary>
        /// Removes from the cache all items that have a specific dependency.
        /// </summary>
        /// <param name="key">The dependency key.</param>
        public void InvalidateDependency( [Required] string key )
        {
            using (LogActivity activity = this.Logger.OpenActivity("InvalidateDependency( \"{Key}\" )", key))
            {
                try
                {
                    this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );

                    this.InvalidateDependencyCore( key );

                    activity.SetSuccess();
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
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
        public async Task InvalidateDependencyAsync( [Required] string key, CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("InvalidateDependencyAsync( \"{Key}\" )", key))
            {
                try
                {
                    this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );

                    cancellationToken.ThrowIfCancellationRequested();

                    await this.InvalidateDependencyAsyncCore( key, cancellationToken );

                    activity.SetSuccess();
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion

        #region ContainsDependency
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
        protected virtual Task<bool> ContainsDependencyAsyncCore(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.ContainsDependencyCore(key) ? trueTaskResult : falseTaskResult;
        }

        /// <summary>
        /// Determines whether the cache contains a given dependency.
        /// </summary>
        /// <param name="key">The dependency key.</param>
        /// <returns><c>true</c> if the cache contains the dependency <paramref name="key"/>, otherwise <c>false</c>.</returns>

        public bool ContainsDependency( [Required] string key )
        {
            using (LogActivity activity = this.Logger.OpenActivity("ContainsDependency( \"{Key}\" )", key))
            {
                try
                {
                    this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );
                    this.RequireFeature( this.SupportedFeatures.ContainsDependency, nameof(this.SupportedFeatures.ContainsDependency) );

                    bool present = this.ContainsDependencyCore( key );

                    activity.SetSuccess( present ? "Cache hit." : "Cache miss." );

                    return present;
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
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
        public async Task<bool> ContainsDependencyAsync( [Required] string key, CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("ContainsDependencyAsync( \"{Key}\" )", key))
            {
                try
                {
                    activity.Write( LogLevel.Debug, "The dependency key is \"{Key}\".", key );

                    this.RequireFeature( this.SupportedFeatures.Dependencies, nameof(this.SupportedFeatures.Dependencies) );
                    this.RequireFeature( this.SupportedFeatures.ContainsDependency, nameof(this.SupportedFeatures.ContainsDependency) );

                    cancellationToken.ThrowIfCancellationRequested();

                    bool present = await this.ContainsDependencyAsyncCore( key, cancellationToken );

                    activity.SetSuccess( present ? "Cache hit." : "Cache miss." );

                    return present;
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion


        #region Clear
        /// <summary>
        /// Clears the cache. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked. 
        /// </summary>
        protected abstract void ClearCore();

        /// <summary>
        /// Asynchronously clears the cache. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected virtual Task ClearAsyncCore(CancellationToken cancellationToken)
        {
            this.ClearCore();
            return trueTaskResult;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            using ( LogActivity activity = this.Logger.OpenActivity( "Clear()" ) )
            {
                try
                {
                    this.ClearCore();

                    activity.SetSuccess();
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously clears the cache.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task ClearAsync( CancellationToken cancellationToken = default(CancellationToken) )
        {
            using (LogActivity activity = this.Logger.OpenActivity("ClearAsync()"))
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await this.ClearAsyncCore( cancellationToken );
                }
                catch ( Exception e) when (activity.SetException(e))
                {
                    // Unreachable.
                    throw;
                }
            }
        }

        #endregion


        /// <summary>
        /// Gets the status of the current <see cref="CachingBackend"/> (<see cref="CachingBackendStatus.Default"/>,
        /// <see cref="CachingBackendStatus.Disposing"/> or <see cref="CachingBackendStatus.Disposed"/>).
        /// </summary>
        public CachingBackendStatus Status => (CachingBackendStatus)this.status;

        private bool TryChangeStatus( CachingBackendStatus expectedStatus, CachingBackendStatus newStatus )
        {
            return Interlocked.CompareExchange(ref this.status, (int)newStatus, (int)expectedStatus) == (int)expectedStatus;
        }

        /// <summary>
        /// Initializes a new <see cref="CachingBackend"/>.
        /// </summary>
        protected CachingBackend()
        {
            this.Id = Guid.NewGuid();
            this.AutoReloadManager = new AutoReloadManager( this);

            // TODO: We may need an instance-scoped logger instead of a type-scoped logger.
#pragma warning disable 618
            this.Logger = this.Logger.GetLogger( LoggingRoles.Caching, this.GetType() );
#pragma warning restore 618
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{{0} {1}}}", this.GetType().Name, this.Id );
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that is signaled to the complete state when all background tasks
        /// have completed. 
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> that is signaled to the complete state when all background tasks
        /// have completed</returns>
        /// <remarks>
        /// <para>
        /// Other background tasks may possibly be enqueued between the time
        /// the returned <see cref="Task"/> has been signaled and the time the signal is processed
        /// by the caller.
        /// </para>
        /// </remarks>
        public virtual Task WhenBackgroundTasksCompleted( CancellationToken cancellationToken )
        {
            if ( this.backgroundTaskScheduler == null )
                return PortableThreadingApi.CompletedTask;
            else
                return  this.backgroundTaskScheduler.WhenBackgroundTasksCompleted( cancellationToken );
        }


        /// <summary>
        /// Event raised when a cache item is removed from the cache. Check the <see cref="CachingBackendFeatures.Events"/>
        /// property to see if the current <see cref="CachingBackend"/> supports events.
        /// </summary>
        public event EventHandler<CacheItemRemovedEventArgs> ItemRemoved
        {
            add
            {
                if ( !this.SupportedFeatures.Events )
                    throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "{0} does not support events.", this.GetType().Name ) );

                this.itemRemoved += value;
            }
            remove
            {
                this.itemRemoved -= value;
            }
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
                    throw new InvalidOperationException( string.Format(CultureInfo.InvariantCulture, "{0} does not support events.", this.GetType().Name ) );

                if (!this.SupportedFeatures.Dependencies)
                    throw new InvalidOperationException( string.Format(CultureInfo.InvariantCulture, "{0} does not support dependencies.", this.GetType().Name ) );

                this.dependencyInvalidated += value;
            }
            remove { this.dependencyInvalidated -= value; }
        }

        private void Validate( CacheItem cacheItem )
        {
            if ( cacheItem.Configuration != null )
            {
                if ( cacheItem.Configuration.AbsoluteExpiration.HasValue && cacheItem.Configuration.SlidingExpiration.HasValue )
                {
                    throw new ArgumentException(
                              "The absolute and sliding expiration values are mutually exclusive. They cannot be set both at the same time." );
                }

                if ( (cacheItem.Configuration.AbsoluteExpiration.HasValue || cacheItem.Configuration.SlidingExpiration.HasValue)
                     && cacheItem.Configuration.Priority == CacheItemPriority.NotRemovable )
                {
                    throw new ArgumentException(
                              string.Format(CultureInfo.InvariantCulture, "The priority cannot be set to '{0}' when either the absolute expiration or sliding expiration is set as well.",
                                             CacheItemPriority.NotRemovable ) );
                }
            }

            if ( !this.SupportedFeatures.Dependencies && cacheItem.Dependencies?.Count > 0 )
            {
                this.RequireFeature( this.SupportedFeatures.Dependencies, nameof( this.SupportedFeatures.Dependencies ) );
            }

        }

        /// <summary>
        /// Raises the <see cref="ItemRemoved"/> event given a <see cref="CacheItemRemovedEventArgs"/>.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected void OnItemRemoved( [Required] CacheItemRemovedEventArgs args )
        {
            this.Logger.Write( LogLevel.Debug, "Item removed. Reason={Reason}, Key=\"{Key}\"", args.RemovedReason, args.Key );
            this.itemRemoved?.Invoke( this, args );
        }

        /// <summary>
        /// Raises the <see cref="ItemRemoved"/> event, but lazily instantiates the <see cref="CacheItemRemovedEventArgs"/>
        /// if there is an event client.
        /// </summary>
        /// <param name="key">The key of the removed cache item.</param>
        /// <param name="reason">The reason of the removal.</param>
        /// <param name="sourceId">The <see cref="Guid"/> of the <see cref="CachingBackend"/> instance that removed the item,.
        /// or <see cref="Guid.Empty"/> if the source <see cref="CachingBackend"/> cannot be determined.</param>
        protected void OnItemRemoved( [Required] string key, CacheItemRemovedReason reason, Guid sourceId )
        {
            this.Logger.Write( LogLevel.Debug, "Item removed. Reason={Reason}, Key=\"{Key}\"", reason, key );
            this.itemRemoved?.Invoke( this, new CacheItemRemovedEventArgs( key, reason, sourceId ) );
        }

        /// <summary>
        /// Raises the <see cref="DependencyInvalidated"/> event , but lazily instantiates the <see cref="CacheDependencyInvalidatedEventArgs"/>
        /// if there is an event client.
        /// </summary>
        /// <param name="key">The key of the removed cache item.</param>
        /// <param name="sourceId">The <see cref="Guid"/> of the <see cref="CachingBackend"/> instance that removed the item,.
        /// or <see cref="Guid.Empty"/> if the source <see cref="CachingBackend"/> cannot be determined.</param>
        protected void OnDependencyInvalidated( [Required] string key, Guid sourceId )
        {
            this.Logger.Write( LogLevel.Debug, "Dependency invalidated. Key=\"{Key}\"", key );
            this.dependencyInvalidated?.Invoke( this, new CacheDependencyInvalidatedEventArgs( key, sourceId ) );
        }

        /// <summary>
        /// Raises the <see cref="DependencyInvalidated"/> event given a <see cref="CacheDependencyInvalidatedEventArgs"/>.
        /// </summary>
        /// <param name="args">A <see cref="CacheDependencyInvalidatedEventArgs"/>.</param>
        protected void OnDependencyInvalidated( [Required] CacheDependencyInvalidatedEventArgs args )
        {
            this.Logger.Write( LogLevel.Debug, "Dependency invalidated. Key=\"{Key}\"", args.Key );
            this.dependencyInvalidated?.Invoke( this, args );
        }


        /// <summary>
        /// Synchronously disposes the current <see cref="CachingBackend"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1063:ImplementIDisposableCorrectly" )]
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations" )]
        public void Dispose()
        {
            this.Dispose( true );
        }

        /// <summary>
        /// Synchronously disposes the current <see cref="CachingBackend"/>, with a parameter instructing whether this method is called because
        /// of a call to the <see cref="Dispose()"/> method or because of object finalizing.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this method is called because the <see cref="Dispose()"/> method has been called, or <c>false</c> if the object is being finalized.</param>
        [SuppressMessage("Microsoft.Design", "CA1063")] // The Dispose pattern is implemented differently.
        protected void Dispose( bool disposing )
        {
            if ( this.TryChangeStatus( CachingBackendStatus.Default, CachingBackendStatus.Disposing ) )
            {
                using ( LogActivity activity = this.Logger.OpenActivity( "Disposing" ) )
                {
                    try
                    {
                        this.DisposeCore( disposing );

                        if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.Disposed ) )
                        {
#if DEBUG
                            throw new AssertionFailedException();
#else
                            this.Logger.Write( LogLevel.Error, "Cannot dispose back-end: cannot change the status to Disposed." );
                            return;
#endif
                        }

                        this.disposeTask.SetResult( true );
                        activity.SetSuccess();
                    }
                    catch ( Exception e )
                    {
                        if (!this.TryChangeStatus(CachingBackendStatus.Disposing, CachingBackendStatus.DisposeFailed))
                        {
#if DEBUG
                            throw new AssertionFailedException();
#else
                            this.Logger.Write( LogLevel.Error, "Cannot dispose back-end: cannot change the status to DisposeFailed." );
                            return;
#endif
                        }

                        this.disposeTask.SetException( e );
                        activity.SetException( e );
                        throw;
                    }
                }
                
            }
            else
            {
                this.disposeTask.Task.Wait();
            }
        }

        /// <summary>
        /// Synchronously disposes the current <see cref="CachingBackend"/>. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
        /// </summary>
        protected virtual void DisposeCore(bool disposing)
        {
#pragma warning disable 612
            if ( !this.WhenBackgroundTasksCompleted( CancellationToken.None ).Wait( disposeTimeout ) )
#pragma warning restore 612
            {
                throw new TimeoutException("Timeout when waiting for background tasks to complete.");
            }
        }

        /// <summary>
        /// Asynchronously dispose the current <see cref="CachingBackend"/>.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task DisposeAsync( CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.TryChangeStatus(CachingBackendStatus.Default, CachingBackendStatus.Disposing))
            {
                using (LogActivity activity = this.Logger.OpenActivity( "Disposing" ) )
                {
                    try
                    {
                        await this.DisposeAsyncCore( cancellationToken );

                        if ( !this.TryChangeStatus( CachingBackendStatus.Disposing, CachingBackendStatus.Disposed ) )
                            throw new AssertionFailedException();

                        this.disposeTask.SetResult( true );
                        activity.SetSuccess();
                    }
                    catch ( Exception e )
                    {
                        if (!this.TryChangeStatus(CachingBackendStatus.Disposing, CachingBackendStatus.DisposeFailed))
                            throw new AssertionFailedException();

                        this.disposeTask.SetException( e );
                        activity.SetException( e );
                        throw;
                    }
                }
            }
            else
            {
                await this.disposeTask.Task;
            }
        }

        /// <summary>
        /// Asynchronously disposes the current <see cref="CachingBackend"/>. This protected method is part of the implementation API and is meant to be overridden in user code, not invoked.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        protected virtual async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            Task delay = Task.Delay( disposeTimeout, cancellationToken );
#pragma warning disable 612
            Task task = await Task.WhenAny( this.WhenBackgroundTasksCompleted( cancellationToken ), delay );
#pragma warning restore 612
            if ( task == delay )
            {
                throw new TimeoutException("Timeout when waiting for background tasks to complete.");
            }
        }



        [ExplicitCrossPackageInternal]
        internal virtual int BackgroundTaskExceptions
        {
            get
            {
                if ( this.backgroundTaskScheduler == null )
                    return 0;
                else
                    return this.backgroundTaskScheduler.BackgroundTaskExceptions;
            }
        }

        int ITestableCachingComponent.BackgroundTaskExceptions => this.BackgroundTaskExceptions;

        internal AutoReloadManager AutoReloadManager { get; }

     
    }
}