// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETSTANDARD || NETCOREAPP
using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.TestHelpers;
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
    private static readonly string _connectionString = Secrets.Get( "CacheInvalidationNetCoreTestServiceBusConnectionString" );
    private static readonly string _clientId = Secrets.Get( "ServiceBusManagerClientId" );
    private static readonly string _tenantId = Secrets.Get( "ServiceBusManagerTenantId" );
    private static readonly string _secret = Secrets.Get( "ServiceBusManagerSecret" );
    private static readonly string _resourceGroup = Secrets.Get( "CacheInvalidationNetCoreTestServiceBusResourceGroup" );
    private static readonly string _namespaceName = Secrets.Get( "CacheInvalidationNetCoreTestServiceBusNamespaceName" );
    private static readonly string _topicName = Secrets.Get( "CacheInvalidationNetCoreTestServiceBusTopicName" );
    private static readonly string _subscriptionId = Secrets.Get( "CacheInvalidationNetCoreTestServiceBusSubscriptionId" );

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
            _resourceGroup,
            _namespaceName,
            _topicName,
            _subscriptionId );
    }

    protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
    {
        return AzureCacheInvalidator2.Create( backend, CreateOptions() );
    }
}

#endif