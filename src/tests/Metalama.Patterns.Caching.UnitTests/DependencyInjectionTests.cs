// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests;

public sealed class DependencyInjectionTests : BaseCachingTests
{
    public DependencyInjectionTests( ITestOutputHelper testOutputHelper ) : base( testOutputHelper ) { }

    [Fact]
    public void TestDependencyInjection()
    {
        ServiceCollection serviceCollection = new();
        var cachingService = new CachingService( this.ServiceProvider );
        var backend = new TestingCacheBackend( "test", this.ServiceProvider );
        cachingService.DefaultBackend = backend;
        serviceCollection.AddSingleton( cachingService );
        serviceCollection.AddSingleton<C>();
        var c = (C) serviceCollection.BuildServiceProvider().GetService( typeof(C) )!;
        _ = c.Method();

        Assert.NotNull( backend.LastCachedItem );
        Assert.Equal( "DependencyInjection!", backend.LastCachedItem.Value );
    }

    private sealed class C
    {
        [Cache( UseDependencyInjection = true )]
        
        // ReSharper disable once MemberCanBeMadeStatic.Local
#pragma warning disable CA1822
        public string Method() => "DependencyInjection!";
#pragma warning restore CA1822
    }
}