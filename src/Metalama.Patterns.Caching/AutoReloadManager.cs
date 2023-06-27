// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Diagnostics;
using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder;

namespace PostSharp.Patterns.Caching
{
    internal sealed class AutoReloadManager
    {
        private readonly CachingBackend backend;
        private int autoRefreshSubscriptions;
        private readonly ConcurrentDictionary<string, AutoRefreshInfo> autoRefreshInfos = new ConcurrentDictionary<string, AutoRefreshInfo>();
        readonly BackgroundTaskScheduler backgroundTaskScheduler = new BackgroundTaskScheduler();

        public AutoReloadManager( CachingBackend backend )
        {
            this.backend = backend;
        }

        private void BeginAutoRefreshValue( object sender, CacheItemRemovedEventArgs args )
        {
            string key = args.Key;
            AutoRefreshInfo autoRefreshInfo;
            if ( this.autoRefreshInfos.TryGetValue(key, out autoRefreshInfo))
            {
                if ( autoRefreshInfo.IsAsync )
                {
                    this.backgroundTaskScheduler.EnqueueBackgroundTask( () => AutoRefreshCoreAsync( key, autoRefreshInfo, CancellationToken.None ) );
                }
                else
                {
                    this.backgroundTaskScheduler.EnqueueBackgroundTask( () => Task.Run( () => AutoRefreshCore( key, autoRefreshInfo ) ) );
                }

            }
        }


        internal void SubscribeAutoRefresh( string key, Type valueType, CacheItemConfiguration configuration, Func<object> valueProvider, LogSource logger, bool isAsync )
        {
            if ( !this.backend.SupportedFeatures.Events )
            {
                logger.Warning.Write( Formatted("The backend {Backend} does not support auto-refresh.", this.backend) );
                return;
            }

            // TODO: We may want to preemptively renew the cache item before it gets removed, otherwise there could be latency.

            this.autoRefreshInfos.GetOrAdd( key, k =>
                                                 {
                                                     if ( Interlocked.Increment( ref this.autoRefreshSubscriptions ) == 1 )
                                                     {
                                                         this.backend.ItemRemoved += this.BeginAutoRefreshValue;
                                                     }
                                                     
                                                     return new AutoRefreshInfo( configuration, valueType, valueProvider, logger, isAsync );
                                                 } );

            // NOTE: We never remove things from autoRefreshInfos. AutoRefresh keys are there forever, they are never evicted.

        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
        private static void AutoRefreshCore( string key, AutoRefreshInfo info )
        {
            using ( var activity = info.Logger.Default.OpenActivity( Formatted("Auto-refreshing: {Key}", key )) )
            {
                try
                {
                    using ( CachingContext context = CachingContext.OpenCacheContext( key ) )
                    {

                        object value = info.ValueProvider.Invoke();

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
            using ( var activity = info.Logger.Default.OpenActivity( Formatted("Auto-refreshing: {Key}", key )) )
            {
                try
                {
                    using ( CachingContext context = CachingContext.OpenCacheContext( key ) )
                    {
                        Task<object> invokeValueProviderTask = (Task<object>) info.ValueProvider.Invoke();
                        object value = await invokeValueProviderTask;

                        await  CachingFrontend.SetItemAsync(key, value, info.ReturnType, info.Configuration, context, cancellationToken);
                    }

                    activity.SetSuccess();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch ( Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    activity.SetException( e );
                }
            }
        }

        private sealed class AutoRefreshInfo
        {
            public AutoRefreshInfo( CacheItemConfiguration configuration, Type returnType, Func<object> valueProvider, LogSource logger, bool isAsync )
            {
                this.Configuration = configuration;
                this.ReturnType = returnType;
                this.ValueProvider = valueProvider;
                this.Logger = logger;
                this.IsAsync = isAsync;
            }

            public Type ReturnType { get; }

            public Func<object> ValueProvider { get; }

            public LogSource Logger { get; }

            public bool IsAsync { get; }

            public CacheItemConfiguration Configuration { get; }
        }


    }
}
