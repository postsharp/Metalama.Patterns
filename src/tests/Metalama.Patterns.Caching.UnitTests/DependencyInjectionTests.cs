// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Aspects;
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
        var backend = new TestingCacheBackend( "test", this.ServiceProvider );
        serviceCollection.AddFlashtrace( b => b.EnabledRoles.Add( FlashtraceRoles.Caching ) );
        serviceCollection.AddCaching( this.InitializeCaching );
        serviceCollection.AddSingleton<C>();
        var c = (C) serviceCollection.BuildServiceProvider().GetService( typeof(C) )!;
        _ = c.Method();

        Assert.NotNull( backend.LastCachedItem );
        Assert.Equal( "DependencyInjection!", backend.LastCachedItem.Value );
    }

    private void InitializeCaching( CachingServiceBuilder builder )
    {
        builder.Backend = new TestingCacheBackend( "test", builder.ServiceProvider );
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