// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Implementation
{
    /// <summary>
    /// A base class for all cache enhancers. A <see cref="CachingBackendEnhancer"/> is a node in a chain of responsibility where
    /// the last node is the physical implementation of the <see cref="CachingBackend"/>. A <see cref="CachingBackendEnhancer"/>
    /// can add behaviors to a <see cref="CachingBackend"/>.
    /// </summary>
    /// <remarks>
    /// <para>The default behavior of all methods of a <see cref="CachingBackendEnhancer"/> is to delegate the implementation
    /// to the next <see cref="CachingBackend"/> in the chain of responsibility.</para>
    /// </remarks>
    public abstract class CachingBackendEnhancer : CachingBackend
    {
        /// <summary>
        /// Initializes a new <see cref="CachingBackendEnhancer"/>.
        /// </summary>
        /// <param name="underlyingBackend">The next <see cref="CachingBackend"/> in the chain of responsibility.</param>
        protected CachingBackendEnhancer( [Required] CachingBackend underlyingBackend )
        {
            this.UnderlyingBackend = underlyingBackend;
            if ( underlyingBackend.SupportedFeatures.Events )
            {
                this.UnderlyingBackend.ItemRemoved += this.OnBackendItemRemoved;
                if ( underlyingBackend.SupportedFeatures.Dependencies )
                {
                    this.UnderlyingBackend.DependencyInvalidated += this.OnBackendDependencyInvalidated;
                }
            }
        }

        /// <summary>
        /// Method invoked when the <see cref="CachingBackend.DependencyInvalidated"/> event of the next <see cref="UnderlyingBackend"/>
        /// in the chain of responsibility is invoked.
        /// </summary>
        /// <param name="sender">The sender (typically the value of the <see cref="UnderlyingBackend"/> property).</param>
        /// <param name="args">The <see cref="CacheDependencyInvalidatedEventArgs"/>.</param>
        protected virtual void OnBackendDependencyInvalidated( object sender, CacheDependencyInvalidatedEventArgs args )
        {
            this.OnDependencyInvalidated( args );
        }

        /// <summary>
        /// Method invoked when the <see cref="CachingBackend.ItemRemoved"/> event of the next <see cref="UnderlyingBackend"/>
        /// in the chain of responsibility is invoked.
        /// </summary>
        /// <param name="sender">The sender (typically the value of the <see cref="UnderlyingBackend"/> property).</param>
        /// <param name="args">The <see cref="CacheItemRemovedEventArgs"/>.</param>
        protected virtual void OnBackendItemRemoved( object sender, CacheItemRemovedEventArgs args )
        {
            this.OnItemRemoved( args );
        }

        /// <summary>
        /// Gets the next <see cref="CachingBackend"/> in the chain of responsibility.
        /// </summary>
        public CachingBackend UnderlyingBackend { get; }

        /// <inheritdoc />
        protected override CachingBackendFeatures CreateFeatures()
        {
            return this.UnderlyingBackend.SupportedFeatures;
        }

        /// <inheritdoc />
        protected override void SetItemCore( string key, CacheItem item )
        {
            this.UnderlyingBackend.SetItem( key, item );
        }

        /// <inheritdoc />
        protected override bool ContainsItemCore( string key )
        {
            return this.UnderlyingBackend.ContainsItem( key );
        }

        /// <inheritdoc />
        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            return this.UnderlyingBackend.GetItem( key, includeDependencies );
        }

        /// <inheritdoc />
        protected override void RemoveItemCore( string key )
        {
            this.UnderlyingBackend.RemoveItem( key );
        }

        /// <inheritdoc />
        protected override void InvalidateDependencyCore( string key )
        {
            this.UnderlyingBackend.InvalidateDependency( key );
        }

        /// <inheritdoc />
        protected override bool ContainsDependencyCore( string key )
        {
            return this.UnderlyingBackend.ContainsDependency( key );
        }

        /// <inheritdoc />
        protected override void ClearCore()
        {
            this.UnderlyingBackend.Clear();
        }

        /// <inheritdoc />
        protected override Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.SetItemAsync( key, item, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.ContainsItemAsync( key, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.ContainsDependencyAsync( key, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task<CacheValue> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.GetItemAsync( key, includeDependencies, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.InvalidateDependencyAsync( key, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.RemoveItemAsync( key, cancellationToken );
        }

        /// <inheritdoc />
        protected override Task ClearAsyncCore( CancellationToken cancellationToken )
        {
            return this.UnderlyingBackend.ClearAsync( cancellationToken );
        }

        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            // It is important to dispose 'this' before local and remote caches because we need to empty the current queue before disposing
            // the remote cache.

            base.DisposeCore(disposing);

            this.UnderlyingBackend.ItemRemoved -= this.OnBackendItemRemoved;
            this.UnderlyingBackend.DependencyInvalidated -= this.OnBackendDependencyInvalidated;

            this.UnderlyingBackend.Dispose();
        }

        /// <inheritdoc />
        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            // It is important to dispose 'this' before local and remote caches because we need to empty the current queue before disposing
            // the remote cache.

            await base.DisposeAsyncCore( cancellationToken );

            this.UnderlyingBackend.ItemRemoved -= this.OnBackendItemRemoved;
            this.UnderlyingBackend.DependencyInvalidated -= this.OnBackendDependencyInvalidated;

            await this.UnderlyingBackend.DisposeAsync( cancellationToken );
        }

    }
}
