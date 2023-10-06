// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Formatters;
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

    internal static void ResetCachingServices()
    {
        Assert.True(
            CachingService.Default.DefaultBackend is UninitializedCachingBackend or NullCachingBackend,
            "Each test has to use the TestProfileConfigurationFactory." );

        var uninitialized = CachingService.CreateUninitialized();
        CachingService.Default = uninitialized;
    }

    protected CachingTestContext<MemoryCachingBackend> InitializeTestWithCachingBackend(
        string name,
        Func<IFormatterRepository, CacheKeyBuilder>? keyBuilderFactory = null )
    {
        ResetCachingServices();
        var backend = new MemoryCachingBackend( new MemoryCachingBackendConfiguration { ServiceProvider = this.ServiceProvider } ) { DebugName = name };
        CachingService.Default = new CachingService( backend, keyBuilderFactory, serviceProvider: this.ServiceProvider );

        return new CachingTestContext<MemoryCachingBackend>( backend );
    }

    protected CachingTestContext<TestingCacheBackend> InitializeTestWithTestingBackend( string name )
    {
        ResetCachingServices();
        var backend = new TestingCacheBackend( "test-" + name, this.ServiceProvider );
        CachingService.Default = new CachingService( backend, serviceProvider: this.ServiceProvider );

        return new CachingTestContext<TestingCacheBackend>( backend );
    }
}