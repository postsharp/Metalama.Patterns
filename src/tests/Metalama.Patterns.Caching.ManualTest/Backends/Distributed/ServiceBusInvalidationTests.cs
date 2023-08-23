// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed
{
    // ReSharper disable once UnusedType.Global
    public class ServiceBusInvalidationTests : BaseInvalidationBrokerTests
    {
        private const string _connectionString =
            "Endpoint=sb://postsharp-test.servicebus.windows.net/;SharedAccessKeyName=TestClient;SharedAccessKey=NngSkkAP1Ve9djFW29FbMT5orajCe+y8T9f1URLVzh8=;EntityPath=cacheinvalidationtest";

        public ServiceBusInvalidationTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

        protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
        {
            return await AzureCacheInvalidator.CreateAsync(
                backend,
                new AzureCacheInvalidatorOptions.NewSubscription( _connectionString ) { Prefix = prefix } );
        }
    }
}