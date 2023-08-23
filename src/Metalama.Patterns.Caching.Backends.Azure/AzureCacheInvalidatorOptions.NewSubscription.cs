// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Azure.Messaging.ServiceBus.Administration;
using JetBrains.Annotations;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Backends.Azure;

public abstract partial class AzureCacheInvalidatorOptions
{
    /// <summary>
    /// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator"/> instance when creating a new subscription.
    /// </summary>
    [PublicAPI]
    public sealed class NewSubscription : AzureCacheInvalidatorOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheInvalidatorOptions.NewSubscription"/> class indicating that a new subscription to an Azure Service Bus topic should be created.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure Service Bus. The connection string must include the topic name.</param>
        /// <param name="topicName">Topic of an Azure Service Bus instance; invalidation messages will be passed via this topic.</param>
        public NewSubscription( [Required] string connectionString, [Required] string topicName )
            : base( connectionString, topicName ) { }

        public ServiceBusAdministrationClientOptions AdministrationClientOptions { get; init; } = new();

        public TimeSpan AutoDeleteOnIdle { get; init; }

        public int MaxDeliveryCount { get; init; }
    }
}