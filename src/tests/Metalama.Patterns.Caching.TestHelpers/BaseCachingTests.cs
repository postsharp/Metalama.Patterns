// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
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
        Action<CachingTestBuilder>? buildTest = null )
        where T : CachingBackend
    {
        ResetCachingServices();

        CachingService.Default = CachingService.Create(
            b =>
            {
                b.Backend = backend;

                var testBuilder = new CachingTestBuilder( b );
                buildTest?.Invoke( testBuilder );

                // We always add a profile named after the test because many tests use this trick.
                if ( b.Profiles.All( p => p.Name != name ) )
                {
                    b.AddProfile( new CachingProfile( name ) );
                }
            },
            this.ServiceProvider );

        return new CachingTestContext<T>( backend );
    }

    protected CachingTestContext<MemoryCachingBackend> InitializeTest(
        string name,
        Action<CachingTestBuilder>? buildTest = null )
    {
        var backend = new MemoryCachingBackend( this.ServiceProvider ) { DebugName = name };

        return this.InitializeTest( name, backend, buildTest );
    }

    protected CachingTestContext<TestingCacheBackend> InitializeTestWithTestingBackend(
        string name,
        Action<CachingTestBuilder>? buildTest = null )
    {
        var backend = new TestingCacheBackend( "test-" + name, this.ServiceProvider );

        return this.InitializeTest( name, backend, buildTest );
    }
}