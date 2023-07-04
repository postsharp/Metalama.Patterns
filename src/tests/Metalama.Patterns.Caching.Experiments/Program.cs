// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.Experiments;
using System.Text;

CachingServices.DefaultBackend = new Metalama.Patterns.Caching.Backends.MemoryCachingBackend();
Console.WriteLine( $"Invoke #1 got {await invoke()}" );
Console.WriteLine( $"Invoke #2 got {await invoke()}" );
Console.WriteLine( $"Invoke #3 got {await invoke()}" );

static async Task<string> invoke()
{
    var sb = new StringBuilder();

    await foreach ( var item in S_AsyncEnumerable.OneTwoThree() )
    {
        sb.Append( item ).Append( "," );
    }

    return sb.ToString();
}

static async Task<bool> ThingToCache()
{
    return await S_AsyncEnumerable.OneTwoThree().GetAsyncEnumerator().MoveNextAsync();
}