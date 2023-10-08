// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends;

/// <summary>
/// A <see cref="CachingBackendEnhancer"/> that modifies all write operations to run in the background and
/// immediately return to the caller.
/// </summary>
[PublicAPI]
public class NonBlockingCachingBackendEnhancer : CachingBackendEnhancer
{
    private static readonly ValueTask _finishedTask = new( Task.CompletedTask );
    private readonly BackgroundTaskScheduler _taskScheduler;

    /// <inheritdoc />
    protected override CachingBackendFeatures CreateFeatures() => new Features( this.UnderlyingBackend.SupportedFeatures );

    /// <summary>
    /// Initializes a new instance of the <see cref="NonBlockingCachingBackendEnhancer"/> class.
    /// </summary>
    public NonBlockingCachingBackendEnhancer( CachingBackend underlyingBackend ) : base( underlyingBackend )
    {
        this._taskScheduler = new BackgroundTaskScheduler( underlyingBackend.ServiceProvider, true );
    }

    private void EnqueueBackgroundTask( Func<Task> task ) => this._taskScheduler.EnqueueBackgroundTask( task );

    /// <inheritdoc />
    protected override void ClearCore() => this.EnqueueBackgroundTask( () => this.UnderlyingBackend.ClearAsync().AsTask() );

    /// <inheritdoc />
    protected override void InvalidateDependencyCore( string key )
        => this.EnqueueBackgroundTask( () => this.UnderlyingBackend.InvalidateDependencyAsync( key ).AsTask() );

    /// <inheritdoc />
    protected override void SetItemCore( string key, CacheItem item )
        => this.EnqueueBackgroundTask( () => this.UnderlyingBackend.SetItemAsync( key, item ).AsTask() );

    /// <inheritdoc />
    protected override void RemoveItemCore( string key ) => this.EnqueueBackgroundTask( () => this.UnderlyingBackend.RemoveItemAsync( key ).AsTask() );

    /// <inheritdoc />
    protected override ValueTask SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
    {
        this.EnqueueBackgroundTask( () => this.UnderlyingBackend.SetItemAsync( key, item, cancellationToken ).AsTask() );

        return _finishedTask;
    }

    /// <inheritdoc />
    protected override ValueTask InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
    {
        this.EnqueueBackgroundTask( () => this.UnderlyingBackend.InvalidateDependencyAsync( key, cancellationToken ).AsTask() );

        return _finishedTask;
    }

    /// <inheritdoc />
    protected override ValueTask RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
    {
        this.EnqueueBackgroundTask( () => this.UnderlyingBackend.RemoveItemAsync( key, cancellationToken ).AsTask() );

        return _finishedTask;
    }

    /// <inheritdoc />
    protected override ValueTask ClearAsyncCore( CancellationToken cancellationToken )
    {
        this.EnqueueBackgroundTask( () => this.UnderlyingBackend.ClearAsync( cancellationToken ).AsTask() );

        return _finishedTask;
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore( CancellationToken cancellationToken )
    {
        await this._taskScheduler.DisposeAsync( cancellationToken );
        await base.DisposeAsyncCore( cancellationToken );
    }

    /// <inheritdoc />
    protected override void DisposeCore( bool disposing )
    {
        this._taskScheduler.Dispose();
        base.DisposeCore( disposing );
    }

    private class Features : CachingBackendEnhancerFeatures
    {
        public Features( CachingBackendFeatures underlyingBackendFeatures ) : base( underlyingBackendFeatures ) { }

        public override bool Blocking => false;
    }
}