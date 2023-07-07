// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Temporary, initial development only. Remove or adapt to proper tests.
// ReSharper disable all
#pragma warning disable

// TODO: Work around #33441 : Some method calls in scope via `using static` are not transformed.
using Metalama.Patterns.Caching.Implementation;
using System.Runtime.CompilerServices;
using static Flashtrace.FormattedMessageBuilder;
using static Metalama.Patterns.Caching.Experiments.InfoWriter;

namespace Metalama.Patterns.Caching.Experiments;

public static class S_Int_WithInvalidator
{
    [Cache]
    public static int TimesTwo( int x )
    {
        Enter();
        return x * 2;
    }

    [InvalidateCache( nameof( TimesTwo ) )]
    public static void Invalidate( int x )
    {
    }
}
