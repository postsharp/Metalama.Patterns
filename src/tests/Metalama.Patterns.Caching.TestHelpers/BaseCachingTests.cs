// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers;

public class TestBuilder
{
    public List<CachingProfile> Profiles { get; } = new();

    public Func<IFormatterRepository, CacheKeyBuilder>? KeyBuilderFactory { get; private set; }

    public TestBuilder WithProfile( string name ) => this.WithProfile( new CachingProfile( name ) );

    public TestBuilder WithProfile( CachingProfile profile )
    {
        this.Profiles.Add( profile );

        return this;
    }

    public TestBuilder WithKeyBuilder( Func<IFormatterRepository, CacheKeyBuilder> factory )
    {
        this.KeyBuilderFactory = factory;

        return this;
    }
}

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
        Action<TestBuilder>? buildTest = null )
        where T : CachingBackend
    {
        ResetCachingServices();
        var testBuilder = new TestBuilder();
        buildTest?.Invoke( testBuilder );

        // We always add a profile named after the test because many tests use this trick.
        if ( testBuilder.Profiles.All( p => p.Name != name ) )
        {
            testBuilder.Profiles.Add( new CachingProfile( name ) );
        }

        CachingService.Default = new CachingService(
            backend,
            profiles: testBuilder.Profiles,
            keyBuilderFactory: testBuilder.KeyBuilderFactory,
            serviceProvider: this.ServiceProvider );

        return new CachingTestContext<T>( backend );
    }

    protected CachingTestContext<MemoryCachingBackend> InitializeTest(
        string name,
        Action<TestBuilder>? buildTest = null )
    {
        var backend = new MemoryCachingBackend( new MemoryCachingBackendConfiguration { ServiceProvider = this.ServiceProvider } ) { DebugName = name };

        return this.InitializeTest( name, backend, buildTest );
    }

    protected CachingTestContext<TestingCacheBackend> InitializeTestWithTestingBackend(
        string name,
        Action<TestBuilder>? buildTest = null )
    {
        var backend = new TestingCacheBackend( "test-" + name, this.ServiceProvider );

        return this.InitializeTest( name, backend, buildTest );
    }
}