﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Temporary, initial development only. Remove or adapt to proper tests.
// ReSharper disable all
#pragma warning disable

// TODO: Work around #33441 : Some method calls in scope via `using static` are not transformed.
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Experiments;

public static class S_Enumerator
{
    [Cache]
    public static IEnumerator<int> TimesTwo( params int[] values )
    {
        return (IEnumerator<int>) values.GetEnumerator();
    }
}

public static class S_AsyncEnumerable
{
    [Cache]
    public static async IAsyncEnumerable<int> OneTwoThree()
    {
        await Task.Delay( 1 );
        yield return 1;
        await Task.Delay( 1 );
        yield return 2;
        await Task.Delay( 1 );
        yield return 3;
    }
}

public static class S_AsyncEnumerator
{
    [Cache]
    public static IAsyncEnumerator<int> GetEnumerator()
        => OneTwoThree().GetAsyncEnumerator();

    private static async IAsyncEnumerable<int> OneTwoThree()
    {
        await Task.Delay( 1 );
        yield return 1;
        await Task.Delay( 1 );
        yield return 2;
        await Task.Delay( 1 );
        yield return 3;
    }
}

public sealed class I_S_YieldingEnumerable
{
    [Cache]
    public IEnumerable<int> TimesTwo( params int[] values )
    {
        foreach ( var value in values )
        {
            yield return value * 2;
        }
    }
}

public sealed class I_S_IntAsyncTask
{
    [Cache]
    public static async Task<int> TimesTwo( int x )
    {
        return x * 2;
    }
}

public sealed class I_S_IntAsyncValueTask
{
    [Cache]
    public static async ValueTask<int> TimesTwo( int x )
    {
        return x * 2;
    }
}

public sealed class I_I_Int
{
    [Cache]
    public int TimesTwo( int x )
    {
        return x * 2;
    }
}

public static class S_Int
{
    [Cache]
    public static int TimesTwo( int x )
    {
        Console.WriteLine( ">> {0}", System.Reflection.MethodBase.GetCurrentMethod().Name );
        return x * 2;
    }
}

public sealed class I_I_String
{
    [Cache]
    public string Reverse( string x )
    {
        return new string( x.Reverse().ToArray() );
    }
}

public sealed class I_S_String
{
    [Cache]
    public static string Reverse( string x )
    {
        return new string( x.Reverse().ToArray() );
    }
}

public sealed class I_S_TwoCachedMethods
{
    [Cache]
    public static int TimesTwo( int x )
    {
        return x * 2;
    }

    [Cache]
    public static int TimesThree( int x )
    {
        return x * 3;
    }
}

public sealed class I_S_TwoCachedMethodsSameName
{
    [Cache]
    public static int TimesTwo( int x )
    {
        return x * 2;
    }

    [Cache]
    public static double TimesTwo( double x )
    {
        return x * 3;
    }
}