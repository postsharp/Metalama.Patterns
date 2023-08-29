// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers;

public abstract class BaseCachingTests
{
    protected BaseCachingTests( ITestOutputHelper testOutputHelper )
    {
        this.TestOutputHelper = testOutputHelper;
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILoggerFactory>( new XUnitLoggerFactory( testOutputHelper ) );
        this.ServiceProvider = serviceCollection.BuildServiceProvider();
        CachingServices.Default = new CachingService( this.ServiceProvider );
    }

    protected ServiceProvider ServiceProvider { get; }

    protected ITestOutputHelper TestOutputHelper { get; }

    protected CachingBackend InitializeTestWithCachingBackend( string name )
    {
        return TestProfileConfigurationFactory.InitializeTestWithCachingBackend( name, this.ServiceProvider );
    }

    protected TestingCacheBackend InitializeTestWithTestingBackend( string name )
    {
        return TestProfileConfigurationFactory.InitializeTestWithTestingBackend( name, this.ServiceProvider );
    }
}