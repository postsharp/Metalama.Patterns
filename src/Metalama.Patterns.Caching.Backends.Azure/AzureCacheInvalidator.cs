// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Flashtrace.Messages;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using System.Text;

namespace Metalama.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// An implementation of <see cref="CacheInvalidator"/> based on Microsoft Azure Service Bus, using the <c>Microsoft.Azure.ServiceBus</c> API
    /// meant for .NET Standard.
    /// </summary>
    [PublicAPI]
    internal sealed class AzureCacheInvalidator : CacheInvalidator
    {
        private const string _subject = "Metalama.Patterns.Caching.Backends.Azure.Invalidation";

        private readonly CancellationTokenSource _receiverCancellation = new();

        private string _subscriptionName = null!;
        private ServiceBusReceiver? _receiver;
        private ServiceBusSender? _sender;
        private int _backgroundTaskExceptions;

        public AzureCacheInvalidator( CachingBackend underlyingBackend, AzureCacheInvalidatorConfiguration configuration ) : base(
            underlyingBackend,
            configuration ) { }

        protected override int BackgroundTaskExceptions => base.BackgroundTaskExceptions + this._backgroundTaskExceptions;

        /// <summary>
        /// Asynchronously creates a new <see cref="AzureCacheInvalidator"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator"/>.</param>
        /// <param name="configuration">Options.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator"/>.</returns>
        public static async Task<AzureCacheInvalidator> CreateAsync(
            CachingBackend backend,
            AzureCacheInvalidatorConfiguration configuration,
            CancellationToken cancellationToken = default )
        {
            var invalidator = new AzureCacheInvalidator( backend, configuration );
            await invalidator.InitAsync( configuration, cancellationToken );

            return invalidator;
        }

        public event EventHandler<AzureCacheInvalidatorExceptionEventArgs>? ReceiverException;

        private async Task InitAsync( AzureCacheInvalidatorConfiguration configuration, CancellationToken cancellationToken )
        {
            if ( configuration.SubscriptionName == null )
            {
                this._subscriptionName = await this.CreateSubscriptionAsync( configuration, cancellationToken );
            }
            else
            {
                this._subscriptionName = configuration.SubscriptionName;
            }

            var client = new ServiceBusClient( configuration.ConnectionString, configuration.ClientOptions );

            this._receiver = client.CreateReceiver( configuration.TopicName, this._subscriptionName );
            this._sender = client.CreateSender( configuration.TopicName );

            // Start a background task to process received messages.
            _ = Task.Run( this.ReceiveMessagesAsync, this._receiverCancellation.Token );
        }

        private async Task ReceiveMessagesAsync()
        {
            while ( true )
            {
                try
                {
                    await foreach ( var message in this._receiver!.ReceiveMessagesAsync( this._receiverCancellation.Token ) )
                    {
                        this._receiverCancellation.Token.ThrowIfCancellationRequested();

                        try
                        {
                            this.OnMessageReceived( message.Body.ToString() );
                        }
                        catch ( OperationCanceledException ) { }
                        catch ( Exception e )
                        {
                            this.Source.Error.Write( FormattedMessageBuilder.Formatted( "Exception while processing Azure Service Bus message." ), e );
                            this._backgroundTaskExceptions++;
                        }
                    }
                }
                catch ( Exception e )
                {
                    this.ReceiverException?.Invoke( this, new AzureCacheInvalidatorExceptionEventArgs( e ) );
                    await Task.Delay( ((AzureCacheInvalidatorConfiguration) this.Configuration).RetryOnReceiveError );
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        /// <inheritdoc />
        protected override Task SendMessageAsync( string message, CancellationToken cancellationToken )
        {
            var azureMessage = new ServiceBusMessage( Encoding.UTF8.GetBytes( message ) )
            {
                ContentType = "text/plain", Subject = _subject, TimeToLive = TimeSpan.FromMinutes( 5 )
            };

            return this._sender!.SendMessageAsync( azureMessage, cancellationToken );
        }

        /// <inheritdoc />
        protected override void DisposeCore( bool disposing )
        {
            this._receiverCancellation.Cancel();

            if ( this._receiver is { IsClosed: false } || this._sender is { IsClosed: false } )
            {
                Task.Run(
                        async () =>
                        {
                            if ( this._receiver is { IsClosed: false } )
                            {
                                await this._receiver.CloseAsync();
                            }

                            if ( this._sender is { IsClosed: false } )
                            {
                                await this._sender.CloseAsync();
                            }
                        } )
                    .Wait();
            }

            if ( disposing )
            {
                GC.SuppressFinalize( this );
            }
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore( CancellationToken cancellationToken )
        {
            this._receiverCancellation.Cancel();

            if ( this._receiver != null )
            {
                await this._receiver.CloseAsync( cancellationToken );
            }

            if ( this._sender != null )
            {
                await this._sender.CloseAsync( cancellationToken );
            }

            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AzureCacheInvalidator"/> class.
        /// </summary>
        ~AzureCacheInvalidator()
        {
            this.DisposeCore( false );
        }

        private async Task<string> CreateSubscriptionAsync( AzureCacheInvalidatorConfiguration configuration, CancellationToken cancellationToken )
        {
            try
            {
                var subscriptionId = Guid.NewGuid().ToString();

                var client = new ServiceBusAdministrationClient( configuration.ConnectionString, configuration.AdministrationClientOptions );

                var subscriptionOptions =
                    new CreateSubscriptionOptions( configuration.TopicName, subscriptionId )
                    {
                        MaxDeliveryCount = configuration.MaxDeliveryCount, AutoDeleteOnIdle = configuration.AutoDeleteOnIdle
                    };

                await client.CreateSubscriptionAsync( subscriptionOptions, cancellationToken );

                return subscriptionId;
            }
            catch ( Exception e )
            {
                this.Source.Error.Write( FormattedMessageBuilder.Formatted( "Exception while processing Azure Service Bus subscription." ), e );

                throw;
            }
        }
    }
}