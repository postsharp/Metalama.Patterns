// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Building;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers;

public abstract class BaseCachingTests
{
    protected BaseCachingTests( ITestOutputHelper testOutputHelper )
    {
        this.TestOutputHelper = testOutputHelper;
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IFlashtraceLoggerFactory>( new XUnitLoggerFactory( testOutputHelper ) );
        this.ServiceProvider = serviceCollection.BuildServiceProvider();
        CachingService.Default = CachingService.CreateUninitialized( this.ServiceProvider );
    }

    protected ServiceProvider ServiceProvider { get; }

    protected ITestOutputHelper TestOutputHelper { get; }

    private static void ResetCachingServices()
    {
        Assert.True(
            CachingService.Default.DefaultBackend is UninitializedCachingBackend or NullCachingBackend,
            "Each test has to use the TestProfileConfigurationFactory." );

        var uninitialized = CachingService.CreateUninitialized();
        CachingService.Default = uninitialized;
    }

    protected CachingTestContext<T> InitializeTest<T>(
        string name,
        T backend,
        Action<CachingTestBuilder>? buildTest = null,
        bool passServiceProvider = true )
        where T : CachingBackend
    {
        ResetCachingServices();

        CachingService.Default = CachingService.Create(
            b =>
            {
                b.WithBackend( x => x.Specific( backend ) );

                var testBuilder = new CachingTestBuilder( b );
                buildTest?.Invoke( testBuilder );
                b.AddProfile( new CachingProfile( name ), true );
            },
            passServiceProvider ? this.ServiceProvider : null );

        return new CachingTestContext<T>( backend );
    }

    protected CachingTestContext<CachingBackend> InitializeTest(
        string name,
        Action<CachingTestBuilder>? buildTest = null,
        bool passServiceProvider = true )
    {
        var backend = CachingBackend.Create( b => b.Memory(), passServiceProvider ? this.ServiceProvider : null );
        backend.DebugName = name;

        return this.InitializeTest( name, backend, buildTest, passServiceProvider );
    }

    protected CachingTestContext<TestingCacheBackend> InitializeTestWithTestingBackend(
        string name,
        Action<CachingTestBuilder>? buildTest = null )
    {
        var backend = new TestingCacheBackend( "test-" + name, this.ServiceProvider );

        return this.InitializeTest( name, backend, buildTest );
    }
}