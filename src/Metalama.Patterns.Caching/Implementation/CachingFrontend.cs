// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Locking;
using Metalama.Patterns.Caching.ValueAdapters;
using System.Collections.Immutable;
using System.Reflection;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Implementation
{
    internal static class CachingFrontend
    {

        internal static object GetOrAdd( MethodInfo method, string key, Type valueType, CacheItemConfiguration configuration, Func<object> valueProvider,
                                         LogSource logger )
        {
            ILockHandle lockHandle = null;
            CacheValue item = null;
            CachingBackend backend = CachingServices.DefaultBackend;

            CachingProfile profile = CachingServices.Profiles[configuration.ProfileName];

            try
            {


                if ( (CachingContext.Current.Kind & CachingContextKind.Recache) == 0 )
                {

                    item = backend.GetItem( key );

                    if ( item == null )
                    {
                        // The item was not found in the cache, so we have to acquire a lock.
                        lockHandle = profile.LockManager.GetLock( key );

                        if ( lockHandle.Acquire( TimeSpan.Zero, CancellationToken.None ) )
                        {
                            // The lock was acquired without waiting so we are sure to be the first reader and we don't need to
                            // do a second cache lookup.
                        } 
                        else if ( lockHandle.Acquire( profile.AcquireLockTimeout, CancellationToken.None ) )
                        {
                            // We had to wait to acquire the lock, so another reader may have put the item in the cache.
                            // Do a second cache lookup.

                            item = backend.GetItem( key );
                        }
                        else
                        {
                            // Time out condition.
                            profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                            
                        }
                    }


                    if ( item != null )
                    {
                        IValueAdapter valueAdapter = backend.ValueAdapters.Get( valueType );
                        if ( valueAdapter != null )
                        {
                            object unwrappedValue = valueAdapter.GetExposedValue( item.Value );
                            item = item.WithValue( unwrappedValue );
                        }
                    }

                }
                else
                {
                    // When we recache, we have to acquire the lock without doing a cache lookup.
                    lockHandle = profile.LockManager.GetLock(key);

                    if ( !lockHandle.Acquire( profile.AcquireLockTimeout, CancellationToken.None ) )
                    {
                        // Time out condition.
                        profile.AcquireLockTimeoutStrategy.OnTimeout(key);
                    }

                }

                object value;
                if ( item == null )
                {


#if DEBUG
                    // At this point, we assume we own the lock.
                    if ( lockHandle == null )
                        throw new MetalamaPatternsCachingAssertionFailedException();
#endif

                    // Cache miss.
                    if ( configuration.AutoReload.GetValueOrDefault() )
                    {
                        backend.AutoReloadManager.SubscribeAutoRefresh( key, valueType, configuration, valueProvider, logger, false );
                    }

                    logger.Debug.EnabledOrNull?.Write( Formatted("Cache miss: Key=\"{Key}\".", key) );

                    using ( CachingContext context = CachingContext.OpenCacheContext( key ) )
                    {

                        value = valueProvider.Invoke();

                        value = SetItem( key, value, valueType, configuration, context );

                        context.AddDependenciesToParent( method );

                    }
                }
                else
                {
                    // Cache hit.

                    logger.Debug.EnabledOrNull?.Write( Formatted( "Cache hit: Key=\"{Key}\".", key  ));

                    AddCacheHitDependencies( key, item );

                    value = item.Value;
                }

                return value;
            }
            finally
            {
                if ( lockHandle != null )
                {
                    lockHandle.Release();
                    lockHandle.Dispose();
                }
            }
        }

        internal static async Task<object> GetOrAddAsync( MethodInfo method, string key, Type valueType, CacheItemConfiguration configuration,
                                                          Func<Task<object>> valueProvider, LogSource logger, CancellationToken cancellationToken )
        {
            CachingBackend backend = CachingServices.DefaultBackend;
            CacheValue item = null;
            ILockHandle lockHandle = null;

            CachingProfile profile = CachingServices.Profiles[configuration.ProfileName];

            try
            {

                if ( (CachingContext.Current.Kind & CachingContextKind.Recache) == 0 )
                {
                
                    item = await backend.GetItemAsync( key, cancellationToken: cancellationToken );

                    if (item == null)
                    {
                        // The item was not found in the cache, so we have to acquire a lock.
                        lockHandle = profile.LockManager.GetLock(key);

                        if (await lockHandle.AcquireAsync(TimeSpan.Zero, CancellationToken.None))
                        {
                            // The lock was acquired without waiting so we are sure to be the first reader and we don't need to
                            // do a second cache lookup.
                        }
                        else if (await lockHandle.AcquireAsync( profile.AcquireLockTimeout, CancellationToken.None))
                        {
                            // We had to wait to acquire the lock, so another reader may have put the item in the cache.
                            // Do a second cache lookup.

                            item = await backend.GetItemAsync(key, cancellationToken: cancellationToken);
                        }
                        else
                        {
                            // Time out condition.
                            profile.AcquireLockTimeoutStrategy.OnTimeout(key);

                        }
                    }

                    if ( item != null )
                    {
                        IValueAdapter valueAdapter = backend.ValueAdapters.Get( valueType );
                        if ( valueAdapter != null )
                        {
                            object unwrappedValue = valueAdapter.GetExposedValue( item.Value );
                            item = item.WithValue( unwrappedValue );
                        }
                    }

                }
                else
                {
                    // When we recache, we have to acquire the lock without doing a cache lookup.
                    lockHandle = profile.LockManager.GetLock(key);

                    if (!await lockHandle.AcquireAsync(profile.AcquireLockTimeout, CancellationToken.None))
                    {
                        // Time out condition.
                        profile.AcquireLockTimeoutStrategy.OnTimeout(key);
                    }

                }

                object value;
                if ( item == null )
                {
                    // Cache miss.

#if DEBUG
                    // At this point, we assume we own the lock.
                    if (lockHandle == null)
                        throw new MetalamaPatternsCachingAssertionFailedException();
#endif


                    if ( configuration.AutoReload.GetValueOrDefault() )
                    {
                        backend.AutoReloadManager.SubscribeAutoRefresh( key, valueType, configuration, valueProvider, logger, true );
                    }

                    logger.Debug.EnabledOrNull?.Write( Formatted(  "Cache miss: Key=\"{Key}\".", key ) );

                    using ( CachingContext context = CachingContext.OpenCacheContext( key ) )
                    {

                        Task<object> invokeValueProviderTask = valueProvider.Invoke();

                        value = await invokeValueProviderTask;


                        Task<object> invokeSetItemAsyncTask = SetItemAsync(key, value, valueType, configuration, context, cancellationToken);

                        value = await invokeSetItemAsyncTask;

                        context.AddDependenciesToParent( method );

                    }
                }
                else
                {
                    // Cache hit.

                    logger.Debug.EnabledOrNull?.Write( Formatted("Cache hit: Key=\"{Key}\".", key) );

                    AddCacheHitDependencies( key, item );

                    value = item.Value;
                }

                return value;
            }
            finally
            {
                if ( lockHandle != null )
                {
                    await lockHandle.ReleaseAsync();
                    lockHandle.Dispose();
                }
            }
        }

        private static void AddCacheHitDependencies( string key, CacheValue item )
        {
            CachingContext.Current.AddDependencies( item.Dependencies );

            if ( CachingServices.DefaultBackend.SupportedFeatures.Dependencies )
            {
                CachingContext.Current.AddDependency( key );
            }
        }


        internal static object SetItem( string key, object value, Type valueType, CacheItemConfiguration configuration, CachingContext context )
        {
            object exposedValue = value;

            if ( value != null )
            {
                IValueAdapter valueAdapter = CachingServices.DefaultBackend.ValueAdapters.Get( valueType );
                if ( valueAdapter != null )
                {
                    value = valueAdapter.GetStoredValue( value );
                    exposedValue = valueAdapter.GetExposedValue( value );
                }
            }

            CacheItem cacheItem = new CacheItem( value, context?.Dependencies?.ToImmutableList(), configuration );

            CachingServices.DefaultBackend.SetItem( key, cacheItem );

            return exposedValue;
        }



        internal static async Task<object> SetItemAsync(string key, object value, Type valueType, CacheItemConfiguration configuration, CachingContext context,
            CancellationToken cancellationToken)
        {
            object exposedValue = value;

            if ( value != null )
            {
                IValueAdapter valueAdapter = CachingServices.DefaultBackend.ValueAdapters.Get( valueType );
                if ( valueAdapter != null )
                {
                    if ( valueAdapter.IsAsyncSupported )
                    {
                        Task<object> getStoredValueTask = valueAdapter.GetStoredValueAsync( value, cancellationToken );
                        value = await getStoredValueTask;
                    }
                    else
                    {
                        value = valueAdapter.GetStoredValue( value );
                    }

                    exposedValue = valueAdapter.GetExposedValue( value );
                }
            }

            CacheItem cacheItem = new CacheItem( value, context?.Dependencies?.ToImmutableList(), configuration );

            Task setItemTask = CachingServices.DefaultBackend.SetItemAsync( key, cacheItem, cancellationToken );
            await setItemTask;

            return exposedValue;
        }
    }
}
