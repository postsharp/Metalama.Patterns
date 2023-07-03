// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Concurrent;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching;

internal sealed class AutoReloadManager
{
    private readonly CachingBackend _backend;
    private readonly ConcurrentDictionary<string, AutoRefreshInfo> _autoRefreshInfos = new();
    private readonly BackgroundTaskScheduler _backgroundTaskScheduler = new();

    private int _autoRefreshSubscriptions;

    public AutoReloadManager( CachingBackend backend )
    {
        this._backend = backend;
    }

    private void BeginAutoRefreshValue( object? sender, CacheItemRemovedEventArgs args )
    {
        var key = args.Key;

        if ( this._autoRefreshInfos.TryGetValue( key, out var autoRefreshInfo ) )
        {
            if ( autoRefreshInfo.IsAsync )
            {
                this._backgroundTaskScheduler.EnqueueBackgroundTask( () => AutoRefreshCoreAsync( key, autoRefreshInfo, CancellationToken.None ) );
            }
            else
            {
                this._backgroundTaskScheduler.EnqueueBackgroundTask( () => Task.Run( () => AutoRefreshCore( key, autoRefreshInfo ) ) );
            }
        }
    }

    internal void SubscribeAutoRefresh(
        string key,
        Type valueType,
        IRunTimeCacheItemConfiguration configuration,
        Func<object?> valueProvider,
        LogSource logger,
        bool isAsync )
    {
        if ( !this._backend.SupportedFeatures.Events )
        {
            logger.Warning.Write( Formatted( "The backend {Backend} does not support auto-refresh.", this._backend ) );

            return;
        }

        // TODO: We may want to preemptively renew the cache item before it gets removed, otherwise there could be latency.

        this._autoRefreshInfos.GetOrAdd(
            key,
            _ =>
            {
                if ( Interlocked.Increment( ref this._autoRefreshSubscriptions ) == 1 )
                {
                    this._backend.ItemRemoved += this.BeginAutoRefreshValue;
                }

                return new AutoRefreshInfo( configuration, valueType, valueProvider, logger, isAsync );
            } );

        // NOTE: We never remove things from autoRefreshInfos. AutoRefresh keys are there forever, they are never evicted.
    }

    private static void AutoRefreshCore( string key, AutoRefreshInfo info )
    {
        using ( var activity = info.Logger.Default.OpenActivity( Formatted( "Auto-refreshing: {Key}", key ) ) )
        {
            try
            {
                using ( var context = CachingContext.OpenCacheContext( key ) )
                {
                    var value = info.ValueProvider.Invoke();

                    CachingFrontend.SetItem( key, value, info.ReturnType, info.Configuration, context );
                }

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );
            }
        }
    }

    private static async Task AutoRefreshCoreAsync( string key, AutoRefreshInfo info, CancellationToken cancellationToken )
    {
        using ( var activity = info.Logger.Default.OpenActivity( Formatted( "Auto-refreshing: {Key}", key ) ) )
        {
            try
            {
                using ( var context = CachingContext.OpenCacheContext( key ) )
                {
                    var invokeValueProviderTask = (Task<object?>?) info.ValueProvider.Invoke();
                    var value = invokeValueProviderTask == null ? null : await invokeValueProviderTask;

                    await CachingFrontend.SetItemAsync( key, value, info.ReturnType, info.Configuration, context, cancellationToken );
                }

                activity.SetSuccess();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch ( Exception e )
#pragma warning restore CA1031 // Do not catch general exception types
            {
                activity.SetException( e );
            }
        }
    }

    private sealed record AutoRefreshInfo( IRunTimeCacheItemConfiguration Configuration, Type ReturnType, Func<object?> ValueProvider, LogSource Logger, bool IsAsync ); 
}