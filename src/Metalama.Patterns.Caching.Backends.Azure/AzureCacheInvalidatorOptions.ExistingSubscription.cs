// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Backends.Azure;

public abstract partial class AzureCacheInvalidatorOptions
{
    /// <summary>
    /// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator"/> instance when using an existing subscription.
    /// </summary>
    [PublicAPI]
    public sealed class ExistingSubscription : AzureCacheInvalidatorOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheInvalidatorOptions.ExistingSubscription"/> class indicating that an existing subscription is used. The subscription must have been created
        /// by you previously.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure Service Bus. The connection string must include the topic name.</param>
        /// <param name="subscriptionName">Name of a subscription to the topic identified in the connection string. The subscription must already exist.</param>
        public ExistingSubscription( [Required] string connectionString, [Required] string topicName, [Required] string subscriptionName )
            : base( connectionString, topicName )
        {
            this.SubscriptionName = subscriptionName;
        }

        /// <summary>
        /// Gets the subscription name. See the constructor for details.
        /// </summary>
        public string SubscriptionName { get; }
    }
}