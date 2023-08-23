// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Locking;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Implementation;

[EditorBrowsable( EditorBrowsableState.Never )]
internal static class CachingFrontend
{
    public static object? GetOrAdd(
        MethodInfo method,
        string key,
        Type valueType,
        ICacheItemConfiguration configuration,
        Func<object?, object?[], object?> invokeOriginalMethod,
        object? instance,
        object?[] args,
        LogSource logger )
    {
        ILockHandle? lockHandle = null;
        CacheValue? item = null;
        var backend = CachingServices.DefaultBackend;

        var profile = CachingServices.Profiles[configuration.ProfileName ?? CachingProfile.DefaultName];

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
                    var valueAdapter = backend.ValueAdapters.Get( valueType );

                    if ( valueAdapter != null )
                    {
                        var unwrappedValue = valueAdapter.GetExposedValue( item.Value );
                        item = item with { Value = unwrappedValue };
                    }
                }
            }
            else
            {
                // When we recache, we have to acquire the lock without doing a cache lookup.
                lockHandle = profile.LockManager.GetLock( key );

                if ( !lockHandle.Acquire( profile.AcquireLockTimeout, CancellationToken.None ) )
                {
                    // Time out condition.
                    profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                }
            }

            object? value;

            if ( item == null )
            {
#if DEBUG

                // At this point, we assume we own the lock.
                if ( lockHandle == null )
                {
                    throw new CachingAssertionFailedException();
                }
#endif

                // Cache miss.
                if ( configuration.AutoReload.GetValueOrDefault() )
                {
                    var valueProvider = () => invokeOriginalMethod( instance, args );
                    backend.AutoReloadManager.SubscribeAutoRefresh( key, valueType, configuration, valueProvider, logger, false );
                }

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache miss: Key=\"{Key}\".", key ) );

                using ( var context = CachingContext.OpenCacheContext( key ) )
                {
                    value = invokeOriginalMethod( instance, args );

                    value = SetItem( key, value, valueType, configuration, context );

                    context.AddDependenciesToParent( method );
                }
            }
            else
            {
                // Cache hit.

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache hit: Key=\"{Key}\".", key ) );

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

    public static async Task<object?> GetOrAddAsync(
        MethodInfo method,
        string key,
        Type valueType,
        ICacheItemConfiguration configuration,
        Func<object?, object?[], Task<object?>> invokeOriginalMethod,
        object? instance,
        object?[] args,
        LogSource logger,
        CancellationToken cancellationToken )
    {
        // Keep any changes in logic in sync with other overloads of GetOrAddAsync.

        var backend = CachingServices.DefaultBackend;
        CacheValue? item = null;
        ILockHandle? lockHandle = null;

        var profile = CachingServices.Profiles[configuration.ProfileName ?? CachingProfile.DefaultName];

        try
        {
            if ( (CachingContext.Current.Kind & CachingContextKind.Recache) == 0 )
            {
                item = await backend.GetItemAsync( key, cancellationToken: cancellationToken );

                if ( item == null )
                {
                    // The item was not found in the cache, so we have to acquire a lock.
                    lockHandle = profile.LockManager.GetLock( key );

                    if ( await lockHandle.AcquireAsync( TimeSpan.Zero, CancellationToken.None ) )
                    {
                        // The lock was acquired without waiting so we are sure to be the first reader and we don't need to
                        // do a second cache lookup.
                    }
                    else if ( await lockHandle.AcquireAsync( profile.AcquireLockTimeout, CancellationToken.None ) )
                    {
                        // We had to wait to acquire the lock, so another reader may have put the item in the cache.
                        // Do a second cache lookup.

                        item = await backend.GetItemAsync( key, cancellationToken: cancellationToken );
                    }
                    else
                    {
                        // Time out condition.
                        profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                    }
                }

                if ( item != null )
                {
                    var valueAdapter = backend.ValueAdapters.Get( valueType );

                    if ( valueAdapter != null )
                    {
                        var unwrappedValue = valueAdapter.GetExposedValue( item.Value );
                        item = item with { Value = unwrappedValue };
                    }
                }
            }
            else
            {
                // When we recache, we have to acquire the lock without doing a cache lookup.
                lockHandle = profile.LockManager.GetLock( key );

                if ( !await lockHandle.AcquireAsync( profile.AcquireLockTimeout, CancellationToken.None ) )
                {
                    // Time out condition.
                    profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                }
            }

            object? value;

            if ( item == null )
            {
                // Cache miss.

#if DEBUG

                // At this point, we assume we own the lock.
                if ( lockHandle == null )
                {
                    throw new CachingAssertionFailedException();
                }
#endif

                if ( configuration.AutoReload.GetValueOrDefault() )
                {
                    var valueProvider = () => invokeOriginalMethod( instance, args );
                    backend.AutoReloadManager.SubscribeAutoRefresh( key, valueType, configuration, valueProvider, logger, true );
                }

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache miss: Key=\"{Key}\".", key ) );

                using ( var context = CachingContext.OpenCacheContext( key ) )
                {
                    var invokeValueProviderTask = invokeOriginalMethod( instance, args );

                    value = await invokeValueProviderTask;

                    var invokeSetItemAsyncTask = SetItemAsync( key, value, valueType, configuration, context, cancellationToken );

                    value = await invokeSetItemAsyncTask;

                    context.AddDependenciesToParent( method );
                }
            }
            else
            {
                // Cache hit.

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache hit: Key=\"{Key}\".", key ) );

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

    public static async Task<object?> GetOrAddAsync(
        MethodInfo method,
        string key,
        Type valueType,
        ICacheItemConfiguration configuration,
        Func<object?, object?[], ValueTask<object?>> invokeOriginalMethod,
        object? instance,
        object?[] args,
        LogSource logger,
        CancellationToken cancellationToken )
    {
        // Keep any changes in logic in sync with other overloads of GetOrAddAsync.
        // TODO: Ideally we'd avoid allocating Task here (ie, use ValueTask), at least in the cache hit path, but this is non-trivial given the extensible API which uses Task widely.       

        var backend = CachingServices.DefaultBackend;
        CacheValue? item = null;
        ILockHandle? lockHandle = null;

        var profile = CachingServices.Profiles[configuration.ProfileName ?? CachingProfile.DefaultName];

        try
        {
            if ( (CachingContext.Current.Kind & CachingContextKind.Recache) == 0 )
            {
                item = await backend.GetItemAsync( key, cancellationToken: cancellationToken );

                if ( item == null )
                {
                    // The item was not found in the cache, so we have to acquire a lock.
                    lockHandle = profile.LockManager.GetLock( key );

                    if ( await lockHandle.AcquireAsync( TimeSpan.Zero, CancellationToken.None ) )
                    {
                        // The lock was acquired without waiting so we are sure to be the first reader and we don't need to
                        // do a second cache lookup.
                    }
                    else if ( await lockHandle.AcquireAsync( profile.AcquireLockTimeout, CancellationToken.None ) )
                    {
                        // We had to wait to acquire the lock, so another reader may have put the item in the cache.
                        // Do a second cache lookup.

                        item = await backend.GetItemAsync( key, cancellationToken: cancellationToken );
                    }
                    else
                    {
                        // Time out condition.
                        profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                    }
                }

                if ( item != null )
                {
                    var valueAdapter = backend.ValueAdapters.Get( valueType );

                    if ( valueAdapter != null )
                    {
                        var unwrappedValue = valueAdapter.GetExposedValue( item.Value );
                        item = item with { Value = unwrappedValue };
                    }
                }
            }
            else
            {
                // When we recache, we have to acquire the lock without doing a cache lookup.
                lockHandle = profile.LockManager.GetLock( key );

                if ( !await lockHandle.AcquireAsync( profile.AcquireLockTimeout, CancellationToken.None ) )
                {
                    // Time out condition.
                    profile.AcquireLockTimeoutStrategy.OnTimeout( key );
                }
            }

            object? value;

            if ( item == null )
            {
                // Cache miss.

#if DEBUG

                // At this point, we assume we own the lock.
                if ( lockHandle == null )
                {
                    throw new CachingAssertionFailedException();
                }
#endif

                if ( configuration.AutoReload.GetValueOrDefault() )
                {
                    var valueProvider = () => invokeOriginalMethod( instance, args ).AsTask();
                    backend.AutoReloadManager.SubscribeAutoRefresh( key, valueType, configuration, valueProvider, logger, true );
                }

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache miss: Key=\"{Key}\".", key ) );

                using ( var context = CachingContext.OpenCacheContext( key ) )
                {
                    var invokeValueProviderTask = invokeOriginalMethod( instance, args );

                    value = await invokeValueProviderTask;

                    var invokeSetItemAsyncTask = SetItemAsync( key, value, valueType, configuration, context, cancellationToken );

                    value = await invokeSetItemAsyncTask;

                    context.AddDependenciesToParent( method );
                }
            }
            else
            {
                // Cache hit.

                logger.Debug.EnabledOrNull?.Write( Formatted( "Cache hit: Key=\"{Key}\".", key ) );

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

    public static object? SetItem( string key, object? value, Type valueType, ICacheItemConfiguration configuration, CachingContext context )
    {
        var exposedValue = value;

        if ( value != null )
        {
            var valueAdapter = CachingServices.DefaultBackend.ValueAdapters.Get( valueType );

            if ( valueAdapter != null )
            {
                value = valueAdapter.GetStoredValue( value );
                exposedValue = valueAdapter.GetExposedValue( value );
            }
        }

        var cacheItem = new CacheItem( value, context.Dependencies.ToImmutableList(), configuration );

        CachingServices.DefaultBackend.SetItem( key, cacheItem );

        return exposedValue;
    }

    public static async Task<object?> SetItemAsync(
        string key,
        object? value,
        Type valueType,
        ICacheItemConfiguration configuration,
        CachingContext context,
        CancellationToken cancellationToken )
    {
        var exposedValue = value;

        if ( value != null )
        {
            var valueAdapter = CachingServices.DefaultBackend.ValueAdapters.Get( valueType );

            if ( valueAdapter != null )
            {
                if ( valueAdapter.IsAsyncSupported )
                {
                    var getStoredValueTask = valueAdapter.GetStoredValueAsync( value, cancellationToken );
                    value = await getStoredValueTask;
                }
                else
                {
                    // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                    value = valueAdapter.GetStoredValue( value );
                }

                exposedValue = valueAdapter.GetExposedValue( value );
            }
        }

        var cacheItem = new CacheItem( value, context.Dependencies.ToImmutableList(), configuration );

        var setItemTask = CachingServices.DefaultBackend.SetItemAsync( key, cacheItem, cancellationToken );
        await setItemTask;

        return exposedValue;
    }
}