// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Azure;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.TestHelpers;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.ManualTest.Backends.Distributed
{
    // ReSharper disable once UnusedType.Global
    public class AzureServiceBusInvalidationTests : BaseInvalidationBrokerTests
    {
        private readonly string _connectionString = Secrets.Get( "CacheInvalidationTestServiceBusConnectionString" );

        public AzureServiceBusInvalidationTests( TestContext testContext, ITestOutputHelper testOutputHelper ) : base( testContext, testOutputHelper ) { }

        protected override async Task<CacheInvalidator> CreateInvalidationBrokerAsync( CachingBackend backend, string prefix )
        {
            return await AzureCacheInvalidator.CreateAsync( backend, new AzureCacheInvalidatorOptions( this._connectionString ) { Prefix = prefix } );
        }

        protected override CacheInvalidator CreateInvalidationBroker( CachingBackend backend, string prefix )
        {
            return AzureCacheInvalidator.Create( backend, new AzureCacheInvalidatorOptions( this._connectionString ) { Prefix = prefix } );
        }
    }
}