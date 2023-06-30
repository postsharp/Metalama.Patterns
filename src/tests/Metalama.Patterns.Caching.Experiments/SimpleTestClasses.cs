// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Experiments;

// TODO: [Porting] Temporary, initial development only. Remove or adapt to proper tests.
// ReSharper disable all
#pragma warning disable

public sealed class InstanceInt
{
    [Cache]
    public int TimesTwo( int x )
    {
        return x * 2;
    }
}

public sealed class StaticInt
{
    [Cache]
    public static int TimesTwo( int x )
    {
        return x * 2;
    }
}

public sealed class InstanceString
{
    [Cache]
    public string Reverse( string x )
    {
        return new string( x.Reverse().ToArray() );
    }
}

public sealed class StaticString
{
    [Cache]
    public static string Reverse( string x )
    {
        return new string( x.Reverse().ToArray() );
    }
}

public sealed class TwoCachedMethods
{
    [Cache]
    public static int TimesTwo( int x )
    {
        return x * 2;
    }

#if false
    [Cache]
    public static int TimesThree( int x )
    {
        return x * 3;
    }
#endif
}