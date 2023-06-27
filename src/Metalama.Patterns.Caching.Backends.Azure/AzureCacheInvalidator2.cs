// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial source-available license. Please see the LICENSE.md file in the repository root for details.

#if NETSTANDARD || NET5_0 || NET6_0 || NET7_0
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using PostSharp.Aspects.Advices;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Diagnostics;

namespace PostSharp.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// An implementation of <see cref="CacheInvalidator"/> based on Microsoft Azure Service Bus, using the <c>Microsoft.Azure.ServiceBus</c> API
    /// meant for .NET Standard.
    /// </summary>
    public class AzureCacheInvalidator2 : CacheInvalidator
    {
        private static readonly LogSource logger = LogSourceFactory.ForRole3( LoggingRoles.Caching )
                                                                   .GetLogSource( typeof(AzureCacheInvalidator2) );

        private string subscriptionName;
        private readonly TopicClient topic;
        private SubscriptionClient subscription;
        private readonly ServiceBusConnectionStringBuilder connectionStringBuilder;

        private AzureCacheInvalidator2( CachingBackend underlyingBackend, AzureCacheInvalidatorOptions2 options ) : base(underlyingBackend, options)
        {
            this.connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.ConnectionString);
            this.topic = new TopicClient( this.connectionStringBuilder );
        }


        /// <summary>
        /// Asynchronously creates a new <see cref="AzureCacheInvalidator2"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator2"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator2"/>.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static async Task<AzureCacheInvalidator2> CreateAsync([Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions2 options )
        {
            AzureCacheInvalidator2 invalidator = new AzureCacheInvalidator2( backend, options );
            await invalidator.Init( options );
            return invalidator;
        }
        /// <summary>
        /// Creates a new <see cref="AzureCacheInvalidator2"/> and blocks until a subscription is established.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator2"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator2"/>.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static AzureCacheInvalidator2 Create( [Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions2 options )
        {
            AzureCacheInvalidator2 invalidator = new AzureCacheInvalidator2( backend, options );
            invalidator.Init(options).Wait();
            return invalidator;
        }

        private async Task Init( AzureCacheInvalidatorOptions2 options )
        {
            if ( options.SubscriptionName == null )
            {
                this.subscriptionName = await CreateSubscription( options );
            }
            else
            {
                this.subscriptionName = options.SubscriptionName;
            }
            this.subscription = new SubscriptionClient( this.connectionStringBuilder, this.subscriptionName, ReceiveMode.ReceiveAndDelete );
            this.subscription.RegisterMessageHandler( this.ProcessSingleMessage, this.ProcessSingleException );
        }

        private Task ProcessSingleException( ExceptionReceivedEventArgs arg )
        {
            logger.Error.Write( FormattedMessageBuilder.Formatted(  "Exception coming from Azure Service bus." ), arg.Exception );
            return Task.CompletedTask;
        }

        private Task ProcessSingleMessage( Message arg1, CancellationToken arg2 )
        {
            try
            {
                string value = Encoding.UTF8.GetString( arg1.Body );
                this.OnMessageReceived( value );
            }
            catch ( OperationCanceledException )
            {
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch ( Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                logger.Error.Write( FormattedMessageBuilder.Formatted(  "Exception while processing Azure Service Bus message." ), e );
            }
            return Task.CompletedTask;
        }

     
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Reliability", "CA2000")] // BrokeredMessage should not be disposed in this method.
        protected override Task SendMessageAsync( string message, CancellationToken cancellationToken )
        {
            Message azureMessage = new Message(Encoding.UTF8.GetBytes(message))
                                   {
                                       ContentType = "text/plain",
                                       Label = "InvalidationMessage",
                                       TimeToLive = TimeSpan.FromMinutes(5)
                                   };
            return this.topic.SendAsync( azureMessage );
        }


        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            this.subscription.CloseAsync();
            this.topic.CloseAsync();

            if ( disposing )
            {
                GC.SuppressFinalize( this );
            }
        }

        /// <inheritdoc />
        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            await this.subscription.CloseAsync();
            await this.topic.CloseAsync();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063")]
        ~AzureCacheInvalidator2()
        {
           this.DisposeCore( false );
        }

		private static async Task<string> CreateSubscription( AzureCacheInvalidatorOptions2 options )
		{
			try
            {
                string subscriptionId = Guid.NewGuid().ToString();

				string token = await GetToken(options.TenantId, options.ClientId, options.ClientSecret, options.AzureLoginAuthority);

				TokenCredentials credentials = new TokenCredentials(token);
                using ( ServiceBusManagementClient sbClient = new ServiceBusManagementClient( credentials ) { SubscriptionId = options.AzureSubscriptionId } )
                {
                    SBSubscription subscriptionParams = new SBSubscription
                                                        {
                                                            MaxDeliveryCount = 10,
                                                            AutoDeleteOnIdle = TimeSpan.FromMinutes( 6 )
                                                        };

                    await sbClient.Subscriptions.CreateOrUpdateAsync(options.ResourceGroupName, options.NamespaceName, options.TopicName, subscriptionId, subscriptionParams);
                    return subscriptionId;
                }
            }
			catch (Exception e)
			{
                logger.Error.Write( FormattedMessageBuilder.Formatted(  "Exception while processing Azure Service Bus subscription." ), e );
                throw;
            }
		}
        private static async Task<string> GetToken([Required] string tenantId, [Required] string clientId, [Required] string clientSecret,
                                                   [Required] string loginAuthority)
        {
            try
            {
                // Check to see if the token has expired before requesting one.
                // We will go ahead and request a new one if we are within 2 minutes of the token expiring.
               
                AuthenticationContext context = new AuthenticationContext($"{loginAuthority}{tenantId}");

                AuthenticationResult result = await context.AcquireTokenAsync(
                    "https://management.core.windows.net/",
                    new ClientCredential(clientId, clientSecret)
                );

                // If the token isn't a valid string, throw an error.
                if (string.IsNullOrEmpty(result.AccessToken))
                {
                    throw new Exception("Token result is empty!");
                }

                return result.AccessToken;
            }
            catch (Exception e)
            {
                logger.Error.Write( FormattedMessageBuilder.Formatted(  "Could not create an Azure Service Bus token." ), e );
                throw;
            }
        }

    }
}
#endif