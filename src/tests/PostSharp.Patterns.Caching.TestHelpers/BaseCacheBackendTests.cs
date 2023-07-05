﻿// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections;
using System.Threading;
using Xunit;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.TestHelpers;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using PostSharp.Patterns.Caching.TestHelpers.Shared;
using PostSharp.Patterns.Threading.Primitives;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.Backends
{
    public abstract class BaseCacheBackendTests : IDisposable, IClassFixture<TestContext>
    {
        protected const int Timeout = 120000; // 2 minutes ought to be enough to anyone. (otherwise the test should be refactored, anyway).
        protected static readonly TimeSpan TimeoutTimeSpan = TimeSpan.FromMilliseconds( Timeout *0.8);

        private const string namePrefix = "Caching.Tests.Backends.MemoryCacheBackendTests_";
        const int eventTimeout = 1000;

        public BaseCacheBackendTests(TestContext testContext)
        {
            this.TestContext = testContext;
        }

        protected virtual bool TestDependencies { get; } = true;

        protected TestContext TestContext { get; private set; }

        protected abstract CachingBackend CreateBackend();
        
        protected virtual Task<CachingBackend> CreateBackendAsync()
        {
            return Task.FromResult( this.CreateBackend() );
        }

        protected virtual void GiveChanceToResetLocalCache( CachingBackend backend ) { }

        internal virtual ITestableCachingComponent CreateCollector( CachingBackend backend )
        {
            // Return anything disposable.
            return new NullTestableCachingComponent();
        }

        internal virtual Task<ITestableCachingComponent> CreateCollectorAsync( CachingBackend backend )
        {
            // Return anything disposable.
            return Task.FromResult( this.CreateCollector( backend ) );
        }

        public virtual TimeSpan GetExpirationTolerance( double multiplier = 1 )
        {
            return TimeSpan.FromSeconds(0.1*multiplier);
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


        [Fact(Timeout = Timeout)]
        public void TestMiss()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                string key = Guid.NewGuid().ToString();

                if (cache.SupportedFeatures.Clear)
                {
                    cache.Clear();
                }

                object retrievedItem = cache.GetItem( key );

                AssertEx.Null( retrievedItem, "The cache does not return null on miss." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }


        [Fact(Timeout = Timeout )]
        public async Task TestMissAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {
                string key = Guid.NewGuid().ToString();

                if (cache.SupportedFeatures.Clear)
                {
                    await cache.ClearAsync();
                }

                object retrievedItem = await cache.GetItemAsync( key );

                AssertEx.Null( retrievedItem, $"The cache does not return null on miss. It returned {{{retrievedItem}}} instead.");

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestSet()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                CachedValueClass storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem0 = new CacheItem( storedValue0, this.TestDependencies ? ImmutableList.Create( "a", "b", "c" ) : null );

                cache.SetItem( key, cacheItem0 );
                CacheValue retrievedItem = cache.GetItem( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );

                AssertEx.Equal( storedValue0, retrievedItem.Value, "The item retrieved before the timeout is not the same as the initial item." );

                if ( this.TestDependencies )
                {
                    // The dependencies retrieved before the timeout must be the same as the initial dependencies.
                    Assert.Equal( cacheItem0.Dependencies.ToList(), (ICollection) retrievedItem.Dependencies.ToList() );
                }

                CachedValueClass storedValue1 = new CachedValueClass( 1 );
                CacheItem cacheItem1 = new CacheItem( storedValue1 );

                cache.SetItem( key, cacheItem1 );
                this.GiveChanceToResetLocalCache(cache);
                retrievedItem = cache.GetItem( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.NotEqual( cacheItem0, retrievedItem, "The item has not been changed." );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestSetAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {
                CachedValueClass storedValue0 = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem0 = new CacheItem( storedValue0, this.TestDependencies ? ImmutableList.Create( "a", "b", "c" ) : null );

                cache.SetItem( key, cacheItem0 );
                CacheValue retrievedItem = await cache.GetItemAsync( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );

                AssertEx.Equal( storedValue0, retrievedItem.Value, "The item retrieved before the timeout is not the same as the initial item." );

                if ( this.TestDependencies )
                {
                    // "The dependencies retrieved before the timeout must be the same as the initial dependencies."
                    Assert.Equal( cacheItem0.Dependencies.ToList(), (ICollection) retrievedItem.Dependencies.ToList() );
                }

                CachedValueClass storedValue1 = new CachedValueClass( 1 );
                CacheItem cacheItem1 = new CacheItem( storedValue1 );

                await cache.SetItemAsync( key, cacheItem1 );
                this.GiveChanceToResetLocalCache(cache);
                retrievedItem = await cache.GetItemAsync( key );

                AssertEx.NotNull( retrievedItem, "The item has not been stored in the cache." );
                AssertEx.NotEqual( cacheItem0, retrievedItem, "The item has not been changed." );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }


        [Fact(Timeout = Timeout )]
        public void TestAbsoluteExpiration()
        {
            while ( true )
            {
                using ( CachingBackend cache = this.CreateBackend() )
                {
                    try
                    {
                        CachedValueClass storedValue = new CachedValueClass( 0 );
                        const string key = "0";

                        TimeSpan offset = this.GetExpirationTolerance(3);

                        CacheItem cacheItem = new CacheItem( storedValue, configuration: new CacheItemConfiguration {AbsoluteExpiration = offset},
                                                             dependencies: this.TestDependencies ? ImmutableList.Create( "d" ) : null );

                        ManualResetEvent itemRemovedEvent = new ManualResetEvent(false);
                        cache.ItemRemoved += (s, a) => itemRemovedEvent.Set();
                        DateTime setTime = DateTime.Now;

                        cache.SetItem( key, cacheItem );

                        Thread.Sleep( this.GetExpirationTolerance());
                        CacheValue retrievedItemBeforeTimeout = cache.GetItem( key );

                        if ( DateTime.Now > setTime + offset )
                        {
                            // Bad timing. Retry the test.
                            continue;
                        }

                        AssertEx.NotNull( retrievedItemBeforeTimeout, "The item has been removed before expiration." );
                        
                        // This forces collection of the expired item on some backends.
                        Thread.Sleep( offset.Multiply( 2 ) );
                        Assert.Null( cache.GetItem( key ) );


                        Assert.True( itemRemovedEvent.WaitOne( TimeoutTimeSpan ) );
                        
                        object retrievedItemAfterTimeout = cache.GetItem( key );

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


        [Fact(Timeout = Timeout )]
        public async Task TestAbsoluteExpirationDependencyCollectedAsync()
        {
            if ( !this.TestDependencies || !this.RunningOnWindows )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = await this.CreateBackendAsync() )
            using ( ITestableCachingComponent collector = await this.CreateCollectorAsync( cache ) )
            {
                try
                {
                    CachedValueClass storedValue = new CachedValueClass( 0 );
                    const string key = "0";

                    TimeSpan offset = this.GetExpirationTolerance();

                    CacheItem cacheItem = new CacheItem( storedValue,
                                                            configuration: new CacheItemConfiguration {AbsoluteExpiration = offset},
                                                            dependencies: ImmutableList.Create( "d" ) );


                    TaskCompletionSource<bool> itemRemoved = new TaskCompletionSource<bool>();
                    cache.ItemRemoved += ( sender, args ) => itemRemoved.SetResult( true );

                    await cache.SetItemAsync( key, cacheItem );

                    while ( !itemRemoved.Task.IsCompleted )
                    {
                        cache.SetItem( "cycle", new CacheItem( "value" ) );
                        await Task.Delay( 50 );
                    }
                    Assert.True( await itemRemoved.Task.WithTimeout( TimeoutTimeSpan ) );
                    
                    object retrievedItemAfterTimeout = await cache.GetItemAsync( key );

                    AssertEx.Null( retrievedItemAfterTimeout, "There is an item retrieved after the timeout." );


                    if ( cache.SupportedFeatures.ContainsDependency )
                    {
                        bool success = false;
                        DateTime until = DateTime.Now.AddSeconds( 30 );
                        while ( DateTime.Now < until )
                        {
                            if ( !await cache.ContainsDependencyAsync( "d" ) )
                            {
                                success = true;
                                break;
                            }
                        }

                        Console.WriteLine( $"Checking for dependency in {cache}." );
                        Assert.True( success );
                    }
                }
                finally
                {
                    await TestableCachingComponentDisposer.DisposeAsync( cache, collector );
                }
            }
            
        }

        private static bool? runningOnWindows;

        /// <summary>
        /// Returns true if the test is run on Windows. We don't run some tests on Linux because, for some reason, the event that an item expired from the cache
        /// arrives up to 20 minutes later on Linux, and I don't know why. So we're just no longer testing this on Linux.
        /// </summary>
        private bool RunningOnWindows =>
            runningOnWindows ?? (runningOnWindows =
#if RUNTIME_INFORMATION
                RuntimeInformation.IsOSPlatform( OSPlatform.Windows )).Value;
#else
                Environment.OSVersion.Platform == PlatformID.Win32NT).Value;
#endif

        [Fact(Timeout = Timeout )]
        public void TestAbsoluteExpirationDependencyCollected()
        {
            if ( !this.TestDependencies || !this.RunningOnWindows )
            {
                AssertEx.Inconclusive();
                return;
            }

            // We need at least one sync test with the collector, this is why we have this one, otherwise a duplicate from TestAbsoluteExpirationDependencyCollectedAsync.

            using ( CachingBackend cache = this.CreateBackend() )
            using ( ITestableCachingComponent collector = this.CreateCollector( cache ) )
            {
                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";

                TimeSpan offset = this.GetExpirationTolerance();

                CacheItem cacheItem = new CacheItem( storedValue,
                                                     configuration: new CacheItemConfiguration {AbsoluteExpiration = offset},
                                                     dependencies: ImmutableList.Create( "d" ) );


                ManualResetEventSlim itemRemoved = new ManualResetEventSlim( false );
                cache.ItemRemoved += ( sender, args ) => itemRemoved.Set();
                cache.SetItem( key, cacheItem );

                while ( !itemRemoved.IsSet )
                {
                    cache.SetItem( "cycle", new CacheItem( "value" ) );
                    Thread.Yield();
                }
                
                object retrievedItemAfterTimeout = cache.GetItem( key );

                AssertEx.Null( retrievedItemAfterTimeout, "There is an item retrieved after the timeout." );


                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    bool success = false;
                    DateTime until = DateTime.Now.AddSeconds( 30 );
                    while ( DateTime.Now < until )
                    {
                        
                        if ( !cache.ContainsDependency( "d" ) )
                        {
                            success = true;
                            break;
                        }
                    }

                    Console.WriteLine( $"Checking for dependency in {cache}." );
                    Assert.True( success );
                }

                TestableCachingComponentDisposer.Dispose( cache, collector );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestSlidingExpiration()
        {
            if ( !this.RunningOnWindows )
            {
                AssertEx.Inconclusive();
                return;
            }
            while ( true )
            {
                using ( CachingBackend cache = this.CreateBackend() )
                {
                    try
                    {
                        CachedValueClass storedValue = new CachedValueClass( 0 );
                        const string key = "0";
                        TimeSpan expiration = this.GetExpirationTolerance( 2 );
                        CacheItem cacheItem = new CacheItem( storedValue,
                                                             configuration:
                                                             new CacheItemConfiguration
                                                             {
                                                                 SlidingExpiration = expiration
                                                             } );

                        ManualResetEventSlim itemRemoved = new ManualResetEventSlim( false );
                        cache.ItemRemoved += ( sender, args ) => itemRemoved.Set();
                        DateTime timeWhenSet = DateTime.Now;

                        cache.SetItem( key, cacheItem );
                        Thread.Sleep( this.GetExpirationTolerance() );

                        object retrievedItemBeforeTimeout = cache.GetItem( key );

                        if ( DateTime.Now > timeWhenSet + expiration )
                        {
                            Console.WriteLine("We slept too much time. Retry the test.");
                            continue;
                        }


                        AssertEx.NotNull( retrievedItemBeforeTimeout, "There is not an item retrieved before the timeout." );

                        while ( !itemRemoved.IsSet )
                        {
                            cache.SetItem( "cycle", new CacheItem( "value" ) );
                            Thread.Yield();
                        }
                        
                        object retrievedItemAfterTimeout = cache.GetItem( key );

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

        [Fact(Timeout = Timeout )]
        public async Task TestSlidingExpirationAsync()
        {
            if ( !this.RunningOnWindows )
            {
                AssertEx.Inconclusive();
                return;
            }
            while ( true )
            {
                // Having an outer try-finally block is broken in VS 2019
                // https://github.com/dotnet/roslyn/issues/34720
                //using ( CachingBackend cache = this.CreateBackend() )
                //{
                    CachingBackend cache = this.CreateBackend();
                    try
                    {
                        CachedValueClass storedValue = new CachedValueClass( 0 );
                        const string key = "0";
                        TimeSpan expiration = this.GetExpirationTolerance( 2 );
                        CacheItem cacheItem = new CacheItem( storedValue,
                                                             configuration:
                                                             new CacheItemConfiguration
                                                             {
                                                                 SlidingExpiration = expiration
                                                             } );

                        DateTime timeWhenSet = DateTime.Now;
                        TaskCompletionSource<bool> itemRemoved = new TaskCompletionSource<bool>();
                        cache.ItemRemoved += ( sender, args ) => itemRemoved.SetResult( true );
                        await cache.SetItemAsync( key, cacheItem );

                        Thread.Sleep( this.GetExpirationTolerance() );

                        if ( DateTime.Now > timeWhenSet + expiration )
                        {
                            Console.WriteLine( "We slept too much time." );
                            continue;
                        }

                        object retrievedItemBeforeTimeout = await cache.GetItemAsync( key );
                        AssertEx.NotNull( retrievedItemBeforeTimeout, "There is not an item retrieved before the timeout." );

                        while ( !itemRemoved.Task.IsCompleted )
                        {
                            cache.SetItem( "cycle", new CacheItem( "value" ) );
                            await Task.Delay( 50 );
                        }
                        
                        object retrievedItemAfterTimeout = await cache.GetItemAsync( key );

                        AssertEx.Null( retrievedItemAfterTimeout, "There is an item retrieved after the timeout." );

                        return;
                    }
                    finally
                    {
                        await TestableCachingComponentDisposer.DisposeAsync( cache );
                        cache.Dispose();
                    }
                //}
            }
        }



        [Fact(Timeout = Timeout )]
        public void TestRemovalEventByExpiration()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                ManualResetEvent eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs removalArguments = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         removalArguments = args;
                                         eventRaised.Set();
                                         
                                     };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";
                TimeSpan offset = this.GetExpirationTolerance();

                CacheItem cacheItem = new CacheItem( storedValue, configuration: new CacheItemConfiguration {AbsoluteExpiration = offset} );

                cache.SetItem( key, cacheItem );
                

                Thread.Sleep( offset.Multiply( 2 ) );
                Assert.Null( cache.GetItem( key ) ); // Forces collection in some backends as well.

                Assert.True( eventRaised.WaitOne(TimeoutTimeSpan) );

              
                
                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Expired, removalArguments.RemovedReason );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestRemovalEventByExpirationAsync()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                ManualResetEvent eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs removalArguments = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         
                                         removalArguments = args;
                                         eventRaised.Set();
                                     };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";
                TimeSpan offset = this.GetExpirationTolerance();

                CacheItem cacheItem = new CacheItem( storedValue, configuration: new CacheItemConfiguration {AbsoluteExpiration = offset} );

                await cache.SetItemAsync( key, cacheItem );


                Thread.Sleep( offset.Multiply( 2 ) );
                Assert.Null( await cache.GetItemAsync( key ) ); // Forces collection in some backends as well.

                Assert.True( eventRaised.WaitOne(TimeoutTimeSpan) );


                
                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Expired, removalArguments.RemovedReason );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestRemovalEventByEviction()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Clear || !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                ManualResetEventSlim eventRaised = new ManualResetEventSlim();
                CacheItemRemovedEventArgs removalArguments = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         // Order matters, since at the point that the manual reset event is set, the assertions can start happening,
                                         // and so the argument must already be set:
                                         removalArguments = args;
                                         eventRaised.Set();
                                     };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem = new CacheItem( storedValue );
                cache.SetItem( key, cacheItem );

                cache.Clear();
                
                

                Assert.True( eventRaised.Wait(TimeSpan.FromSeconds( 5 )), "The event has not been raised." );
                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Evicted, removalArguments.RemovedReason );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestRemovalEventByEvictionAsync()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Clear || !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                ManualResetEvent eventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs removalArguments = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         removalArguments = args;
                                         eventRaised.Set();
                                         
                                     };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";
                CacheItem cacheItem = new CacheItem( storedValue );
                await cache.SetItemAsync( key, cacheItem );

                await cache.ClearAsync();

                Assert.True( eventRaised.WaitOne(TimeoutTimeSpan) );

                
                AssertEx.NotNull( removalArguments, "The event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Evicted, removalArguments.RemovedReason );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact]
        //[Timeout( Timeout )]
        public void TestRemovalEventByDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                ManualResetEvent itemEventRaised = new ManualResetEvent( false );
                CacheItemRemovedEventArgs itemEventArgs = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         itemEventArgs = args;
                                         itemEventRaised.Set();
                                     };

                ManualResetEvent dependencyEventRaised = new ManualResetEvent(false);
                CacheDependencyInvalidatedEventArgs dependencyEventArgs = null;
                cache.DependencyInvalidated += ( sender, args ) =>
                                               {
                                                   
                                                   dependencyEventArgs = args;
                                                   dependencyEventRaised.Set();
                                               };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";

                const string dependencyKey = "1";
                CacheItem cacheItem = new CacheItem( storedValue,
                                                     dependencies: ImmutableList.Create( dependencyKey ) );

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

        [Fact(Timeout = Timeout )]
        public async Task TestRemovalEventByDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( !cache.SupportedFeatures.Events )
                {
                    AssertEx.Inconclusive();
                    return;
                }

                TaskCompletionSource<bool> itemEventRaised = new TaskCompletionSource<bool>();
                CacheItemRemovedEventArgs itemEventArgs = null;
                cache.ItemRemoved += ( sender, args ) =>
                                     {
                                         
                                         itemEventArgs = args;
                                         itemEventRaised.SetResult( true );
                                     };

                TaskCompletionSource<bool> dependencyEventRaised = new TaskCompletionSource<bool>();
                CacheDependencyInvalidatedEventArgs dependencyEventArgs = null;
                cache.DependencyInvalidated += ( sender, args ) =>
                                               {
                                                   
                                                   dependencyEventArgs = args;
                                                   dependencyEventRaised.SetResult( true );
                                               };

                CachedValueClass storedValue = new CachedValueClass( 0 );
                const string key = "0";

                const string dependencyKey = "1";
                CacheItem cacheItem = new CacheItem( storedValue,
                                                     dependencies: ImmutableList.Create( dependencyKey ) );

                await cache.SetItemAsync( key, cacheItem );

                await cache.InvalidateDependencyAsync( dependencyKey );
                Assert.Null( await cache.GetItemAsync( key ) );

                Assert.True( await Task.WhenAll( itemEventRaised.Task, dependencyEventRaised.Task ).WithTimeout( TimeoutTimeSpan ) );

                //await cache.FlushAsync();

                
                AssertEx.NotNull( itemEventArgs, "The item event did not pass any arguments." );
                Assert.Equal( CacheItemRemovedReason.Invalidated, itemEventArgs.RemovedReason );

                
                AssertEx.NotNull( dependencyEventArgs, "The dependency event did not pass any arguments." );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }


        [Fact(Timeout = Timeout )]
        public void TestDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                List<CacheItemRemovedEventArgs> events = new List<CacheItemRemovedEventArgs>();
                cache.ItemRemoved += ( sender, args ) => events.Add( args );

                const string dependencyKey = "dependency";
                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), ImmutableList.Create( dependencyKey ) );
                cache.SetItem( "m1", cacheItem1 );

                this.GiveChanceToResetLocalCache(cache);

                Assert.Empty( events);
                Assert.NotNull( cache.GetItem( "m1" ) );
                cache.InvalidateDependency( dependencyKey );

                this.GiveChanceToResetLocalCache(cache);

                Assert.Null( cache.GetItem( "m1" ) );

                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    Assert.False( cache.ContainsDependency( dependencyKey ) );
                }

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                int eventsCount = 0;
                cache.ItemRemoved += ( sender, args ) => eventsCount++;

                const string dependencyKey = "dependency";
                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), ImmutableList.Create( dependencyKey ) );
                await cache.SetItemAsync( "m1", cacheItem1 );

                this.GiveChanceToResetLocalCache(cache);

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( await cache.GetItemAsync( "m1" ) );

                this.GiveChanceToResetLocalCache(cache);

                await cache.InvalidateDependencyAsync( dependencyKey );
                Assert.Null( await cache.GetItemAsync( "m1" ) );

                if ( cache.SupportedFeatures.ContainsDependency )
                {
                    Assert.False( await cache.ContainsDependencyAsync( dependencyKey ) );
                }

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestSharedDependency()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                int eventsCount = 0;
                cache.ItemRemoved += ( sender, args ) => eventsCount++;

                const string dependencyKey = "dependency";
                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                CacheItem cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                cache.SetItem( "m1", cacheItem1 );
                cache.SetItem( "m2", cacheItem2 );

                this.GiveChanceToResetLocalCache(cache);

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( cache.GetItem( "m1" ) );
                Assert.NotNull( cache.GetItem( "m2" ) );


                Assert.Equal( 0, eventsCount );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestSharedDependencyAsync()
        {
            if ( !this.TestDependencies )
            {
                AssertEx.Inconclusive();
                return;
            }

            using ( CachingBackend cache = this.CreateBackend() )
            {
                int eventsCount = 0;
                cache.ItemRemoved += ( sender, args ) => eventsCount++;

                const string dependencyKey = "dependency";
                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                CacheItem cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( dependencyKey ) : null );
                await cache.SetItemAsync( "m1", cacheItem1 );
                await cache.SetItemAsync( "m2", cacheItem2 );

                this.GiveChanceToResetLocalCache(cache);

                Assert.Equal( 0, eventsCount );
                Assert.NotNull( await cache.GetItemAsync( "m1" ) );
                Assert.NotNull( await cache.GetItemAsync( "m2" ) );

                
                Assert.Equal( 0, eventsCount );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestReplace()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {

                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( "d1" ) : null );
                CacheItem cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( "d2" ) : null );
                cache.SetItem( "m", cacheItem1 );
                cache.SetItem( "m", cacheItem2 );

                this.GiveChanceToResetLocalCache(cache);

                CacheValue retrievedValue = cache.GetItem( "m" );
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



        [Fact(Timeout = Timeout )]
        public async Task TestReplaceAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {

                CacheItem cacheItem1 = new CacheItem( new CachedValueClass( 1 ), this.TestDependencies ? ImmutableList.Create( "d1" ) : null );
                CacheItem cacheItem2 = new CacheItem( new CachedValueClass( 2 ), this.TestDependencies ? ImmutableList.Create( "d2" ) : null );
                await cache.SetItemAsync( "m", cacheItem1 );
                await cache.SetItemAsync( "m", cacheItem2 );

                this.GiveChanceToResetLocalCache(cache);

                CacheValue retrievedValue = await cache.GetItemAsync( "m" );
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

        [Fact(Timeout = Timeout )]
        public void TestSetItemWithDependencyWithoutSupport()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );

                Assert.Throws<NotSupportedException>( () =>
                {
                    cache.SetItem( "i", new CacheItem( "v", ImmutableList.Create( "d" ) ) );
                } );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public async Task TestSetItemWithDependencyWithoutSupportAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                await Assert.ThrowsAsync<NotSupportedException>( async () =>
                {
                    await cache.SetItemAsync( "i", new CacheItem( "v", ImmutableList.Create( "d" ) ) );
                } );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact(Timeout = Timeout )]
        public void TestInvalidateDependencyWithoutSupport()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                Assert.Throws<NotSupportedException>( () =>
                {
                    cache.InvalidateDependency( "d" );
                } );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestInvalidateDependencyWithoutSupportAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                await Assert.ThrowsAsync<NotSupportedException>( async () =>
                {
                    await cache.InvalidateDependencyAsync( "d" );
                } );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public void TestContainsDependencyWithoutSupport()
        {
            using ( CachingBackend cache = this.CreateBackend() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                Assert.False( cache.SupportedFeatures.ContainsDependency );
                Assert.Throws<NotSupportedException>( () =>
                {
                    cache.ContainsDependency( "d" );
                } );

                TestableCachingComponentDisposer.Dispose( cache );
            }
        }

        [Fact( Timeout = Timeout )]
        public async Task TestContainsDependencyWithoutSupportAsync()
        {
            using ( CachingBackend cache = await this.CreateBackendAsync() )
            {
                if ( this.TestDependencies )
                {
                    Assert.True( cache.SupportedFeatures.Dependencies );
                    AssertEx.Inconclusive();
                    return;
                }

                Assert.False( cache.SupportedFeatures.Dependencies );
                Assert.False( cache.SupportedFeatures.ContainsDependency );
                await Assert.ThrowsAsync<NotSupportedException>( async () =>
                {
                    await cache.ContainsDependencyAsync( "d" );
                } );

                await TestableCachingComponentDisposer.DisposeAsync( cache );
            }
        }

        class NullTestableCachingComponent : ITestableCachingComponent
        {
            public int BackgroundTaskExceptions
            {
                get { return 0; }
            }

            public void Dispose()
            {

            }

            public Task DisposeAsync( CancellationToken cancellationToken = default(CancellationToken) )
            {
                return PortableThreadingApi.CompletedTask;
            }
        }


    }



    static class TimeSpanExtensions
    {
        public static TimeSpan Multiply( this TimeSpan t, double m )
        {
            return TimeSpan.FromMilliseconds( t.TotalMilliseconds * m );
        }
    }

    
}