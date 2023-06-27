// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;

namespace PostSharp.Patterns.Caching.Backends
{
    /// <summary>
    /// A <see cref="CachingBackendEnhancer"/> that modifies all write operations to run in the background and
    /// immediately return to the caller.
    /// </summary>
    public class NonBlockingCachingBackendEnhancer : CachingBackendEnhancer
    {
        private static readonly Task<bool> finishedTask = Task.FromResult( true );
        BackgroundTaskScheduler taskScheduler = new BackgroundTaskScheduler(true);

        /// <inheritdoc />
        protected override CachingBackendFeatures CreateFeatures()
        {
            return new Features( this.UnderlyingBackend.SupportedFeatures );
        }

        /// <inheritdoc />
        public NonBlockingCachingBackendEnhancer(CachingBackend underlyingBackend) : base(underlyingBackend)
        {
        }

        private void EnqueueBackgroundTask( Func<Task> task)
        {
            this.taskScheduler.EnqueueBackgroundTask( task );
        }


        /// <inheritdoc />
        protected override void ClearCore()
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.ClearAsync() );
        }

        /// <inheritdoc />
        protected override void InvalidateDependencyCore( string key )
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.InvalidateDependencyAsync( key ) );
        }

        /// <inheritdoc />
        protected override void SetItemCore( string key, CacheItem item )
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.SetItemAsync( key, item ) );
        }

        /// <inheritdoc />
        protected override void RemoveItemCore( string key )
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.RemoveItemAsync( key ) );
        }

        /// <inheritdoc />
        protected override Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
        {
            this.EnqueueBackgroundTask( () => this.UnderlyingBackend.SetItemAsync( key, item, cancellationToken ) );
            return finishedTask;
        }

        /// <inheritdoc />
        protected override Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
             this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.InvalidateDependencyAsync( key, cancellationToken ) );
            return finishedTask;
        }

        /// <inheritdoc />
        protected override Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.RemoveItemAsync( key, cancellationToken ));
            return finishedTask;
        }

        /// <inheritdoc />
        protected override Task ClearAsyncCore( CancellationToken cancellationToken )
        {
            this.EnqueueBackgroundTask(  () => this.UnderlyingBackend.ClearAsync( cancellationToken ) );
            return finishedTask;
        }

        /// <inheritdoc />
        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            await this.taskScheduler.DisposeAsync( cancellationToken );
            await base.DisposeAsyncCore( cancellationToken );
        }

        /// <inheritdoc />
        protected override void DisposeCore( bool disposing )
        {
            this.taskScheduler.Dispose();
            base.DisposeCore( disposing );
        }

        private class Features : CachingBackendEnhancerFeatures
        {
            public Features( CachingBackendFeatures underlyingBackendFeatures ) : base( underlyingBackendFeatures )
            {
            }

            public override bool Blocking => false;
        }
    }

}
