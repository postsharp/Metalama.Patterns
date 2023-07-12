// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETSTANDARD || NETCOREAPP
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Backends.Azure;

/// <summary>
/// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator2"/> instance.
/// </summary>
[PublicAPI]
public abstract class AzureCacheInvalidatorOptions2 : CacheInvalidatorOptions
{
    /// <summary>
    /// Gets the connection string for the Azure Service Bus topic. The connection string must include the topic name.
    /// </summary>
    public string? ConnectionString { get; }

    /// <summary>
    /// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator2"/> instance when using an existing subscription.
    /// </summary>
    [PublicAPI]
    public sealed class ExistingSubscription : AzureCacheInvalidatorOptions2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheInvalidatorOptions2.ExistingSubscription"/> class indicating that an existing subscription is used. The subscription must have been created
        /// by you previously.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure Service Bus. The connection string must include the topic name.</param>
        /// <param name="subscriptionName">Name of a subscription to the topic identified in the connection string. The subscription must already exist.</param>
        public ExistingSubscription( string? connectionString, [Required] string subscriptionName )
            : base( connectionString )
        {
            this.SubscriptionName = subscriptionName;
        }

        /// <summary>
        /// Gets the subscription name. See the constructor for details.
        /// </summary>
        public string SubscriptionName { get; }
    }

    /// <summary>
    /// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator2"/> instance when creating a new subscription.
    /// </summary>
    [PublicAPI]
    public sealed class NewSubscription : AzureCacheInvalidatorOptions2
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheInvalidatorOptions2.NewSubscription"/> class indicating that a new subscription to an Azure Service Bus topic should be created.
        /// </summary>
        /// <param name="connectionString">Connection string to Azure Service Bus. The connection string must include the topic name.</param>
        /// <param name="clientId">Client ID that allows for management of subscriptions of Azure Service Bus.</param>
        /// <param name="clientSecret">Client secret that allows for management of subscriptions of Azure Service Bus.</param>
        /// <param name="tenantId">Tenant ID that allows for management of subscriptions of Azure Service Bus.</param>
        /// <param name="resourceGroupName">Name of the resource group of your Azure Service Bus instance.</param>
        /// <param name="namespaceName">Namespace of your Azure Service Bus instance.</param>
        /// <param name="topicName">Topic of an Azure Service Bus instance; invalidation messages will be passed via this topic.</param>
        /// <param name="azureSubscriptionId">Unique GUID for your Azure subscription. This is account-wide.</param>
        public NewSubscription(
            string connectionString,
            [Required] string clientId,
            [Required] string clientSecret,
            [Required] string tenantId,
            string resourceGroupName,
            string namespaceName,
            string topicName,
            string azureSubscriptionId )
            : base( connectionString )
        {
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;
            this.TenantId = tenantId;
            this.ResourceGroupName = resourceGroupName;
            this.NamespaceName = namespaceName;
            this.TopicName = topicName;
            this.AzureSubscriptionId = azureSubscriptionId;
        }

        /// <summary>
        /// Gets the client ID. See the constructor for details.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the client secret. See the constructor for details.
        /// </summary>
        public string ClientSecret { get; }

        /// <summary>
        /// Gets the tenant ID. See the constructor for details.
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Gets the resource group name. See the constructor for details.
        /// </summary>
        public string? ResourceGroupName { get; }

        /// <summary>
        /// Gets the namespace name. See the constructor for details.
        /// </summary>
        public string? NamespaceName { get; }

        /// <summary>
        /// Gets the topic name. See the constructor for details.
        /// </summary>
        public string? TopicName { get; }

        /// <summary>
        /// Gets the azure subscription ID. See the constructor for details.
        /// </summary>
        public string? AzureSubscriptionId { get; }

        /// <summary>
        /// Gets or sets the URL that hosts the Azure login service which authenticates Azure tokens. This is normally
        /// <c>https://login.microsoftonline.com/</c> which is also the default value. You do not need to change this unless you use
        /// an on-premises Azure deployment. If you change it, your URL must end with a slash. The <see cref="TenantId"/> is appended to it
        /// to get the full address.
        /// </summary>
        [Required]
        public string AzureLoginAuthority { get; set; } = "https://login.microsoftonline.com/";
    }

    protected AzureCacheInvalidatorOptions2( string? connectionString )
    {
        this.ConnectionString = connectionString;
    }
}
#endif