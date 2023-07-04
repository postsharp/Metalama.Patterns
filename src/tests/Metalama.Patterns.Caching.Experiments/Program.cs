// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.Experiments;

Func<int, int> invoke = i => StaticInt.TimesTwo(i);

CachingServices.DefaultBackend = new Metalama.Patterns.Caching.Backends.MemoryCachingBackend();
Console.WriteLine( $"Invoke #1 got {invoke( 2 )}" );
Console.WriteLine( $"Invoke #2 got {invoke( 2 )}" );
Console.WriteLine( $"Invoke #3 got {invoke( 2 )}" );