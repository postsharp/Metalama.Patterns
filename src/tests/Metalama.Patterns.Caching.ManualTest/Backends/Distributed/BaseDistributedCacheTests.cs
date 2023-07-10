// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers.Shared;
using Metalama.Patterns.Common.Tests.Helpers;
using Metalama.Patterns.Diagnostics;

namespace Metalama.Patterns.Caching.ManualTest.Backends;

public abstract class BaseDistributedCacheTests
{
    private const int timeout = 120000; // 2 minutes ought to be enough to anyone. (otherwise the test should be refactored, anyway).
    private static readonly TimeSpan timeouTimeSpan = TimeSpan.FromMilliseconds( timeout * 0.8 );

    protected virtual bool TestDependencies { get; } = true;

    protected abstract Task<CachingBackend[]> CreateBackendsAsync();

    protected abstract CachingBackend[] CreateBackends();

    public BaseDistributedCacheTests( TestContext testContext )
    {
        this.TestContext = testContext;
    }

    /// <summary>
    /// If the actual class is a Redis test, connect to the persistent Redis server (or create the server, if it doesn't run yet).
    /// <para>
    /// If this is not a Redis test, do nothing.
    /// </para>
    /// </summary>
    protected virtual void ConnectToRedisIfRequired() { }

    protected TestContext TestContext { get; private set; }

    [Fact( Timeout = timeout )]
    public async Task TestInvalidateDependencyIdenticalItemsAsync()
    {
        if ( !this.TestDependencies )
        {
            AssertEx.Inconclusive();

            return;
        }

        this.ConnectToRedisIfRequired();

        const string dependencyKey = "d", itemKey = "i";

        CachingBackend[] backends = await this.CreateBackendsAsync();

        try
        {
            TaskCompletionSource<bool>[] itemRemovedEvents = new TaskCompletionSource<bool>[backends.Length];

            for ( var j = 0; j < backends.Length; j++ )
            {
                var index = j;
                backends[j].ItemRemoved += ( sender, args ) => itemRemovedEvents[index].TrySetResult( true );
            }

            for ( var j = 0; j < backends.Length; j++ )
            {
                CachingBackend backend1 = backends[j];
                Console.WriteLine( $"Testing with master backend #{j}: {backend1}" );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    itemRemovedEvents[i] = new TaskCompletionSource<bool>();
                    CachingBackend backend2 = backends[i];
                    await backend2.SetItemAsync( itemKey, new CacheItem( "Hello, world.", ImmutableList.Create( dependencyKey ) ) );
                }

                backend1.InvalidateDependency( dependencyKey );

                Assert.True(
                    await Task.WhenAll( itemRemovedEvents.Select( e => e.Task ) ).WithTimeout( timeouTimeSpan ),
                    "ItemsRemoved event not received." );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    var backend2 = backends[i];

                    Console.WriteLine( $"Testing with slave backend #{i}: {backend2}" );

                    Assert.False( await backend2.ContainsItemAsync( itemKey ), $"Backend {i} still contains the item." );
                    Assert.False( await backend2.ContainsDependencyAsync( dependencyKey ), $"Backend {i} still contains the dependency." );
                }
            }
        }
        finally
        {
            await TestableCachingComponentDisposer.DisposeAsync( backends );
        }
    }

    [Fact( Timeout = timeout )]
    public void TestInvalidateDependencyIdenticalItems()
    {
        if ( !this.TestDependencies )
        {
            AssertEx.Inconclusive();

            return;
        }

        this.ConnectToRedisIfRequired();
        const string dependencyKey = "d", itemKey = "i";

        CachingBackend[] backends = this.CreateBackends();

        try
        {
            ManualResetEventSlim[] itemRemovedEvents = new ManualResetEventSlim[backends.Length];

            for ( var j = 0; j < backends.Length; j++ )
            {
                var index = j;
                backends[j].ItemRemoved += ( sender, args ) => itemRemovedEvents[index].Set();
            }

            for ( var j = 0; j < 1; j++ )
            {
                CachingBackend backend1 = backends[j];
                Console.WriteLine( $"Testing with master backend #{j}: {backend1}" );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    itemRemovedEvents[i] = new ManualResetEventSlim( false );
                    CachingBackend backend2 = backends[i];
                    backend2.SetItem( itemKey, new CacheItem( "Hello, world.", ImmutableList.Create( dependencyKey ) ) );
                }

                backend1.InvalidateDependency( dependencyKey );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    Assert.True( itemRemovedEvents[i].Wait( timeout ) );
                }

                for ( var i = 0; i < backends.Length; i++ )
                {
                    Console.WriteLine( "Waiting..." );
                    var backend2 = backends[i];

                    Console.WriteLine( $"Testing with slave backend #{i}: {backend2}" );

                    Assert.False( backend2.ContainsItem( itemKey ), $"Backend {i} still contains the item." );
                    Assert.False( backend2.ContainsDependency( dependencyKey ), $"Backend {i} still contains the dependency." );
                }
            }
        }
        finally
        {
            TestableCachingComponentDisposer.Dispose( backends );
        }
    }

    [Fact( Timeout = timeout )]
    public async Task TestRemoveSharedItemAsync()
    {
        this.ConnectToRedisIfRequired();
        const string dependencyKey = "d", itemKey = "i";

        CachingBackend[] backends = await this.CreateBackendsAsync();

        try
        {
            TaskCompletionSource<bool>[] itemRemovedEvents = new TaskCompletionSource<bool>[backends.Length];

            for ( var j = 0; j < backends.Length; j++ )
            {
                var index = j;
                backends[j].ItemRemoved += ( sender, args ) => itemRemovedEvents[index].TrySetResult( true );
            }

            for ( var j = 0; j < backends.Length; j++ )
            {
                CachingBackend backend1 = backends[j];
                Console.WriteLine( $"Testing with master backend #{j}: {backend1}" );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    CachingBackend backend2 = backends[i];

                    itemRemovedEvents[i] = new TaskCompletionSource<bool>();

                    CacheItem cacheItem;

                    if ( this.TestDependencies )
                    {
                        cacheItem = new CacheItem( "Hello, world.", ImmutableList.Create( dependencyKey ) );
                    }
                    else
                    {
                        cacheItem = new CacheItem( "Hello, world." );
                    }

                    await backend2.SetItemAsync( itemKey, cacheItem );
                }

                await backend1.RemoveItemAsync( itemKey );

                Assert.True( await Task.WhenAll( itemRemovedEvents.Select( e => e.Task ) ).WithTimeout( timeouTimeSpan ) );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    var backend2 = backends[i];

                    Console.WriteLine( $"Testing with slave backend #{i}: {backend2}" );

                    Assert.False( await backend2.ContainsItemAsync( itemKey ), $"Backend {i} still contains the item." );

                    if ( this.TestDependencies )
                    {
                        Assert.False( await backend2.ContainsDependencyAsync( dependencyKey ), $"Backend {i} still contains the dependency." );
                    }
                }
            }
        }
        finally
        {
            await TestableCachingComponentDisposer.DisposeAsync( backends );
        }
    }

    [Fact( Timeout = timeout )]
    public async Task TestInvalidateDependencyDifferentItemsAsync()
    {
        if ( !this.TestDependencies )
        {
            AssertEx.Inconclusive();

            return;
        }

        this.ConnectToRedisIfRequired();
        const string dependencyKey = "d";

        CachingBackend[] backends = await this.CreateBackendsAsync();

        try
        {
            TaskCompletionSource<bool>[] itemRemovedEvents = new TaskCompletionSource<bool>[backends.Length];

            for ( var j = 0; j < backends.Length; j++ )
            {
                var index = j;
                backends[j].ItemRemoved += ( sender, args ) => itemRemovedEvents[index].TrySetResult( true );
            }

            for ( var j = 0; j < backends.Length; j++ )
            {
                CachingBackend backend1 = backends[j];

                Console.WriteLine( $"Testing with master backend #{j}: {backend1}" );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    CachingBackend backend2 = backends[i];
                    await backend2.SetItemAsync( string.Format( "i{0}", i ), new CacheItem( "Hello, world.", ImmutableList.Create( dependencyKey ) ) );
                    itemRemovedEvents[i] = new TaskCompletionSource<bool>();
                }

                await backend1.InvalidateDependencyAsync( dependencyKey );

                Assert.True( await Task.WhenAll( itemRemovedEvents.Select( e => e.Task ) ).WithTimeout( timeouTimeSpan ) );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    var backend2 = backends[i];

                    Console.WriteLine( $"Testing with slave backend #{i}: {backend2}" );

                    Assert.False(
                        await backends[i].ContainsItemAsync( string.Format( "i{0}", i ) ),
                        $"Backend {i} still contains the item but it was invalidated by backend {j}." );

                    Assert.False(
                        await backends[i].ContainsDependencyAsync( dependencyKey ),
                        $"Backend {i} still contains the dependency but it was invalidated by backend {j}." );
                }
            }
        }
        finally
        {
            await TestableCachingComponentDisposer.DisposeAsync( backends );
        }
    }

    [Fact( Timeout = timeout )]
    public async Task TestRemoveDifferentItemsAsync()
    {
        this.ConnectToRedisIfRequired();
        const string dependencyKey = "d";

        CachingBackend[] backends = await this.CreateBackendsAsync();

        try
        {
            TaskCompletionSource<bool>[] itemRemovedEvents = new TaskCompletionSource<bool>[backends.Length];

            for ( var j = 0; j < backends.Length; j++ )
            {
                var index = j;
                backends[j].ItemRemoved += ( sender, args ) => itemRemovedEvents[index].TrySetResult( true );
            }

            for ( var j = 0; j < backends.Length; j++ )
            {
                CachingBackend backend1 = backends[j];

                Console.WriteLine( $"Testing with master backend #{j}: {backend1}" );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    itemRemovedEvents[i] = new TaskCompletionSource<bool>();

                    CachingBackend backend2 = backends[i];
                    CacheItem cacheItem;

                    if ( this.TestDependencies )
                    {
                        cacheItem = new CacheItem( "Hello, world.", ImmutableList.Create( dependencyKey ) );
                    }
                    else
                    {
                        cacheItem = new CacheItem( "Hello, world." );
                    }

                    await backend2.SetItemAsync( string.Format( "i{0}", i ), cacheItem );
                }

                for ( var i = 0; i < backends.Length; i++ )
                {
                    // We intentionally remove from a different backend than the one that added.
                    await backend1.RemoveItemAsync( string.Format( "i{0}", i ) );
                }

                Assert.True( await Task.WhenAll( itemRemovedEvents.Select( e => e.Task ) ).WithTimeout( timeouTimeSpan ) );

                for ( var i = 0; i < backends.Length; i++ )
                {
                    Console.WriteLine( $"Testing with slave backend #{i}: {backends[i]}" );

                    Assert.False( backends[i].ContainsItem( string.Format( "i{0}", i ) ), $"Backend {i} still contains the item." );

                    if ( this.TestDependencies )
                    {
                        Assert.False( backends[i].ContainsDependency( dependencyKey ), $"Backend {i} still contains the dependency." );
                    }
                }
            }
        }
        finally
        {
            await TestableCachingComponentDisposer.DisposeAsync( backends );
        }
    }
}