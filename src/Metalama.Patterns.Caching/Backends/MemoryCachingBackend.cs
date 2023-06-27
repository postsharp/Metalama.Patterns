// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.Caching;
using PostSharp.Patterns.Caching.Implementation;
using PSCacheItem = PostSharp.Patterns.Caching.Implementation.CacheItem;
using MemoryCacheItemPolicy = System.Runtime.Caching.CacheItemPolicy;
using PSCacheItemPriority = PostSharp.Patterns.Caching.Implementation.CacheItemPriority;
using System.Linq;
using System.Threading;

namespace PostSharp.Patterns.Caching.Backends
{
    /// <summary>
    /// A <see cref="CachingBackend"/> based on <c>System.Runtime.Caching.MemoryCache</c> (<see cref="MemoryCache"/>). This cache is part of .NET Framework
    /// and is available for .NET Standard in the NuGet package System.Runtime.Caching.
    /// </summary>
    public sealed class MemoryCachingBackend : CachingBackend
    {

        private static readonly MemoryCacheItemPolicy dependencyCacheItemPolicy = new MemoryCacheItemPolicy
                    {
                        Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable
                    };


        private readonly MemoryCache cache;

       
        private static string GetItemKey( string key )
        {
            return nameof( MemoryCachingBackend ) + ":item:" + key;
        }

        private static string GetDependencyKey( string key )
        {
            return nameof( MemoryCachingBackend ) + ":dependency:" + key;
        }

        /// <summary>
        /// Initializes a new <see cref="MemoryCachingBackend"/> based on the <see cref="MemoryCache.Default"/> instance of the <see cref="MemoryCache"/> class.
        /// </summary>
        public MemoryCachingBackend() : this(null)
        {
            
        }

        /// <summary>
        /// Initializes a new <see cref="MemoryCachingBackend"/> based on the given <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="cache">A <see cref="MemoryCache"/>, or <c>null</c> to use  the <see cref="MemoryCache.Default"/> instance of the <see cref="MemoryCache"/> class.</param>
        public MemoryCachingBackend( MemoryCache cache  )
        {
            this.cache = cache ?? MemoryCache.Default;
        }

        private static CacheItemRemovedReason CreateRemovalReason( CacheEntryRemovedReason sourceReason )
        {
            switch ( sourceReason )
            {
                case CacheEntryRemovedReason.CacheSpecificEviction:
                    return CacheItemRemovedReason.Other;
                case CacheEntryRemovedReason.ChangeMonitorChanged:
                    return CacheItemRemovedReason.Invalidated;
                case CacheEntryRemovedReason.Evicted:
                    return CacheItemRemovedReason.Evicted;
                case CacheEntryRemovedReason.Expired:
                    return CacheItemRemovedReason.Expired;
                case CacheEntryRemovedReason.Removed:
                    return CacheItemRemovedReason.Removed;
                default:
                    throw new ArgumentException( string.Format(CultureInfo.InvariantCulture, "The CacheEntryRemovedReason '{0}' is unknown.", sourceReason ) );
            }
        }

        private MemoryCacheItemPolicy CreatePolicy( PSCacheItem item )
        {
            MemoryCacheItemPolicy targetPolicy = new MemoryCacheItemPolicy
                                                 {
                                                     RemovedCallback = this.OnCacheItemRemoved
                                                 };





            if ( item.Configuration != null )
            {
                if ( item.Configuration.AbsoluteExpiration.HasValue )
                {
                    targetPolicy.AbsoluteExpiration = DateTime.Now + item.Configuration.AbsoluteExpiration.Value;
                }

                if ( item.Configuration.SlidingExpiration.HasValue )
                {
                    targetPolicy.SlidingExpiration = item.Configuration.SlidingExpiration.Value;
                }

                switch ( item.Configuration.Priority.GetValueOrDefault() )
                {
                    case PSCacheItemPriority.Default:
                    case PSCacheItemPriority.Low:
                    case PSCacheItemPriority.High:
                        targetPolicy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
                        break;
                    case PSCacheItemPriority.NotRemovable:
                        targetPolicy.Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable;
                        break;
                    default:
                        throw new NotSupportedException( string.Format(CultureInfo.InvariantCulture, "The priority '{0}' is not supported by the MemoryCache back-end.",
                                                                        item.Configuration.Priority ) );
                }


            }

            return targetPolicy;
        }

        private void OnCacheItemRemoved( CacheEntryRemovedArguments arguments )
        {
            if ( arguments.RemovedReason == CacheEntryRemovedReason.Removed )
            {
                // In this case, all actions are taken by the method that removes the item.
                return;
            }
            string prefix = GetItemKey( "" );

            if ( arguments.CacheItem.Key.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
            {

                string key = arguments.CacheItem.Key.Substring( prefix.Length );

                CacheValue item = (CacheValue) arguments.CacheItem.Value;
                this.CleanDependencies( key, item );

                this.OnItemRemoved( key, CreateRemovalReason( arguments.RemovedReason ), this.Id );
            }

        }

        private void AddDependencies( string key, IImmutableList<string> dependencies )
        {
            if ( dependencies == null || dependencies.Count <= 0 )
                return;

            foreach ( string dependency in dependencies )
            {
                string dependencyKey = GetDependencyKey( dependency );

                HashSet<string> backwardDependencies = (HashSet<string>) this.cache.Get( dependencyKey );
                if ( backwardDependencies == null )
                {
                    HashSet<string> newHashSet = new HashSet<string>();
                    backwardDependencies = (HashSet<string>) this.cache.AddOrGetExisting( dependencyKey, newHashSet, dependencyCacheItemPolicy ) ?? newHashSet;
                }

                lock ( backwardDependencies )
                {
                    backwardDependencies.Add( key );

                    // The invalidation callback may have removed the key.
                    object addOrGetExisting = this.cache.AddOrGetExisting( dependencyKey, backwardDependencies, dependencyCacheItemPolicy );
                }
            }
        }


        /// <inheritdoc />
        protected override void SetItemCore( string key, PSCacheItem item )
        {
            string itemKey = GetItemKey( key );
            bool lockTaken = false;
            MemoryCacheValue previousValue = (MemoryCacheValue) this.cache.Get( itemKey );

            try
            {
                if ( previousValue != null )
                {
                    Monitor.Enter( previousValue.Sync, ref lockTaken );
                    this.CleanDependencies( key, previousValue );
                }

                if ( item.Dependencies != null && item.Dependencies.Count > 0 )
                {
                    this.AddDependencies( key, item.Dependencies );
                }


                this.cache.Set( itemKey, new MemoryCacheValue( item.Value, item.Dependencies, previousValue?.Sync ?? new object() ),
                                this.CreatePolicy( item ) );

            }
            finally
            {
                if ( lockTaken )
                {
                    Monitor.Exit( previousValue.Sync );
                }
            }
        }

        /// <inheritdoc />
        protected override bool ContainsItemCore( string key )
        {
            return this.cache.Contains( GetItemKey( key ));
        }

        /// <inheritdoc />  
        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            return (CacheValue) this.cache.Get( GetItemKey( key ) );
        }



        /// <inheritdoc />
        protected override void InvalidateDependencyCore( string key )
        {
            this.InvalidateDependencyImpl( key );
        }

        internal void InvalidateDependencyImpl( string key, MemoryCacheValue replacementValue = null, DateTimeOffset? replacementValueExpiration = null )
        {
            HashSet<string> items = (HashSet<string>) this.cache.Get(  GetDependencyKey( key ) );

            if ( items != null )
            {
                lock ( items )
                {
                    foreach ( string item in items.ToList() )
                    {
                        if ( this.RemoveItemImpl( item, replacementValue, replacementValueExpiration ) )
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



        internal bool RemoveItemImpl( string key, MemoryCacheValue  replacementValue = null, DateTimeOffset? replacementValueExpiration = null)
        {
            string itemKey = GetItemKey( key );

            MemoryCacheValue cacheValue = (MemoryCacheValue) this.cache.Get( itemKey );

            if ( cacheValue == null )
                return false;

            lock ( cacheValue.Sync )
            {
                if ( replacementValue == null )
                {
                    cacheValue = (MemoryCacheValue) this.cache.Remove( itemKey );

                    if ( cacheValue == null )
                    {
                        // The item has been removed by another thread.
                        return false;
                    }
                }
                else
                {
                    replacementValue.Sync = cacheValue.Sync;
                    this.cache.Set( itemKey, replacementValue, replacementValueExpiration.Value );
                }

                this.CleanDependencies( key, cacheValue );
            }

            return true;

        }

        private void CleanDependencies( string key, CacheValue cacheValue )
        {
            if ( cacheValue.Dependencies == null )
                return;

            foreach ( string dependency in cacheValue.Dependencies )
            {
                string dependencyKey = GetDependencyKey( dependency );
                HashSet<string> backwardDependencies = (HashSet<string>) this.cache.Get( dependencyKey );

                if ( backwardDependencies == null )
                    continue;

                lock ( backwardDependencies )
                {
                    backwardDependencies.Remove( key );
                    if ( backwardDependencies.Count == 0 )
                    {

                            this.cache.Remove( dependencyKey);

                    }
                }
            }
        }

        /// <inheritdoc />
        protected override bool ContainsDependencyCore( string key )
        {
            return this.cache.Contains( GetDependencyKey( key ) );
        }


        /// <inheritdoc />
        protected override void ClearCore()
        {
            this.cache.Trim(100);
        }

        /// <inheritdoc />
        protected override void RemoveItemCore( string key )
        {
            if ( this.RemoveItemImpl( key ) )
            {
                this.OnItemRemoved( key, CacheItemRemovedReason.Removed, this.Id );
            }
        }



    }
}