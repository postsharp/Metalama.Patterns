// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.TestHelpers.Shared;
using PostSharp.Patterns.Common.Tests.Helpers;
using CacheItem = PostSharp.Patterns.Caching.Implementation.CacheItem;

namespace PostSharp.Patterns.Caching.TestHelpers
{
    public sealed class TestingCacheBackend : CachingBackend
    {
        private readonly CachingBackend backend;

        // TODO: This pattern might be in a separate class

        private int actualSetCount = 0;
        private int actualContainsKeyCount = 0;
        private int actualGetCount = 0;
        private int actualRemoveCount = 0;
        private int actualInvalidateCount = 0;

        public int ExpectedSetCount { get; set; }

        public int ExpectedContainsKeyCount { get; set; }

        public int ExpectedGetCount { get; set; }

        public int ExpectedRemoveCount { get; set; }

        public int ExpectedInvalidateCount { get; set; }

        public string LastCachedKey { get; set; }

        public CacheItem LastCachedItem { get; set; }

        public event CacheItemSetEventHandler ItemSet;

        protected override CachingBackendFeatures CreateFeatures() => this.backend.SupportedFeatures;

        public TestingCacheBackend( string name )
        {
            this.ResetExpectations();
#if RUNTIME_CACHING
            this.backend = new MemoryCachingBackend( new MemoryCache( name ) );
#elif EXTENSIONS_CACHING
            this.backend = new MemoryCacheBackend(
                new Microsoft.Extensions.Caching.Memory.MemoryCache( new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() ) );
#else
#error You must define at least one of: RUNTIME_CACHING, EXTENSIONS_CACHING.
#endif
            this.backend.ItemRemoved += this.OnItemRemoved;
            this.backend.DependencyInvalidated += this.OnDependencyInvalidated;
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
            AssertEx.Equal( this.ExpectedSetCount, this.actualSetCount,
                             string.Format( "{0}: The set operation was not called as many times as expected.", locationDescription ) );
            AssertEx.Equal( this.ExpectedContainsKeyCount, this.actualContainsKeyCount,
                             string.Format( "{0}: The contains key operation was not called as many times as expected.", locationDescription ) );
            AssertEx.Equal( this.ExpectedGetCount, this.actualGetCount,
                             string.Format( "{0}: The get operation was not called as many times as expected.", locationDescription ) );
            AssertEx.Equal( this.ExpectedRemoveCount, this.actualRemoveCount,
                             string.Format( "{0}: The remove operation was not called as many times as expected.", locationDescription ) );
            AssertEx.Equal( this.ExpectedInvalidateCount, this.actualInvalidateCount,
                             string.Format( "{0}: The invalidate object operation was not called as many times as expected.", locationDescription ) );

            this.actualSetCount = 0;
            this.actualContainsKeyCount = 0;
            this.actualGetCount = 0;
            this.actualRemoveCount = 0;
            this.actualInvalidateCount = 0;
            this.ResetExpectations();
        }


        protected override void SetItemCore( string key, CacheItem item )
        {
            ++this.actualSetCount;
            this.backend.SetItem( key, item );
            this.LastCachedItem = item;
            this.LastCachedKey = key;
            this.ItemSet?.Invoke( this, new CacheItemSetEventArgs( key, item, null ) );
        }

        protected override async Task SetItemAsyncCore( string key, CacheItem item, CancellationToken cancellationToken )
        {
            ++this.actualSetCount;
            await this.backend.SetItemAsync( key, item, cancellationToken );
            this.LastCachedItem = item;
            this.LastCachedKey = key;
            this.ItemSet?.Invoke( this, new CacheItemSetEventArgs( key, item, null ) );
        }

        protected override bool ContainsItemCore( string key )
        {
            ++this.actualContainsKeyCount;
            return this.backend.ContainsItem( key );
        }

        protected override async Task<bool> ContainsItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this.actualContainsKeyCount;
            return await this.backend.ContainsItemAsync( key, cancellationToken );
        }

        protected override CacheValue GetItemCore( string key, bool includeDependencies )
        {
            ++this.actualGetCount;
            return this.backend.GetItem( key, includeDependencies );
        }

        protected override async Task<CacheValue> GetItemAsyncCore( string key, bool includeDependencies, CancellationToken cancellationToken )
        {
            ++this.actualGetCount;
            return await this.backend.GetItemAsync( key, includeDependencies, cancellationToken );
        }

        protected override void InvalidateDependencyCore( string key )
        {
            ++this.actualInvalidateCount;
            this.backend.InvalidateDependency( key );
        }

        protected override async Task InvalidateDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this.actualInvalidateCount;
            await this.backend.InvalidateDependencyAsync( key, cancellationToken );
        }

        protected override bool ContainsDependencyCore( string key )
        {
            return this.backend.ContainsDependency( key );
        }

        protected override Task<bool> ContainsDependencyAsyncCore( string key, CancellationToken cancellationToken )
        {
            return this.backend.ContainsDependencyAsync( key, cancellationToken );
        }

        protected override void DisposeCore(bool disposing)
        {
            TestableCachingComponentDisposer.Dispose( this.backend );
            AssertEx.Equal( 0, this.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
        }

        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            await TestableCachingComponentDisposer.DisposeAsync( this.backend );
            AssertEx.Equal( 0, this.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
        }

        protected override void ClearCore()
        {
            this.backend.Clear();
        }

        protected override Task ClearAsyncCore( CancellationToken cancellationToken )
        {
            return this.backend.ClearAsync( cancellationToken );
        }

        protected override void RemoveItemCore( string key )
        {
            ++this.actualRemoveCount;
            this.backend.RemoveItem( key );
        }

        protected override async Task RemoveItemAsyncCore( string key, CancellationToken cancellationToken )
        {
            ++this.actualRemoveCount;
            await this.backend.RemoveItemAsync( key, cancellationToken );
        }
    }
}