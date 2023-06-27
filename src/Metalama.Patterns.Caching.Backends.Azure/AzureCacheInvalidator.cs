// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if NETFRAMEWORK

using Flashtrace;
using Metalama.Patterns.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using PostSharp.Patterns.Caching.Implementation;
using System.Diagnostics.CodeAnalysis;

namespace PostSharp.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// An implementation of <see cref="CacheInvalidator"/> based on Microsoft Azure Service Bus, using the older API, <c>WindowsAzure.ServiceBus</c>,
    /// meant for .NET Framework.
    /// </summary>
    public class AzureCacheInvalidator : CacheInvalidator
    {
        private static readonly LogSource logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( typeof(AzureCacheInvalidator) );
        private readonly string subscriptionName = Guid.NewGuid().ToString();
        private readonly NamespaceManager serviceBusNamespaceManager;
        private readonly TopicClient topic;
        private SubscriptionClient subscription;
        private volatile bool isStopped;
        private Task processMessageTask;
        private readonly AzureCacheInvalidatorOptions options;

        private AzureCacheInvalidator( CachingBackend underlyingBackend, AzureCacheInvalidatorOptions options ) : base(underlyingBackend, options)
        {
            this.options = options;
            this.topic = TopicClient.CreateFromConnectionString(options.ConnectionString );

            ServiceBusConnectionStringBuilder connectionStringBuilder = new ServiceBusConnectionStringBuilder(options.ConnectionString);
            connectionStringBuilder.EntityPath = null;

            this.serviceBusNamespaceManager = NamespaceManager.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        /// <summary>
        /// Creates a new <see cref="AzureCacheInvalidator"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static AzureCacheInvalidator Create( [Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions options )
        {
            AzureCacheInvalidator invalidator = new AzureCacheInvalidator( backend, options );
            invalidator.Init();
            return invalidator;
        }

        private void Init()
        {
            this.serviceBusNamespaceManager.CreateSubscription(this.CreateSubscriptionDescription());

            this.InitCommon();
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="AzureCacheInvalidator"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static Task<AzureCacheInvalidator> CreateAsync([Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions options )
        {
            AzureCacheInvalidator invalidator = new AzureCacheInvalidator( backend, options );
            return invalidator.InitAsync();
        }



        private async Task<AzureCacheInvalidator> InitAsync()
        {
            await this.serviceBusNamespaceManager.CreateSubscriptionAsync(this.CreateSubscriptionDescription());

            this.InitCommon();

            return this;
        }

        private SubscriptionDescription CreateSubscriptionDescription()
        {
            return new SubscriptionDescription(this.topic.Path, this.subscriptionName)
                   {
                       AutoDeleteOnIdle = TimeSpan.FromMinutes(5),

                   };
        }

        private void InitCommon()
        {
            this.subscription = SubscriptionClient.CreateFromConnectionString( this.options.ConnectionString, this.topic.Path, this.subscriptionName, ReceiveMode.ReceiveAndDelete );

            this.processMessageTask = Task.Run(this.ProcessMessages);
        }

        private async Task ProcessMessages()
        {
            while ( !this.isStopped )
            {
                try
                {
                    using ( BrokeredMessage message = await this.subscription.ReceiveAsync() )
                    {

                        if ( message == null )
                        {
                            continue;
                        }

                        string value = message.GetBody<string>();

                        this.OnMessageReceived( value );
                        await message.CompleteAsync();
                    }


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
            }
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Reliability", "CA2000")] // BrokeredMessage should not be disposed in this method.
        protected override Task SendMessageAsync( string message, CancellationToken cancellationToken )
        {
            BrokeredMessage brokeredMessage = new BrokeredMessage( message );
            return this.topic.SendAsync(brokeredMessage);
        }


        /// <inheritdoc />
        protected override void DisposeCore(bool disposing)
        {
            this.isStopped = true;
            this.subscription.Close();
            this.topic.Close();
            this.serviceBusNamespaceManager.DeleteSubscription( this.topic.Path, this.subscriptionName );
            this.processMessageTask.Wait();

            if ( disposing )
            {
                GC.SuppressFinalize( this );
            }
        }

        /// <inheritdoc />
        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            this.isStopped = true;
            await this.subscription.CloseAsync();
            await this.topic.CloseAsync();
            await this.serviceBusNamespaceManager.DeleteSubscriptionAsync( this.topic.Path, this.subscriptionName );
            await this.processMessageTask;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063")]
        ~AzureCacheInvalidator()
        {
           this.DisposeCore( false );
        }


    }
}
#endif