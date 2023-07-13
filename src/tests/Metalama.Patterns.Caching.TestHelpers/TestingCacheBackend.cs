// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Runtime.Caching;
using Metalama.Patterns.Caching.Backends;
using CacheItem = Metalama.Patterns.Caching.Implementation.CacheItem;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public sealed class TestingCacheBackend : CachingBackend
    {
        private readonly CachingBackend _backend;

        // TODO: This pattern might be in a separate class

        private int _actualSetCount = 0;
        private int _actualContainsKeyCount = 0;
        private int _actualGetCount = 0;
        private int _actualRemoveCount = 0;
        private int _actualInvalidateCount = 0;

        public int ExpectedSetCount { get; set; }

        public int ExpectedContainsKeyCount { get; set; }

        public int ExpectedGetCount { get; set; }

        public int ExpectedRemoveCount { get; set; }

        public int ExpectedInvalidateCount { get; set; }

        public string? LastCachedKey { get; set; }

        public CacheItem? LastCachedItem { get; set; }

        public event CacheItemSetEventHandler ItemSet;

        protected override CachingBackendFeatures CreateFeatures() => this._backend.SupportedFeatures;

        public TestingCacheBackend( string name )
        {
            this.ResetExpectations();
            this._backend = new MemoryCachingBackend( new MemoryCache( name ) );
            this._backend.ItemRemoved += this.OnItemRemoved;
            this._backend.DependencyInvalidated += this.OnDependencyInvalidated;
        }

        private void OnDependencyInvalidated( object sender, CacheDependencyInvalidatedEventArgs args )
        {
            base.OnDependencyInvalidated( args );
        }

        private void OnItemRemoved( object sender, CacheItemRemovedEventArgs args )
        {
            base.OnItemRemoved( args );
        }

        private void ResetExpectations()
        {
            this.ExpectedSetCount = 0;
            this.ExpectedContainsKeyCount = 0;
            this.ExpectedGetCount = 0;
            this.ExpectedRemoveCount = 0;
            this.ExpectedInvalidateCount = 0;
        }

        public void ResetTest( string locationDescription )
        {
            AssertEx.Equal(
                this.ExpectedSetCount,
                this._actualSetCount,
                string.Format( "{0}: The set operation was not called as many times as expected.", locationDescription ) );

            AssertEx.Equal(
                this.ExpectedContainsKeyCount,
                this._actualContainsKeyCount,
                string.Format( "{0}: The contains key operation was not called as many times as expected.", locationDescription ) );

            AssertEx.Equal(
                this.ExpectedGetCount,
                this._actualGetCount,
                string.Format( "{0}: The get operation was not called as many times as expected.", locationDescription ) );

            AssertEx.Equal(
                this.ExpectedRemoveCount,
                this._actualRemoveCount,
                string.Format( "{0}: The remove operation was not called as many times as expected.", locationDescription ) );

            AssertEx.Equal(
                this.ExpectedInvalidateCount,
                this._actualInvalidateCount,
                string.Format( "{0}: The invalidate object operation was not called as many times as expected.", locationDescription ) );

            this._actualSetCount = 0;
            this._actualContainsKeyCount = 0;
            this._actualGetCount = 0;
            this._actualRemoveCount = 0;
            this._actualInvalidateCount = 0;
            this.ResetExpectations();
        }

        protected override void SetItemCore( string key, CacheItem item )
        {
            ++this._actualSetCount;
            this._backend.SetItem( key, item );
            this.LastCachedItem = item;
            this.LastCachedKey = key;
            this.ItemSet?.Invoke( this, new CacheItemSetEventArgs( key, item, null ) );
        }

        protected override async Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
        {
            ++this._actualSetCount;
            await this._backend.SetItemAsync( key, item, cancellationToken );
            this.LastCachedItem = item;
            this.LastCachedKey = key;
            this.ItemSet?.Invoke( this, new CacheItemSetEventArgs( key, item, null ) );
        }

        protected override bool ContainsItemCore( string key )
        {
            ++this._actualContainsKeyCount;

            return this._backend.ContainsItem( key );
        }

        protected override async Task<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this._actualContainsKeyCount;

            return await this._backend.ContainsItemAsync( key, cancellationToken );
        }

        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            ++this._actualGetCount;

            return this._backend.GetItem( key, includeDependencies );
        }

        protected override async Task<CacheValue> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
        {
            ++this._actualGetCount;

            return await this._backend.GetItemAsync( key, includeDependencies, cancellationToken );
        }

        protected override void InvalidateDependencyCore( string key )
        {
            ++this._actualInvalidateCount;
            this._backend.InvalidateDependency( key );
        }

        protected override async Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this._actualInvalidateCount;
            await this._backend.InvalidateDependencyAsync( key, cancellationToken );
        }

        protected override bool ContainsDependencyCore( string key )
        {
            return this._backend.ContainsDependency( key );
        }

        protected override Task<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this._backend.ContainsDependencyAsync( key, cancellationToken );
        }

        protected override void DisposeCore( bool disposing )
        {
            TestableCachingComponentDisposer.Dispose( this._backend );
            AssertEx.Equal( 0, this.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
        }

        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            await TestableCachingComponentDisposer.DisposeAsync( this._backend );
            AssertEx.Equal( 0, this.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
        }

        protected override void ClearCore()
        {
            this._backend.Clear();
        }

        protected override Task ClearAsyncCore( CancellationToken cancellationToken )
        {
            return this._backend.ClearAsync( cancellationToken );
        }

        protected override void RemoveItemCore( string key )
        {
            ++this._actualRemoveCount;
            this._backend.RemoveItem( key );
        }

        protected override async Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this._actualRemoveCount;
            await this._backend.RemoveItemAsync( key, cancellationToken );
        }
    }
}