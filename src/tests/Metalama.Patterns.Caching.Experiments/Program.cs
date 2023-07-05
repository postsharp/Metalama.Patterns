// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.Experiments;
using System.Text;

CachingServices.DefaultBackend = new Metalama.Patterns.Caching.Backends.MemoryCachingBackend();
Console.WriteLine( $"Invoke1 #1 got {await Invoke1()}" );
Console.WriteLine( $"Invoke1 #2 got {await Invoke1()}" );
Console.WriteLine( $"Invoke1 #3 got {await Invoke1()}" );
Console.WriteLine();
Console.WriteLine( $"Invoke2 #1 got {await Invoke2()}" );
Console.WriteLine( $"Invoke2 #2 got {await Invoke2()}" );
Console.WriteLine( $"Invoke2 #3 got {await Invoke2()}" );
Console.WriteLine();
Console.WriteLine( $"Invoke3 #1 got {await Invoke3()}" );
Console.WriteLine( $"Invoke3 #2 got {await Invoke3()}" );
Console.WriteLine( $"Invoke3 #3 got {await Invoke3()}" );
Console.WriteLine();
Console.WriteLine( $"Invoke4 #1 got {await Invoke4()}" );
Console.WriteLine( $"Invoke4 #2 got {await Invoke4()}" );
Console.WriteLine( $"Invoke4 #3 got {await Invoke4()}" );

static async Task<string> Invoke1()
{
    var sb = new StringBuilder();

    await foreach ( var item in S_AsyncEnumerable_DESIRED.OneTwoThree() )
    {
        sb.Append( item ).Append( "," );
    }

    return sb.ToString();
}

static async Task<string> Invoke2()
{
    var sb = new StringBuilder();

    var iter = S_AsyncEnumerator_DESIRED.GetEnumerator();

    while ( await iter.MoveNextAsync() )
    {
        sb.Append( iter.Current ).Append( "," );
    }

    return sb.ToString();
}

static async Task<string> Invoke3()
{
    var sb = new StringBuilder();

    await foreach ( var item in S_AsyncEnumerable.OneTwoThree() )
    {
        sb.Append( item ).Append( "," );
    }

    return sb.ToString();
}

static async Task<string> Invoke4()
{
    var sb = new StringBuilder();

    var iter = S_AsyncEnumerator.GetEnumerator();

    while ( await iter.MoveNextAsync() )
    {
        sb.Append( iter.Current ).Append( "," );
    }

    return sb.ToString();
}