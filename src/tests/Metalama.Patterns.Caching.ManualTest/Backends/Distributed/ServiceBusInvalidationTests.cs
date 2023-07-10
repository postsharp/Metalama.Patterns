// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: #if WINDOWS_AZURE_LEGACY_API

#if WINDOWS_AZURE_LEGACY_API
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;
using Xunit;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.Backends.Azure;

namespace PostSharp.Patterns.Caching.Tests.Backends
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
            return await AzureCacheInvalidator.CreateAsync( backend, new AzureCacheInvalidatorOptions
                                                       {
                                                           ConnectionString = connectionString,
                                                           Prefix = prefix
                                                       } );
        }

        protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
        {
            return AzureCacheInvalidator.Create( backend, new AzureCacheInvalidatorOptions
                                                                     {
                                                                         ConnectionString = connectionString,
                                                                         Prefix = prefix
                                                                     } );
        }
    }
}
#endif