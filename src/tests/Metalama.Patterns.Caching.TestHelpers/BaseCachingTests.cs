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
        this.ServiceCollection = new ServiceCollection();
        this.ServiceCollection.AddSingleton<ILoggerFactory>( new XUnitLoggerFactory( testOutputHelper ) );
        this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();
    }

    public ServiceProvider ServiceProvider { get; }

    public ServiceCollection ServiceCollection { get; }

    protected ITestOutputHelper TestOutputHelper { get; }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public CachingBackend InitializeTestWithCachingBackend( string name )
    {
        return TestProfileConfigurationFactory.InitializeTestWithCachingBackend( name, this.ServiceProvider );
    }

    public TestingCacheBackend InitializeTestWithTestingBackend( string name )
    {
        return TestProfileConfigurationFactory.InitializeTestWithTestingBackend( name, this.ServiceProvider );
    }
}