// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETSTANDARD || NETCOREAPP

using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

#if ENABLE_AZURE_TESTS
public
#else
internal
#endif
class AzureServiceBusInvalidationTests2 : BaseInvalidationBrokerTests
{
    private const string connectionString =
        "Endpoint=sb://petrservicebusstandard.servicebus.windows.net/;SharedAccessKeyName=PetrSAS;SharedAccessKey=3C+I8BExn5AMRaxXJk4kTINM0f2uXCPWKtWAdmGgpQI=;EntityPath=petrtopic";

    private const string appName = "PetrServiceBusManager";

    private const string clientId = "95e2b555-60ed-49dd-8da7-7a5574f9b4f7";
    private const string tenantId = "171276b2-7a8c-4c9b-bc49-57889e2e4f42";
    private const string objectId = "c7003be7-3118-4b21-bad3-3d4e7a484f2b";
    private const string secret = "[?1x=eI@LabKmywBXxfNh5e67PNz3ZKm";

    public AzureServiceBusInvalidationTests2( TestContext testContext ) : base( testContext ) { }

    protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
    {
        return await AzureCacheInvalidator2.CreateAsync( backend, this.CreateOptions() );
    }

    private AzureCacheInvalidatorOptions2 CreateOptions()
    {
        return new AzureCacheInvalidatorOptions2.NewSubscription(
            connectionString,
            clientId,
            secret,
            tenantId,
            "PetrResourceGroup",
            "PetrServiceBusStandard",
            "petrtopic",
            "bd0b2c2c-a1b5-48a0-ae37-28713cf3f27c" );
    }

    protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
    {
        return AzureCacheInvalidator2.Create( backend, this.CreateOptions() );
    }
}

#endif