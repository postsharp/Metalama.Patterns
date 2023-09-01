// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Locking;
using Metalama.Patterns.Caching.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class CacheLockTests : BaseCachingTests, IDisposable
    {
        private int _counter;

        public CacheLockTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper )
        {
            Console.WriteLine( "TestInitialize" );
            this.InitializeTestWithCachingBackend( "CacheLockTests" );
            CachingService.Default.Profiles["LocalLock"].LockManager = new LocalLockManager();
        }

        public void Dispose()
        {
            TestProfileConfigurationFactory.DisposeTest();
        }

        [Fact]
        public void TestSyncLock()
        {
            var t1 = Task.Run( () => this.TestLoop() );
            var t2 = Task.Run( () => this.TestLoop() );

            t1.Wait();
            t2.Wait();
        }

        [Fact]
        public async Task TestAsyncLock()
        {
            var t1 = Task.Run( this.TestLoopAsync );
            var t2 = Task.Run( this.TestLoopAsync );

            await Task.WhenAll( t1, t2 );
        }

        [Fact]
        public void TestSyncLockTimeout()
        {
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( 2 );

            Task t1 = Task.Run( () => this.CachedMethod( 100 ) );
            Task t2 = Task.Run( () => this.CachedMethod( 100 ) );

            try
            {
                t1.Wait();
                t2.Wait();

                AssertEx.Fail( "An exception was expected" );
            }
            catch ( AggregateException e )
            {
                Assert.NotNull( e.InnerExceptions[0] );
                Assert.IsType<TimeoutException>( e.InnerExceptions[0] );
            }

            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( -1 );

            // After a method completes with a timeout, the next call should be successful.
            try
            {
                t1.Wait();
            }
            catch
            {
                // ignored
            }

            try
            {
                t2.Wait();
            }
            catch
            {
                // ignored
            }

            this.CachedMethod();
        }

        [Fact]
        public async Task TestAsyncLockTimeout()
        {
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( 50 );

            var barrier = new AsyncBarrier( 2 );
            Task t1 = Task.Run( async () => await this.CachedMethodAsync( 100, barrier ) );

            Task t2 = Task.Run(
                async () =>
                {
                    await barrier.SignalAndWait();

                    return await this.CachedMethodAsync( 100 );
                } );

            try
            {
                await Task.WhenAll( t1, t2 );

                Assert.Equal( 0, this._counter );
                AssertEx.Fail( "An exception was expected" );
            }
            catch ( TimeoutException ) { }

            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( -1 );

            // After a method completes with a timeout, the next call should be successful.
            try
            {
                await t1;
            }
            catch
            {
                // ignored
            }

            try
            {
                await t2;
            }
            catch
            {
                // ignored
            }

            await this.CachedMethodAsync();
        }

        private static readonly TimeSpan _globalTimeout = TimeSpan.FromSeconds( 10 );

        [Fact]
        public void TestSyncLockTimeoutIgnoreLock()
        {
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( 2 );
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeoutStrategy = new IgnoreLockStrategy();

            Task t1 = Task.Run( () => this.CachedMethod( 100, assert: false ) );
            Task t2 = Task.Run( () => this.CachedMethod( 100, assert: false ) );

            Assert.True( t1.Wait( _globalTimeout ), "Timeout" );
            Assert.True( t2.Wait( _globalTimeout ), "Timeout" );

            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( -1 );
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeoutStrategy = null;

            // After a method completes with a timeout, the next call should be successful.
            this.CachedMethod();
        }

        [Fact]
        public async Task TestAsyncLockTimeoutAsync()
        {
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( 2 );
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeoutStrategy = new IgnoreLockStrategy();

            var barrier = new AsyncBarrier( 2 );
            var t1State = 0;

            var t1 = Task.Run(
                async () =>
                {
                    t1State = 1;
                    await this.CachedMethodAsync( 100, barrier, assert: false );
                    t1State = 2;
                } );

            var t2State = 0;

            var t2 = Task.Run(
                async () =>
                {
                    t2State = 1;
                    await barrier.SignalAndWait();
                    t2State = 2;
                    await this.CachedMethodAsync( 100, assert: false );
                    t2State = 3;
                } );

            var delay = Task.Delay( _globalTimeout );
            var t = await Task.WhenAny( Task.WhenAll( t1, t2 ), delay );

            AssertEx.NotSame( delay, t, $"Timeout. t1={t1.Status}, t1State={t1State}, t2={t2.Status}, t2State={t2State}, barrier={barrier}." );

            Assert.Equal( 0, this._counter );

            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeout = TimeSpan.FromMilliseconds( -1 );
            CachingService.Default.Profiles["LocalLock"].AcquireLockTimeoutStrategy = null;

            await this.CachedMethodAsync();
        }

        private void TestLoop()
        {
            for ( var i = 0; i < 1000; i++ )
            {
                this.CachedMethod( 0 );
                CachingService.Default.Invalidate( this.CachedMethod, 0, (Barrier?) null, true );
            }
        }

        [Cache( ProfileName = "LocalLock" )]
        private int CachedMethod( [NotCacheKey] int sleepTime = 1, [NotCacheKey] Barrier? barrier = null, [NotCacheKey] bool assert = true )
        {
            if ( assert )
            {
                Assert.Equal( 1, Interlocked.Increment( ref this._counter ) );
            }

            barrier?.SignalAndWait();

            Thread.Sleep( sleepTime );

            if ( assert )
            {
                Assert.Equal( 0, Interlocked.Decrement( ref this._counter ) );
            }

            return 1;
        }

        private async Task TestLoopAsync()
        {
            for ( var i = 0; i < 1000; i++ )
            {
                await this.CachedMethodAsync( 0 );
                await CachingService.Default.InvalidateAsync( this.CachedMethodAsync, 0, (AsyncBarrier?) null, true );
            }
        }

        [Cache( ProfileName = "LocalLock" )]
        private async Task<int> CachedMethodAsync(
            [NotCacheKey] int sleepTime = 1,
            [NotCacheKey] AsyncBarrier? barrier = null,
            [NotCacheKey] bool assert = true )
        {
            if ( assert )
            {
                Assert.Equal( 1, Interlocked.Increment( ref this._counter ) );
            }

            if ( barrier != null )
            {
                await barrier.SignalAndWait();
            }

            await Task.Delay( sleepTime );

            if ( assert )
            {
                Assert.Equal( 0, Interlocked.Decrement( ref this._counter ) );
            }

            return 1;
        }
    }
}