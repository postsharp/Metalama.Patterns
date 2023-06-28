// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Backends;

/// <summary>
/// A <see cref="CachingBackendEnhancer"/> that adds a local (fast) <see cref="MemoryCachingBackend"/> to a remote (slower) cache.
/// This class is typically instantiate in the back-end factory method. You should normally not use this class unless you develop a custom caching back-end.
/// </summary>
[PublicAPI]
public sealed class TwoLayerCachingBackendEnhancer : CachingBackendEnhancer
{
    private readonly TimeSpan _removedItemTransitionPeriod = TimeSpan.FromMinutes( 1 );

    /// <summary>
    /// Gets the in-memory local cache.
    /// </summary>
    public MemoryCachingBackend LocalCache { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TwoLayerCachingBackendEnhancer"/> class.
    /// Initializes a new <see cref="TwoLayerCachingBackendEnhancer"/>.
    /// </summary>
    /// <param name="remoteCache">The remote <see cref="CachingBackend"/>.</param>
    /// <param name="memoryCache">A <see cref="MemoryCachingBackend"/>, or <c>null</c> to use a new default <see cref="MemoryCachingBackend"/>.</param>
    public TwoLayerCachingBackendEnhancer( [Required] CachingBackend remoteCache, MemoryCachingBackend? memoryCache = null ) : base( remoteCache )
    {
        this.LocalCache = memoryCache ?? new MemoryCachingBackend();
    }

    /// <inheritdoc />
    protected override void OnBackendDependencyInvalidated( object? sender, CacheDependencyInvalidatedEventArgs args )
    {
        if ( args.SourceId != this.UnderlyingBackend.Id )
        {
            this.LocalCache.InvalidateDependency( args.Key );
        }

        base.OnBackendDependencyInvalidated( sender, args );
    }

    /// <inheritdoc />
    protected override void OnBackendItemRemoved( object? sender, CacheItemRemovedEventArgs args )
    {
        if ( args.SourceId != this.UnderlyingBackend.Id )
        {
            this.LocalCache.RemoveItem( args.Key );
        }

        base.OnBackendItemRemoved( sender, args );
    }

    /// <inheritdoc />
    protected override void SetItemCore( string key, CacheItem item )
    {
        var value = new TwoLayerCacheValue( item );
        var newItem = item.WithValue( value );

        this.LocalCache.SetItem( key, item );
        this.UnderlyingBackend.SetItem( key, newItem );
    }

    /// <inheritdoc />
    protected override Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        var value = new TwoLayerCacheValue( item );
        var newItem = item.WithValue( value );

        this.LocalCache.SetItem( key, item );

        return this.UnderlyingBackend.SetItemAsync( key, newItem, cancellationToken );
    }

    /// <inheritdoc />
    protected override bool ContainsItemCore( string key )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            return this.LocalCache.ContainsItem( key ) || this.UnderlyingBackend.ContainsItem( key );
        }
        else
        {
            return this.GetItemCore( key, false ) != null;
        }
    }

    /// <inheritdoc />
    protected override async Task<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            return this.LocalCache.ContainsItem( key ) || await this.UnderlyingBackend.ContainsItemAsync( key, cancellationToken );
        }
        else
        {
            return await this.GetItemAsyncCore( key, false, cancellationToken ) != null;
        }
    }

    /// <inheritdoc />
    protected override bool ContainsDependencyCore( string key )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            return this.LocalCache.ContainsDependency( key ) || this.UnderlyingBackend.ContainsDependency( key );
        }
        else
        {
            throw new NotSupportedException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "ContainsDependency is not supported with in {0} with a non-blocking remote backend.",
                    nameof(TwoLayerCachingBackendEnhancer) ) );
        }
    }

    /// <inheritdoc />
    protected override async Task<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            // ReSharper disable once MethodHasAsyncOverloadWithCancellation
            return this.LocalCache.ContainsDependency( key ) || await this.UnderlyingBackend.ContainsDependencyAsync( key, cancellationToken );
        }
        else
        {
            throw new NotSupportedException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "ContainsDependencyAsync is not supported with in {0} with a non-blocking remote backend.",
                    nameof(TwoLayerCachingBackendEnhancer) ) );
        }
    }

    /// <inheritdoc />
    protected override CacheValue? GetItemCore( string key, bool includeDependencies )
    {
        var localCacheValue = this.LocalCache.GetItem( key, includeDependencies );

        if ( localCacheValue == null )
        {
            var remoteCacheValue = this.GetValueFromUnderlyingBackend( key );

            if ( remoteCacheValue != null )
            {
                // We have a value stored remotely.
                // Cache in local memory.
                this.SetMemoryCacheFromRemote( key, remoteCacheValue );

                return GetReturnValue( remoteCacheValue );
            }
            else
            {
                return null;
            }
        }
        else
        {
            switch ( localCacheValue )
            {
                case RemovedValue removedValue:
                    {
                        // We have the magic string meaning that the node has been deleted.

                        var remoteCacheValue = this.GetValueFromUnderlyingBackend( key );

                        if ( remoteCacheValue == null )
                        {
                            return null;
                        }
                        else
                        {
                            var multiLayerCacheValue = (TwoLayerCacheValue) (remoteCacheValue.Value
                                                                             ?? throw new MetalamaPatternsCachingAssertionFailedException(
                                                                                 "null not expected." ));

                            if ( multiLayerCacheValue.Timestamp > removedValue.Timestamp )
                            {
                                this.SetMemoryCacheFromRemote( key, remoteCacheValue );

                                return GetReturnValue( remoteCacheValue );
                            }
                            else
                            {
                                // The remote value is older than the deletion.
                                return null;
                            }
                        }
                    }

                default:
                    return localCacheValue;
            }
        }
    }

    private CacheValue? GetValueFromUnderlyingBackend( string key )
    {
        var cacheValue = this.UnderlyingBackend.GetItem( key );

        if ( cacheValue == null )
        {
            return null;
        }

        return cacheValue;
    }

    private async Task<CacheValue?> GetValueFromUnderlyingBackendAsync( string key, CancellationToken cancellationToken )
    {
        var cacheValue = await this.UnderlyingBackend.GetItemAsync( key, true, cancellationToken );

        if ( cacheValue == null )
        {
            return null;
        }

        return cacheValue;
    }

    private static CacheValue GetReturnValue( CacheValue storedCacheValue )
    {
        var multiLayerCacheValue = (TwoLayerCacheValue) (
            storedCacheValue.Value ?? throw new MetalamaPatternsCachingAssertionFailedException( "null not expected." ));

        return new CacheValue( multiLayerCacheValue.Value, storedCacheValue.Dependencies );
    }

    private void SetMemoryCacheFromRemote( string key, CacheValue remoteCacheValue )
    {
        var multiLayerCacheValue =
            (TwoLayerCacheValue) (remoteCacheValue.Value ?? throw new MetalamaPatternsCachingAssertionFailedException( "null not expected." ));

        var cacheItem = new CacheItem(
            multiLayerCacheValue.Value,
            remoteCacheValue.Dependencies?.ToImmutableList(),
            multiLayerCacheValue.ToCacheItemConfiguration() );

        this.LocalCache.SetItem( key, cacheItem );
    }

    /// <inheritdoc />
    protected override async Task<CacheValue?> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
    {
        // ReSharper disable once MethodHasAsyncOverloadWithCancellation
        var localCacheValue = this.LocalCache.GetItem( key, includeDependencies );

        if ( localCacheValue == null )
        {
            var remoteCacheValue = await this.GetValueFromUnderlyingBackendAsync( key, cancellationToken );

            if ( remoteCacheValue != null )
            {
                // We have a value stored remotely.
                // Cache in local memory.
                this.SetMemoryCacheFromRemote( key, remoteCacheValue );

                return GetReturnValue( remoteCacheValue );
            }
            else
            {
                return null;
            }
        }
        else
        {
            switch ( localCacheValue )
            {
                case RemovedValue removedValue:
                    {
                        // We have the magic string meaning that the node has been deleted.

                        var remoteCacheValue = await this.GetValueFromUnderlyingBackendAsync( key, cancellationToken );

                        if ( remoteCacheValue == null )
                        {
                            return null;
                        }
                        else
                        {
                            var multiLayerCacheValue = (TwoLayerCacheValue) (remoteCacheValue.Value
                                                                             ?? throw new MetalamaPatternsCachingAssertionFailedException(
                                                                                 "null not expected." ));

                            if ( multiLayerCacheValue.Timestamp > removedValue.Timestamp )
                            {
                                this.SetMemoryCacheFromRemote( key, remoteCacheValue );

                                return GetReturnValue( remoteCacheValue );
                            }
                            else
                            {
                                // The remote value is older than the deletion.
                                return null;
                            }
                        }
                    }

                default:
                    return localCacheValue;
            }
        }
    }

    /// <inheritdoc />
    protected override void InvalidateDependencyCore( string key )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            this.LocalCache.InvalidateDependency( key );
        }
        else
        {
            this.LocalCache.InvalidateDependencyImpl( key, new RemovedValue(), DateTimeOffset.UtcNow + this._removedItemTransitionPeriod );
        }

        this.UnderlyingBackend.InvalidateDependency( key );
    }

    /// <inheritdoc />
    protected override Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            this.LocalCache.InvalidateDependency( key );
        }
        else
        {
            this.LocalCache.InvalidateDependencyImpl( key, new RemovedValue(), DateTimeOffset.UtcNow + this._removedItemTransitionPeriod );
        }

        return this.UnderlyingBackend.InvalidateDependencyAsync( key, cancellationToken );
    }

    /// <inheritdoc />
    protected override void RemoveItemCore( string key )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            this.LocalCache.RemoveItem( key );
        }
        else
        {
            this.LocalCache.RemoveItemImpl( key, new RemovedValue(), DateTimeOffset.UtcNow + this._removedItemTransitionPeriod );
        }

        this.UnderlyingBackend.RemoveItem( key );
    }

    /// <inheritdoc />
    protected override Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        if ( this.UnderlyingBackend.SupportedFeatures.Blocking )
        {
            this.LocalCache.RemoveItem( key );
        }
        else
        {
            this.LocalCache.RemoveItemImpl( key, new RemovedValue(), DateTime.UtcNow + this._removedItemTransitionPeriod );
        }

        return this.UnderlyingBackend.RemoveItemAsync( key, cancellationToken );
    }

    /// <inheritdoc />
    protected override void ClearCore()
    {
        this.LocalCache.Clear();
        this.UnderlyingBackend.Clear();
    }

    /// <inheritdoc />
    protected override Task ClearAsyncCore( CancellationToken cancellationToken )
    {
        this.LocalCache.Clear();

        return this.UnderlyingBackend.ClearAsync( cancellationToken );
    }

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );

        this.LocalCache.Dispose();
    }

    /// <inheritdoc />
    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken );
        await this.LocalCache.DisposeAsync( cancellationToken );
    }

    /// <inheritdoc />
    protected override CachingBackendFeatures CreateFeatures() => new Features( this.UnderlyingBackend.SupportedFeatures );

    internal static long GetTimestamp() => DateTimeOffset.UtcNow.UtcTicks;

    private sealed class Features : CachingBackendEnhancerFeatures
    {
        public Features( CachingBackendFeatures underlyingBackendFeatures ) : base( underlyingBackendFeatures ) { }

        public override bool ContainsDependency => this.UnderlyingBackendFeatures is { ContainsDependency: true, Blocking: true };
    }

    private sealed class RemovedValue : MemoryCacheValue
    {
#pragma warning disable SA1401
        public readonly long Timestamp = GetTimestamp();
#pragma warning restore SA1401

        public RemovedValue() : base( null, null, new object() ) { }
    }

    /// <summary>
    /// The object stored in the remote class.
    /// </summary>
    [PSerializable]
    [Serializable]
    [DataContract]

    // TODO: Unnest this type.
#pragma warning disable CA1034 // Nested types should not be visible
    public sealed class TwoLayerCacheValue
#pragma warning restore CA1034 // Nested types should not be visible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwoLayerCacheValue"/> class.
        /// </summary>
        /// <param name="item">The original <see cref="CacheItem"/>.</param>
        public TwoLayerCacheValue( [Required] CacheItem item )
        {
            this.Value = item.Value;
            this.SlidingExpiration = item.Configuration?.SlidingExpiration;
            this.Priority = item.Configuration?.Priority;

            if ( item.Configuration?.AbsoluteExpiration != null )
            {
                this.AbsoluteExpiration = DateTime.UtcNow + item.Configuration?.AbsoluteExpiration.Value;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp of the cache item.
        /// </summary>
        [DataMember]
        public long Timestamp { get; set; } = GetTimestamp();

        /// <summary>
        /// Gets or sets the cached value.
        /// </summary>
        [DataMember]
        public object? Value { get; set; }

        /// <summary>
        /// Gets or sets the absolute expiration of the cache item.
        /// </summary>
        [DataMember]
        public DateTime? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets the sliding expiration of the cache item.
        /// </summary>
        [DataMember]
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// Gets or sets the cache item priority.
        /// </summary>
        [DataMember]
        public CacheItemPriority? Priority { get; set; }

        internal CacheItemConfiguration ToCacheItemConfiguration()
        {
            var configuration = new CacheItemConfiguration { Priority = this.Priority, SlidingExpiration = this.SlidingExpiration };

            if ( this.AbsoluteExpiration != null )
            {
                configuration.AbsoluteExpiration = DateTime.UtcNow - this.AbsoluteExpiration.Value;
            }

            return configuration;
        }
    }
}