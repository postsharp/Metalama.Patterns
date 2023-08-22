// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETSTANDARD || NETCOREAPP
using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed;

#if ENABLE_AZURE_TESTS
public
#else

// ReSharper disable once UnusedType.Global
internal
#endif
    class AzureServiceBusInvalidationTests2 : BaseInvalidationBrokerTests
{
    private const string _connectionString =
        "Endpoint=sb://petrservicebusstandard.servicebus.windows.net/;SharedAccessKeyName=PetrSAS;SharedAccessKey=3C+I8BExn5AMRaxXJk4kTINM0f2uXCPWKtWAdmGgpQI=;EntityPath=petrtopic";

    private const string _clientId = "95e2b555-60ed-49dd-8da7-7a5574f9b4f7";
    private const string _tenantId = "171276b2-7a8c-4c9b-bc49-57889e2e4f42";
    private const string _secret = "[?1x=eI@LabKmywBXxfNh5e67PNz3ZKm";

    public AzureServiceBusInvalidationTests2( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

    protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
    {
        return await AzureCacheInvalidator2.CreateAsync( backend, CreateOptions() );
    }

    private static AzureCacheInvalidatorOptions2 CreateOptions()
    {
        // ReSharper disable once StringLiteralTypo
        return new AzureCacheInvalidatorOptions2.NewSubscription(
            _connectionString,
            _clientId,
            _secret,
            _tenantId,
            "PetrResourceGroup",
            "PetrServiceBusStandard",
            "petrtopic",
            "bd0b2c2c-a1b5-48a0-ae37-28713cf3f27c" );
    }

    protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
    {
        return AzureCacheInvalidator2.Create( backend, CreateOptions() );
    }
}

#endif