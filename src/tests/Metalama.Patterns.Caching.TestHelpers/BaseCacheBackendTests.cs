// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public abstract class BaseCacheBackendTests : BaseCachingTests, IDisposable, IClassFixture<TestContext>
    {
        protected const int Timeout = 30_000; // 30 seconds ought to be enough to anyone. (otherwise the test should be refactored, anyway).
        protected static readonly TimeSpan TimeoutTimeSpan = TimeSpan.FromMilliseconds( Timeout );
        
        protected BaseCacheBackendTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testOutputHelper )
        {
            this.TestContext = testContext;
        }

        protected virtual bool TestDependencies { get; } = true;

        protected TestContext TestContext { get; }

        protected abstract CachingBackend CreateBackend();

        protected virtual Task<CachingBackend> CreateBackendAsync()
        {
            return Task.FromResult( this.CreateBackend() );
        }

        protected virtual void GiveChanceToResetLocalCache( CachingBackend backend ) { }

        protected virtual ITestableCachingComponent CreateCollector( CachingBackend backend )
        {
            // Return anything disposable.
            return new NullTestableCachingComponent();
        }

        protected virtual Task<ITestableCachingComponent> CreateCollectorAsync( CachingBackend backend )
        {
            // Return anything disposable.
            return Task.FromResult( this.CreateCollector( backend ) );
        }

        protected virtual TimeSpan GetExpirationQuantum( double multiplier = 1 )
        {
            return TimeSpan.FromSeconds( 0.05 * multiplier );
        }

        private static Task RepeatUntilNullOrFailAsync<T>( Func<ValueTask<T?>> func )
            where T : class
            => RepeatUntilNullOrFailAsync( () => func().AsTask() );

        private static async Task RepeatUntilNullOrFailAsync<T>( Func<Task<T?>> func )
            where T : class
        {
            var stopwatch = Stopwatch.StartNew();

            while ( stopwatch.Elapsed < TimeoutTimeSpan )
            {
                if ( await func() == null )
                {
                    return;
                }

                await Task.Delay( 10 );
            }

            Assert.Fail( $"The item still exists in cache after the {TimeoutTimeSpan} timeout." );
        }

        private static void RepeatUntilNullOrFail<T>( Func<T?> func )
            where T : class
        {
            var stopwatch = Stopwatch.StartNew();

            while ( stopwatch.Elapsed < TimeoutTimeSpan )
            {
                if ( func() == null )
                {
                    return;
                }

                Thread.Sleep( 10 );
            }

            Assert.Fail( $"The item still exists in cache after the {TimeoutTimeSpan} timeout." );
        }

        public void Dispose()
        {
            this.Cleanup();
        }

        protected virtual void Cleanup()
        {
            GC.Collect();
            AssertEx.Equal( 0, BackgroundTaskScheduler.AllBackgroundTaskExceptions, "CachingBackend.AllBackgroundTaskExceptions" );
        }

        [Fact( Timeout = Timeout )]
        public void TestMiss()
        {
            using ( var cache = this.CreateBackend() )
            {
                var key = Guid.NewGuid().ToString();

                if ( cache.SupportedFeatures.Clear )
                {
                    cache.Clear();
                }

                var retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The cache does not return null on miss." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestMissAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                var key = Guid.NewGuid().ToString();

                if ( cache.SupportedFeatures.Clear )
                {
                    await cache.ClearAsync();
                }

                var retrievedItem = await cache.GetItemAsync( key );

                AssertEx.Null( retrievedItem, $"The cache does not return null on miss. It returned {{{retrievedItem}}} instead." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestSet()
        {
            using ( var cache = this.CreateBackend() )
            {
                var storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                var cacheItem0 = new CacheItem( storedValue0, this.TestDependencies ? ImmutableList.Create( "a", "b", "c" ) : null );

                cache.SetItem( key, cacheItem0 );
                var retrievedItem = cache.GetItem( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );

                AssertEx.Equal( storedValue0, retrievedItem.Value, "The item retrieved before the timeout is not the same as the initial item." );

                if ( this.TestDependencies )
                {
                    // The dependencies retrieved before the timeout must be the same as the initial dependencies.
                    Assert.Equal( cacheItem0.Dependencies?.ToList(), (ICollection?) retrievedItem.Dependencies?.ToList() );
                }

                var storedValue1 = new CachedValueClass( 1 );
                var cacheItem1 = new CacheItem( storedValue1 );

                cache.SetItem( key, cacheItem1 );
                this.GiveChanceToResetLocalCache( cache );
                retrievedItem = cache.GetItem( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.NotEqual( cacheItem0, retrievedItem, "The item has not been changed." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestSetAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                var storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                var cacheItem0 = new CacheItem( storedValue0, this.TestDependencies ? ImmutableList.Create( "a", "b", "c" ) : null );

                // [Porting] Not fixing, can't be certain of original intent.
                // ReSharper disable once MethodHasAsyncOverload
                cache.SetItem( key, cacheItem0 );
                var retrievedItem = await cache.GetItemAsync( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );

                AssertEx.Equal( storedValue0, retrievedItem.Value, "The item retrieved before the timeout is not the same as the initial item." );

                if ( this.TestDependencies )
                {
                    // "The dependencies retrieved before the timeout must be the same as the initial dependencies."
                    Assert.Equal( cacheItem0.Dependencies?.ToList(), (ICollection?) retrievedItem.Dependencies?.ToList() );
                }

                var storedValue1 = new CachedValueClass( 1 );
                var cacheItem1 = new CacheItem( storedValue1 );

                await cache.SetItemAsync( key, cacheItem1 );
                this.GiveChanceToResetLocalCache( cache );
                retrievedItem = await cache.GetItemAsync( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.NotEqual( cacheItem0, retrievedItem, "The item has not been changed." );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestAbsoluteExpiration()
        {
            while ( true )
            {
                using ( var cache = this.CreateBackend() )
                {
                    try
                    {
                        var storedValue = new CachedValueClass( 0 );
                        const string key = "0";

                        var offset = this.GetExpirationQuantum( 3 );

                        var cacheItem = new CacheItem(
                            storedValue,
                            Configuration: new CacheItemConfiguration { AbsoluteExpiration = offset },
                            Dependencies: this.TestDependencies ? ImmutableList.Create( "d" ) : null );

                        var itemRemovedEvent = new ManualResetEvent( false );
                        cache.ItemRemoved += ( _, _ ) => itemRemovedEvent.Set();
                        var setTime = DateTime.Now;

                        cache.SetItem( key, cacheItem );

                        Thread.Sleep( this.GetExpirationQuantum() );
                        var retrievedItemBeforeTimeout = cache.GetItem( key );

                        if ( DateTime.Now > setTime + offset )
                        {
                            // Bad timing. Retry the test.
                            this.TestOutputHelper.WriteLine( "We waited too much." );

                            continue;
                        }

                        AssertEx.NotNull( retrievedItemBeforeTimeout, "The item has been removed before expiration." );

                        // This forces collection of the expired item on some backends.

                        Thread.Sleep( offset.Multiply( 2 ) );

                        // ReSharper disable once AccessToDisposedClosure
                        RepeatUntilNullOrFail( () => cache.GetItem( key ) );

                        Assert.True( itemRemovedEvent.WaitOne( TimeoutTimeSpan ) );

                        var retrievedItemAfterTimeout = cache.GetItem( key );

                        AssertEx.Null( retrievedItemAfterTimeout, "There is an item retrieved after the timeout." );

                        return;
                    }
                    finally
                    {
                        TestableCachingComponentDisposer.Dispose( cache );
                    }
                }
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestAbsoluteExpirationDependencyCollectedAsync()
        {
            if ( !this.TestDependencies || !RunningOnWindows )
            {
                AssertEx.Inconclusive();

                return;
            }

            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            using ( var collector = await this.CreateCollectorAsync( cache ) )
            {
                try
                {
                    var storedValue = new CachedValueClass( 0 );
                    const string key = "0";

                    var offset = this.GetExpirationQuantum();

                    var cacheItem = new CacheItem(
                        storedValue,
                        Configuration: new CacheItemConfiguration { AbsoluteExpiration = offset },
                        Dependencies: ImmutableList.Create( "d" ) );

                    var itemRemoved = new TaskCompletionSource<bool>();
                    cache.ItemRemoved += ( _, _ ) => itemRemoved.SetResult( true );

                    await cache.SetItemAsync( key, cacheItem );

                    while ( !itemRemoved.Task.IsCompleted )
                    {
                        // [Porting] Not fixing, can't be certain of original intent.
                        // ReSharper disable once MethodHasAsyncOverload
                        cache.SetItem( "cycle", new CacheItem( "value" ) );
                        await Task.Delay( 50 );
                    }

                    Assert.True( await itemRemoved.Task.WithTimeout( TimeoutTimeSpan ) );

                    await RepeatUntilNullOrFailAsync( () => cache.GetItemAsync( key ) );

                    if ( cache.SupportedFeatures.ContainsDependency )
                    {
                        var success = false;
                        var until = DateTime.Now.AddSeconds( 30 );

                        while ( DateTime.Now < until )
                        {
                            if ( !await cache.ContainsDependencyAsync( "d" ) )
                            {
                                success = true;

                                break;
                            }
                        }

                        this.TestOutputHelper.WriteLine( $"Checking for dependency in {cache}." );
                        Assert.True( success );
                    }
                }
                finally
                {
                    await TestableCachingComponentDisposer.DisposeAsync( cache, collector );
                }
            }

            // ReSharper restore UseAwaitUsing
        }

        private static bool? _runningOnWindows;

        /// <summary>
        /// Gets a value indicating whether the test is run on Windows. We don't run some tests on Linux because, for some reason, the event that an item expired from the cache
        /// arrives up to 20 minutes later on Linux, and I don't know why. So we're just no longer testing this on Linux.
        /// </summary>
        private static bool RunningOnWindows => _runningOnWindows ?? (_runningOnWindows = RuntimeInformation.IsOSPlatform( OSPlatform.Windows )).Value;

        [Fact( Timeout = Timeout )]
        public void TestAbsoluteExpirationDependencyCollected()
        {
            if ( !this.TestDependencies || !RunningOnWindows )
            {
                AssertEx.Inconclusive();

                return;
            }

            // We need at least one sync test with the collector, this is why we have this one, otherwise a duplicate from TestAbsoluteExpirationDependencyCollectedAsync.

            using ( var cache = this.CreateBackend() )
            using ( var collector = this.CreateCollector( cache ) )
            {
                var storedValue = new CachedValueClass( 0 );
                const string key = "0";

                var offset = this.GetExpirationQuantum();

                var cacheItem = new CacheItem(
                    storedValue,
                    Configuration: new CacheItemConfiguration { AbsoluteExpiration = offset },
                    Dependencies: ImmutableList.Create( "d" ) );

                var itemRemoved = new ManualResetEventSlim( false );
                cache.ItemRemoved += ( _, _ ) => itemRemoved.Set();
                cache.SetItem( key, cacheItem );

                while ( !itemRemoved.IsSet )
                {
                    cache.SetItem( "cycle", new CacheItem( "value" ) );
                    Thread.Yield();
                }

                // ReSharper disable once AccessToDisposedClosure
                RepeatUntilNullOrFail( () => cache.GetItem( key ) );

                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    var success = false;
                    var until = DateTime.Now.AddSeconds( 30 );

                    while ( DateTime.Now < until )
                    {
                        if ( !cache.ContainsDependency( "d" ) )
                        {
                            success = true;

                            break;
                        }
                    }

                    this.TestOutputHelper.WriteLine( $"Checking for dependency in {cache}." );
                    Assert.True( success );
                }

                TestableCachingComponentDisposer.Dispose( cache, collector );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestSlidingExpiration()
        {
            if ( !RunningOnWindows )
            {
                AssertEx.Inconclusive();

                return;
            }

            while ( true )
            {
                using ( var cache = this.CreateBackend() )
                {
                    try
                    {
                        var storedValue = new CachedValueClass( 0 );
                        const string key = "0";
                        var expiration = this.GetExpirationQuantum( 2 );

                        var cacheItem = new CacheItem(
                            storedValue,
                            Configuration:
                            new CacheItemConfiguration { SlidingExpiration = expiration } );

                        var itemRemoved = new ManualResetEventSlim( false );
                        cache.ItemRemoved += ( _, _ ) => itemRemoved.Set();
                        var timeWhenSet = DateTime.Now;

                        cache.SetItem( key, cacheItem );
                        Thread.Sleep( this.GetExpirationQuantum() );

                        var retrievedItemBeforeTimeout = cache.GetItem( key );

                        if ( DateTime.Now > timeWhenSet + expiration )
                        {
                            this.TestOutputHelper.WriteLine( "We slept too much time. Retry the test." );

                            continue;
                        }

                        AssertEx.NotNull( retrievedItemBeforeTimeout, "There is not an item retrieved before the timeout." );

                        while ( !itemRemoved.IsSet )
                        {
                            cache.SetItem( "cycle", new CacheItem( "value" ) );
                            Thread.Yield();
                        }

                        Thread.Sleep( this.GetExpirationQuantum() );

                        // ReSharper disable once AccessToDisposedClosure
                        RepeatUntilNullOrFail( () => cache.GetItem( key ) );

                        return;
                    }
                    finally
                    {
                        TestableCachingComponentDisposer.Dispose( cache );
                    }
                }
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestSlidingExpirationAsync()
        {
            if ( !RunningOnWindows )
            {
                AssertEx.Inconclusive();

                return;
            }

            while ( true )
            {
                // Having an outer try-finally block is broken in VS 2019
                // https://github.com/dotnet/roslyn/issues/34720
                // using ( CachingBackend cache = this.CreateBackend() )
                // {

                var cache = await this.CreateBackendAsync();

                try
                {
                    var storedValue = new CachedValueClass( 0 );
                    const string key = "0";
                    var expiration = this.GetExpirationQuantum( 2 );

                    var cacheItem = new CacheItem(
                        storedValue,
                        Configuration:
                        new CacheItemConfiguration { SlidingExpiration = expiration } );

                    var timeWhenSet = DateTime.Now;
                    var itemRemoved = new TaskCompletionSource<bool>();
                    cache.ItemRemoved += ( _, _ ) => itemRemoved.SetResult( true );
                    await cache.SetItemAsync( key, cacheItem );

                    Thread.Sleep( this.GetExpirationQuantum() );

                    if ( DateTime.Now > timeWhenSet + expiration )
                    {
                        this.TestOutputHelper.WriteLine( "We slept too much time." );

                        continue;
                    }

                    var retrievedItemBeforeTimeout = await cache.GetItemAsync( key );
                    AssertEx.NotNull( retrievedItemBeforeTimeout, "There is not an item retrieved before the timeout." );

                    while ( !itemRemoved.Task.IsCompleted )
                    {
                        await cache.SetItemAsync( "cycle", new CacheItem( "value" ) );
                        await Task.Delay( 50 );
                    }

                    await RepeatUntilNullOrFailAsync( () => cache.GetItemAsync( key ) );

                    return;
                }
                finally
                {
                    await TestableCachingComponentDisposer.DisposeAsync( cache );

                    // [Porting] Not fixing, can't be certain of original intent.
                    // ReSharper disable once MethodHasAsyncOverload
                    cache.Dispose();
                }

                // }
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestRemovalEventByExpiration()
        {
            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs? removalArguments = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    removalArguments = args;
                    eventRaised.Set();
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";
                var offset = this.GetExpirationQuantum();

                var cacheItem = new CacheItem( storedValue, Configuration: new CacheItemConfiguration { AbsoluteExpiration = offset } );

                cache.SetItem( key, cacheItem );

                Thread.Sleep( offset.Multiply( 2 ) );
                
                // ReSharper disable once AccessToDisposedClosure
                RepeatUntilNullOrFail( () => cache.GetItem( key ) );

                Assert.True( eventRaised.WaitOne( TimeoutTimeSpan ) );

                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Expired, removalArguments.RemovedReason );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestRemovalEventByExpirationAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent (twice).
            // ReSharper disable once UseAwaitUsing
            // ReSharper disable once MethodHasAsyncOverload
            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs? removalArguments = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    removalArguments = args;
                    eventRaised.Set();
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";
                var offset = this.GetExpirationQuantum();

                var cacheItem = new CacheItem( storedValue, Configuration: new CacheItemConfiguration { AbsoluteExpiration = offset } );

                await cache.SetItemAsync( key, cacheItem );

                Thread.Sleep( offset.Multiply( 2 ) );
                await RepeatUntilNullOrFailAsync( () => cache.GetItemAsync( key ) ); // Forces collection in some backends as well.

                Assert.True( eventRaised.WaitOne( TimeoutTimeSpan ) );

                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Expired, removalArguments.RemovedReason );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestRemovalEventByEviction()
        {
            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Clear || !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var eventRaised = new ManualResetEventSlim();
                CacheItemRemovedEventArgs? removalArguments = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    // Order matters, since at the point that the manual reset event is set, the assertions can start happening,
                    // and so the argument must already be set:
                    removalArguments = args;
                    eventRaised.Set();
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";
                var cacheItem = new CacheItem( storedValue );
                cache.SetItem( key, cacheItem );

                cache.Clear();

                Assert.True( eventRaised.Wait( TimeSpan.FromSeconds( 5 ) ), "The event has not been raised." );
                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Evicted, removalArguments.RemovedReason );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestRemovalEventByEvictionAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent (twice).
            // ReSharper disable once UseAwaitUsing
            // ReSharper disable once MethodHasAsyncOverload
            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Clear || !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs? removalArguments = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    removalArguments = args;
                    eventRaised.Set();
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";
                var cacheItem = new CacheItem( storedValue );
                await cache.SetItemAsync( key, cacheItem );

                await cache.ClearAsync();

                Assert.True( eventRaised.WaitOne( TimeoutTimeSpan ) );

                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Evicted, removalArguments.RemovedReason );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact]

        // [Timeout( Timeout )]
        public void TestRemovalEventByDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var itemEventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs? itemEventArgs = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    itemEventArgs = args;
                    itemEventRaised.Set();
                };

                var dependencyEventRaised = new ManualResetEvent( false );
                CacheDependencyInvalidatedEventArgs? dependencyEventArgs = null;

                cache.DependencyInvalidated += ( _, args ) =>
                {
                    dependencyEventArgs = args;
                    dependencyEventRaised.Set();
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";

                const string dependencyKey = "1";

                var cacheItem = new CacheItem(
                    storedValue,
                    Dependencies: ImmutableList.Create( dependencyKey ) );

                cache.SetItem( key, cacheItem );
                cache.InvalidateDependency( dependencyKey );
                Assert.Null( cache.GetItem( key ) );

                Assert.True( itemEventRaised.WaitOne( TimeoutTimeSpan ), "Did not receive ItemRemoved event." );
                Assert.True( dependencyEventRaised.WaitOne( TimeoutTimeSpan ), "Did not received DependencyInvalidated event." );

                AssertEx.NotNull( itemEventArgs, "The item event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Invalidated, itemEventArgs.RemovedReason );

                AssertEx.NotNull( dependencyEventArgs, "The dependency event did not pass any arguments." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestRemovalEventByDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            // [Porting] Not fixing, can't be certain of original intent (twice).
            // ReSharper disable once UseAwaitUsing
            // ReSharper disable once MethodHasAsyncOverload
            using ( var cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();

                    return;
                }

                var itemEventRaised = new TaskCompletionSource<bool>();
                CacheItemRemovedEventArgs? itemEventArgs = null;

                cache.ItemRemoved += ( _, args ) =>
                {
                    itemEventArgs = args;
                    itemEventRaised.SetResult( true );
                };

                var dependencyEventRaised = new TaskCompletionSource<bool>();
                CacheDependencyInvalidatedEventArgs? dependencyEventArgs = null;

                cache.DependencyInvalidated += ( _, args ) =>
                {
                    dependencyEventArgs = args;
                    dependencyEventRaised.SetResult( true );
                };

                var storedValue = new CachedValueClass( 0 );
                const string key = "0";

                const string dependencyKey = "1";

                var cacheItem = new CacheItem(
                    storedValue,
                    Dependencies: ImmutableList.Create( dependencyKey ) );

                await cache.SetItemAsync( key, cacheItem );

                await cache.InvalidateDependencyAsync( dependencyKey );
                Assert.Null( await cache.GetItemAsync( key ) );

                Assert.True( await Task.WhenAll( itemEventRaised.Task, dependencyEventRaised.Task ).WithTimeout( TimeoutTimeSpan ) );

                // await cache.FlushAsync();

                AssertEx.NotNull( itemEventArgs, "The item event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Invalidated, itemEventArgs.RemovedReason );

                AssertEx.NotNull( dependencyEventArgs, "The dependency event did not pass any arguments." );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            using ( var cache = this.CreateBackend() )
            {
                List<CacheItemRemovedEventArgs> events = new();
                cache.ItemRemoved += ( _, args ) => events.Add( args );

                const string dependencyKey = "dependency";
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), ImmutableList.Create( dependencyKey ) );
                cache.SetItem( "m1", cacheItem1 );

                this.GiveChanceToResetLocalCache( cache );

                Assert.Empty( events );
                Assert.NotNull( cache.GetItem( "m1" ) );
                cache.InvalidateDependency( dependencyKey );

                this.GiveChanceToResetLocalCache( cache );

                Assert.Null( cache.GetItem( "m1" ) );

                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    Assert.False( cache.ContainsDependency( dependencyKey ) );
                }

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            // [Porting] Not fixing, can't be certain of original intent (twice).
            // ReSharper disable once UseAwaitUsing
            // ReSharper disable once MethodHasAsyncOverload
            using ( var cache = this.CreateBackend() )
            {
                var eventsCount = 0;
                cache.ItemRemoved += ( _, _ ) => eventsCount++;

                const string dependencyKey = "dependency";
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), ImmutableList.Create( dependencyKey ) );
                await cache.SetItemAsync( "m1", cacheItem1 );

                this.GiveChanceToResetLocalCache( cache );

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( await cache.GetItemAsync( "m1" ) );

                this.GiveChanceToResetLocalCache( cache );

                await cache.InvalidateDependencyAsync( dependencyKey );
                Assert.Null( await cache.GetItemAsync( "m1" ) );

                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    Assert.False( await cache.ContainsDependencyAsync( dependencyKey ) );
                }

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestSharedDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            using ( var cache = this.CreateBackend() )
            {
                var eventsCount = 0;
                cache.ItemRemoved += ( _, _ ) => eventsCount++;

                const string dependencyKey = "dependency";
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                var cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                cache.SetItem( "m1", cacheItem1 );
                cache.SetItem( "m2", cacheItem2 );

                this.GiveChanceToResetLocalCache( cache );

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( cache.GetItem( "m1" ) );
                Assert.NotNull( cache.GetItem( "m2" ) );

                Assert.Equal( 0, eventsCount );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestSharedDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();

                return;
            }

            // [Porting] Not fixing, can't be certain of original intent (twice).
            // ReSharper disable once UseAwaitUsing
            // ReSharper disable once MethodHasAsyncOverload
            using ( var cache = this.CreateBackend() )
            {
                var eventsCount = 0;
                cache.ItemRemoved += ( _, _ ) => eventsCount++;

                const string dependencyKey = "dependency";
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                var cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                await cache.SetItemAsync( "m1", cacheItem1 );
                await cache.SetItemAsync( "m2", cacheItem2 );

                this.GiveChanceToResetLocalCache( cache );

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( await cache.GetItemAsync( "m1" ) );
                Assert.NotNull( await cache.GetItemAsync( "m2" ) );

                Assert.Equal( 0, eventsCount );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestReplace()
        {
            using ( var cache = this.CreateBackend() )
            {
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( "d1" ) : null );
                var cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( "d2" ) : null );
                cache.SetItem( "m", cacheItem1 );
                cache.SetItem( "m", cacheItem2 );

                this.GiveChanceToResetLocalCache( cache );

                var retrievedValue = cache.GetItem( "m" );
                Assert.NotNull( retrievedValue );

                Assert.Equal( cacheItem2.Value, retrievedValue.Value );

                if ( this.TestDependencies )
                {
                    cache.InvalidateDependency( "d1" );

                    Assert.NotNull( cache.GetItem( "m" ) );
                }

                Assert.True( cache.ContainsItem( "m" ) );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestReplaceAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                var cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( "d1" ) : null );
                var cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( "d2" ) : null );
                await cache.SetItemAsync( "m", cacheItem1 );
                await cache.SetItemAsync( "m", cacheItem2 );

                this.GiveChanceToResetLocalCache( cache );

                var retrievedValue = await cache.GetItemAsync( "m" );
                Assert.NotNull( retrievedValue );

                Assert.Equal( cacheItem2.Value, retrievedValue.Value );

                if ( this.TestDependencies )
                {
                    await cache.InvalidateDependencyAsync( "d1" );

                    Assert.NotNull( await cache.GetItemAsync( "m" ) );
                }

                Assert.True( await cache.ContainsItemAsync( "m" ) );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestSetItemWithDependencyWithoutSupport()
        {
            using ( var cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );

                Assert.Throws<NotSupportedException>( () => cache.SetItem( "i", new CacheItem( "v", ImmutableList.Create( "d" ) ) ) );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestSetItemWithDependencyWithoutSupportAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );

                await Assert.ThrowsAsync<NotSupportedException>(
                    async () => await cache.SetItemAsync( "i", new CacheItem( "v", ImmutableList.Create( "d" ) ) ) );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestInvalidateDependencyWithoutSupport()
        {
            using ( var cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );

                Assert.Throws<NotSupportedException>( () => cache.InvalidateDependency( "d" ) );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestInvalidateDependencyWithoutSupportAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );

                await Assert.ThrowsAsync<NotSupportedException>( async () => await cache.InvalidateDependencyAsync( "d" ) );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestContainsDependencyWithoutSupport()
        {
            using ( var cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                Assert.False( cache.SupportedFeatures.ContainsDependency );

                Assert.Throws<NotSupportedException>( () => cache.ContainsDependency( "d" ) );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestContainsDependencyWithoutSupportAsync()
        {
            // [Porting] Not fixing, can't be certain of original intent.
            // ReSharper disable once UseAwaitUsing
            using ( var cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();

                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                Assert.False( cache.SupportedFeatures.ContainsDependency );

                await Assert.ThrowsAsync<NotSupportedException>( async () => await cache.ContainsDependencyAsync( "d" ) );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        private sealed class NullTestableCachingComponent : ITestableCachingComponent
        {
            public int BackgroundTaskExceptions => 0;

            public void Dispose() { }

            ValueTask IAsyncDisposable.DisposeAsync() => this.DisposeAsync();

            public ValueTask DisposeAsync( CancellationToken cancellationToken = default )
            {
#if NET6_0_OR_GREATER
                return ValueTask.CompletedTask;
#else
                return new ValueTask( Task.CompletedTask );
#endif
            }
        }
    }
}