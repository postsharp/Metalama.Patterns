// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.ManualTest.Backends
{
    public class ServiceBusInvalidationTests : BaseInvalidationBrokerTests
    {
        private const string connectionString =
                "Endpoint=sb://postsharp-test.servicebus.windows.net/;SharedAccessKeyName=TestClient;SharedAccessKey=NngSkkAP1Ve9djFW29FbMT5orajCe+y8T9f1URLVzh8=;EntityPath=cacheinvalidationtest";

        public ServiceBusInvalidationTests( TestContext testContext ) : base( testContext )
        {
        }

        protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
        {
            return await AzureCacheInvalidator.CreateAsync( backend, new AzureCacheInvalidatorOptions( connectionString )
                                                       {
                                                           Prefix = prefix
                                                       } );
        }

        protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
        {
            return AzureCacheInvalidator.Create( backend, new AzureCacheInvalidatorOptions( connectionString )
                                                                     {
                                                                         Prefix = prefix
                                                                     } );
        }
    }
}

#endif