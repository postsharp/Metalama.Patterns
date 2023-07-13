// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETCOREAPP3_0_OR_GREATER
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests;

public class AsyncEnumeratorTests : AsyncEnumTestsBase
{
    public AsyncEnumeratorTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }

    [Fact]
    public void DoesNotIterateOnUnawaitedMethod()
    {
        _ = this.Instance.CachedEnumerator();

        Assert.Equal( "E1", this.StringBuilder.ToString() );
    }

    [Fact]
    public void DoesNotIterateOnUnawaitedFirstMoveNextAsync()
    {
        _ = this.Instance.CachedEnumerator().MoveNextAsync();

        Assert.Equal( "E1", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesCompletelyOnFirstAwaitedMoveNextAsync()
    {
        _ = await this.Instance.CachedEnumerator().MoveNextAsync();

        Assert.Equal( "E1.E2.E3.E4", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void DoesNotIteratesOnSecondAwaitedMoveNextAsync()
    {
        _ = await this.Instance.CachedEnumerator().MoveNextAsync();
        this.StringBuilder.Clear();
        _ = await this.Instance.CachedEnumerator().MoveNextAsync();

        Assert.Equal( "", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesExpectedSequence1()
    {
        await this.Iterate( this.Instance.CachedEnumerator() );

        Assert.Equal( "E1.I1.E2.E3.E4.I2[42].I2[99].I3", this.StringBuilder.ToString() );
    }

    [Fact]
    public async void IteratesExpectedSequence2()
    {
        await this.Iterate( this.Instance.CachedEnumerator() );
        await this.Iterate( this.Instance.CachedEnumerator() );

        Assert.Equal( "E1.I1.E2.E3.E4.I2[42].I2[99].I3.I1.I2[42].I2[99].I3", this.StringBuilder.ToString() );
    }
}

#endif