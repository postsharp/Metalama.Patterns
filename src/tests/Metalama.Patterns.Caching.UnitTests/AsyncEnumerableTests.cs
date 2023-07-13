﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETCOREAPP3_0_OR_GREATER
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests;

public sealed class AsyncEnumerableTests : AsyncEnumTestsBase
{
    public AsyncEnumerableTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }

    [Fact]
    public void DoesNotIterateOnUnawaitedMethod()
    {
        _ = this.Instance.CachedEnumerable();

        Assert.Equal( "E1", this.StringBuilder.ToString() );
    }

    [Fact]
    public void DoesNotIterateOnGetAsyncEnumerator()
    {
        _ = this.Instance.CachedEnumerable().GetAsyncEnumerator();

        Assert.Equal( "E1", this.StringBuilder.ToString() );
    }

    [Fact]
    public void DoesNotIterateOnUnawaitedFirstMoveNextAsync()
    {
        _ = this.Instance.CachedEnumerable().GetAsyncEnumerator().MoveNextAsync();

        Assert.Equal( "E1", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesCompletelyOnFirstAwaitedMoveNextAsync()
    {
        _ = await this.Instance.CachedEnumerable().GetAsyncEnumerator().MoveNextAsync();

        Assert.Equal( "E1.E2.E3.E4", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void DoesNotIteratesOnSecondAwaitedMoveNextAsync()
    {
        _ = await this.Instance.CachedEnumerable().GetAsyncEnumerator().MoveNextAsync();
        this.StringBuilder.Clear();
        _ = await this.Instance.CachedEnumerable().GetAsyncEnumerator().MoveNextAsync();

        Assert.Equal( "", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesExpectedSequence1()
    {
        await this.Iterate( this.Instance.CachedEnumerable().GetAsyncEnumerator() );

        Assert.Equal( "E1.I1.E2.E3.E4.I2[42].I2[99].I3", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesExpectedSequence2()
    {
        await this.Iterate( this.Instance.CachedEnumerable().GetAsyncEnumerator() );
        await this.Iterate( this.Instance.CachedEnumerable().GetAsyncEnumerator() );

        Assert.Equal( "E1.I1.E2.E3.E4.I2[42].I2[99].I3.I1.I2[42].I2[99].I3", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesExpectedSequence3()
    {
        var seq = this.Instance.CachedEnumerable();
        
        // ReSharper disable once PossibleMultipleEnumeration
        await this.Iterate( seq.GetAsyncEnumerator() );
        
        // ReSharper disable once PossibleMultipleEnumeration
        await this.Iterate( seq.GetAsyncEnumerator() );

        Assert.Equal( "E1.I1.E2.E3.E4.I2[42].I2[99].I3.I1.I2[42].I2[99].I3", this.StringBuilder.ToString() );
    }
}

#endif