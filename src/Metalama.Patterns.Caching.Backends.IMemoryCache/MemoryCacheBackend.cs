// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;
using System.Globalization;
using CacheItemPriority = Microsoft.Extensions.Caching.Memory.CacheItemPriority;
using MemoryCache = Microsoft.Extensions.Caching.Memory.MemoryCache;
using PSCacheItem = Metalama.Patterns.Caching.Implementation.CacheItem;
using PSCacheItemPriority = Metalama.Patterns.Caching.Implementation.CacheItemPriority;

namespace Metalama.Patterns.Caching.Backends;

/// <summary>
/// A <see cref="CachingBackend"/> based on Microsoft.Extensions.Caching.Memory.IMemoryCache (<see cref="Microsoft.Extensions.Caching.Memory.IMemoryCache"/>).
/// This cache is recommended for ASP.NET Core use (as opposed to <c>System.Runtime.Caching.MemoryCache</c>).
/// </summary>
/// <remarks>
/// This backend converts PostSharp configuration properties into <see cref="ICacheEntry"/> instances as follows:
/// <list type="bullet">
///    <item>The priority <c>Default</c> is converted to <see cref="CacheItemPriority.Normal"/>.</item>
///    <item>The property <see cref="ICacheEntry.Size"/> is normally not set. If you want it to be set, supply a function to calculate this value for each entry
/// in the <see cref="MemoryCacheBackend(IMemoryCache,Func{PSCacheItem,long})"/> constructor. You only need to do this if you intend to limit the size
/// of the cache.</item>
/// </list>
/// </remarks>
[PublicAPI]
public sealed class MemoryCacheBackend : CachingBackend
{
    private readonly IMemoryCache _cache;
    private readonly Func<PSCacheItem, long>? _sizeCalculator;

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        base.DisposeCore( disposing );
        this._cache.Dispose();
    }

    /// <inheritdoc />
    protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await base.DisposeAsyncCore( cancellationToken ).ConfigureAwait( false );
        this._cache.Dispose();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheBackend"/> class based on a new instance of the <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/> class.
    /// </summary>
    public MemoryCacheBackend() : this( new MemoryCache( new MemoryCacheOptions() ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheBackend"/> class based on the given <see cref="IMemoryCache"/>.
    /// </summary>
    /// <param name="cache">An <see cref="IMemoryCache"/>.</param>
    public MemoryCacheBackend( [Required] IMemoryCache cache ) : this( cache, null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheBackend"/> class based on the given <see cref="IMemoryCache"/>. The backend creates cache entries
    /// with size calculated by the given function.
    /// </summary>
    /// <param name="cache">An <see cref="IMemoryCache"/>.</param>
    /// <param name="sizeCalculator">A function that calculates the size of a new cache item, which some backends may use to evict.</param>
    public MemoryCacheBackend( [Required] IMemoryCache cache, Func<PSCacheItem, long>? sizeCalculator )
    {
        this._cache = cache;
        this._sizeCalculator = sizeCalculator;
    }

    private static string GetItemKey( string key )
    {
        return nameof(MemoryCacheBackend) + ":item:" + key;
    }

    private static string GetDependencyKey( string key )
    {
        return nameof(MemoryCacheBackend) + ":dependency:" + key;
    }

    private static CacheItemRemovedReason CreateRemovalReason( EvictionReason sourceReason )
    {
        switch ( sourceReason )
        {
            case EvictionReason.None:
            case EvictionReason.Replaced:
                return CacheItemRemovedReason.Other;

            case EvictionReason.TokenExpired:
                return CacheItemRemovedReason.Invalidated;

            case EvictionReason.Capacity:
                return CacheItemRemovedReason.Evicted;

            case EvictionReason.Expired:
                return CacheItemRemovedReason.Expired;

            case EvictionReason.Removed:
                return CacheItemRemovedReason.Removed;

            default:
                throw new ArgumentException( string.Format( CultureInfo.InvariantCulture, "The CacheEntryRemovedReason '{0}' is unknown.", sourceReason ) );
        }
    }

    private MemoryCacheEntryOptions CreatePolicy( PSCacheItem item )
    {
        var targetPolicy = new MemoryCacheEntryOptions();
        targetPolicy.RegisterPostEvictionCallback( this.OnCacheItemRemoved );

        if ( item.Configuration != null )
        {
            if ( item.Configuration.AbsoluteExpiration.HasValue )
            {
                targetPolicy.AbsoluteExpirationRelativeToNow = item.Configuration.AbsoluteExpiration.Value;
            }

            if ( item.Configuration.SlidingExpiration.HasValue )
            {
                targetPolicy.SlidingExpiration = item.Configuration.SlidingExpiration.Value;
            }

            switch ( item.Configuration.Priority.GetValueOrDefault() )
            {
                case PSCacheItemPriority.Default:
                    targetPolicy.Priority = CacheItemPriority.Normal;

                    break;

                case PSCacheItemPriority.Low:
                    targetPolicy.Priority = CacheItemPriority.Low;

                    break;

                case PSCacheItemPriority.High:
                    targetPolicy.Priority = CacheItemPriority.High;

                    break;

                case PSCacheItemPriority.NotRemovable:
                    targetPolicy.Priority = CacheItemPriority.NeverRemove;

                    break;

                default:
                    throw new NotSupportedException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The priority '{0}' is not supported by the MemoryCache back-end.",
                            item.Configuration.Priority ) );
            }
        }

        if ( this._sizeCalculator != null )
        {
            targetPolicy.Size = this._sizeCalculator( item );
        }

        return targetPolicy;
    }

    private void OnCacheItemRemoved( object keyAsObject, object value, EvictionReason reason, object state )
    {
        if ( reason is EvictionReason.Removed or EvictionReason.Replaced )
        {
            // In this case, all actions are taken by the method that removes the item.
            return;
        }

        var prefix = GetItemKey( "" );
        var fullKey = (string) keyAsObject;

        if ( fullKey.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
        {
            var key = fullKey.Substring( prefix.Length );

            var item = (CacheValue) value;
            this.CleanDependencies( key, item );
            this.OnItemRemoved( key, CreateRemovalReason( reason ), this.Id );
        }
    }

    private void AddDependencies( string key, IImmutableList<string>? dependencies )
    {
        if ( dependencies is not { Count: > 0 } )
        {
            return;
        }

        foreach ( var dependency in dependencies )
        {
            var dependencyKey = GetDependencyKey( dependency );

            var backwardDependencies = (HashSet<string>?) this._cache.Get( dependencyKey );

            if ( backwardDependencies == null )
            {
                HashSet<string> newHashSet = new();

                backwardDependencies = this._cache.GetOrCreate(
                    dependencyKey,
                    ( createdEntry ) =>
                    {
                        createdEntry.Priority = CacheItemPriority.NeverRemove;
                        createdEntry.Size = 0;

                        return newHashSet;
                    } );
            }

            lock ( backwardDependencies )
            {
                backwardDependencies.Add( key );

                // The invalidation callback may have removed the key.
                this._cache.GetOrCreate(
                    dependencyKey,
                    ( createdEntry ) =>
                    {
                        createdEntry.Priority = CacheItemPriority.NeverRemove;
                        createdEntry.Size = 0;

                        return backwardDependencies;
                    } );
            }
        }
    }

    /// <inheritdoc />
    protected override void SetItemCore( string key, PSCacheItem item )
    {
        var itemKey = GetItemKey( key );
        var lockTaken = false;
        var previousValue = (MemoryCacheValue?) this._cache.Get( itemKey );
        
        try
        {
            if ( previousValue != null )
            {
                Monitor.Enter( previousValue.Sync, ref lockTaken );
                this.CleanDependencies( key, previousValue );
            }

            if ( item.Dependencies is { Count: > 0 } )
            {
                this.AddDependencies( key, item.Dependencies );
            }

            this._cache.Set(
                itemKey,
                new MemoryCacheValue( item.Value, item.Dependencies, previousValue?.Sync ?? new object() ),
                this.CreatePolicy( item ) );
        }
        finally
        {
            if ( lockTaken )
            {
                Monitor.Exit( previousValue!.Sync );
            }
        }
    }

    /// <inheritdoc />
    protected override bool ContainsItemCore( string key )
    {
        return this._cache.Get( GetItemKey( key ) ) != null;
    }

    /// <inheritdoc />  
    protected override CacheValue GetItemCore( string key, bool includeDependencies )
    {
        return (CacheValue) this._cache.Get( GetItemKey( key ) );
    }

    /// <inheritdoc />
    protected override void InvalidateDependencyCore( string key )
    {
        var items = (HashSet<string>?) this._cache.Get( GetDependencyKey( key ) );

        if ( items != null )
        {
            lock ( items )
            {
                foreach ( var item in items.ToList() )
                {
                    if ( this.RemoveItemImpl( item ) )
                    {
                        this.OnItemRemoved( item, CacheItemRemovedReason.Invalidated, this.Id );
                    }
                }

                // A side effect of calling RemoveItems is to remove the dependency entry so
                // we don't have to do it a second time.
            }
        }

        this.OnDependencyInvalidated( key, this.Id );
    }

    internal bool RemoveItemImpl( string key )
    {
        var itemKey = GetItemKey( key );

        var cacheValue = (MemoryCacheValue?) this._cache.Get( itemKey );

        if ( cacheValue == null )
        {
            return false;
        }

        lock ( cacheValue.Sync )
        {
            this._cache.Remove( itemKey );
            this.CleanDependencies( key, cacheValue );
        }

        return true;
    }

    private void CleanDependencies( string key, CacheValue cacheValue )
    {
        if ( cacheValue.Dependencies == null )
        {
            return;
        }

        foreach ( var dependency in cacheValue.Dependencies )
        {
            var dependencyKey = GetDependencyKey( dependency );
            var backwardDependencies = (HashSet<string>?) this._cache.Get( dependencyKey );

            if ( backwardDependencies == null )
            {
                continue;
            }

            lock ( backwardDependencies )
            {
                backwardDependencies.Remove( key );

                if ( backwardDependencies.Count == 0 )
                {
                    this._cache.Remove( dependencyKey );
                }
            }
        }
    }

    /// <inheritdoc />
    protected override bool ContainsDependencyCore( string key )
    {
        return this._cache.Get( GetDependencyKey( key ) ) != null;
    }

    /// <inheritdoc />
    protected override void ClearCore()
    {
        if ( this._cache is MemoryCache classicMemoryCache )
        {
            classicMemoryCache.Compact( 1 );
        }
        else
        {
            throw new NotSupportedException( "IMemoryCache implementations other than MemoryCache don't support clearing." );
        }
    }

    /// <inheritdoc />
    protected override void RemoveItemCore( string key )
    {
        if ( this.RemoveItemImpl( key ) )
        {
            this.OnItemRemoved( key, CacheItemRemovedReason.Removed, this.Id );
        }
    }

    /// <inheritdoc />
    protected override CachingBackendFeatures CreateFeatures()
    {
        return new Features( this._cache is MemoryCache );
    }

    private class Features : CachingBackendFeatures
    {
        public Features( bool supportsClear )
        {
            this.Clear = supportsClear;
        }

        public override bool Clear { get; }
    }
}