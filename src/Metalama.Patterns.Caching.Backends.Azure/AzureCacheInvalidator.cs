// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
using Flashtrace;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Metalama.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// An implementation of <see cref="CacheInvalidator"/> based on Microsoft Azure Service Bus, using the older API, <c>WindowsAzure.ServiceBus</c>,
    /// meant for .NET Framework.
    /// </summary>
    [PublicAPI]
    public class AzureCacheInvalidator : CacheInvalidator
    {
        private static readonly LogSource _logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( typeof(AzureCacheInvalidator) );
        private readonly string _subscriptionName = Guid.NewGuid().ToString();
        private readonly NamespaceManager _serviceBusNamespaceManager;
        private readonly TopicClient _topic;
        private readonly AzureCacheInvalidatorOptions _options;
        private int _backgroundTaskExceptions;

        // According to Create method logic, consumers can't obtain instances where _subscription and _processMessageTask are null. 
        private SubscriptionClient _subscription = null!;
        private Task _processMessageTask = null!;
        private volatile bool _isStopped;

        protected override int BackgroundTaskExceptions => base.BackgroundTaskExceptions + this._backgroundTaskExceptions;

        private AzureCacheInvalidator( CachingBackend underlyingBackend, AzureCacheInvalidatorOptions options ) : base( underlyingBackend, options )
        {
            this._options = options;
            this._topic = TopicClient.CreateFromConnectionString( options.ConnectionString );

            var connectionStringBuilder = new ServiceBusConnectionStringBuilder( options.ConnectionString ) { EntityPath = null };

            this._serviceBusNamespaceManager = NamespaceManager.CreateFromConnectionString( connectionStringBuilder.ToString() );
        }

        /// <summary>
        /// Creates a new <see cref="AzureCacheInvalidator"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator"/>.</returns>
        public static AzureCacheInvalidator Create( [Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions options )
        {
            var invalidator = new AzureCacheInvalidator( backend, options );
            invalidator.Init();

            return invalidator;
        }

        private void Init()
        {
            this._serviceBusNamespaceManager.CreateSubscription( this.CreateSubscriptionDescription() );

            this.InitCommon();
        }

        /// <summary>
        /// Asynchronously creates a new <see cref="AzureCacheInvalidator"/>.
        /// </summary>
        /// <param name="backend">The local (in-memory, typically) cache being invalidated by the new <see cref="AzureCacheInvalidator"/>.</param>
        /// <param name="options">Options.</param>
        /// <returns>A new <see cref="AzureCacheInvalidator"/>.</returns>
        public static Task<AzureCacheInvalidator> CreateAsync( [Required] CachingBackend backend, [Required] AzureCacheInvalidatorOptions options )
        {
            var invalidator = new AzureCacheInvalidator( backend, options );

            return invalidator.InitAsync();
        }

        private async Task<AzureCacheInvalidator> InitAsync()
        {
            await this._serviceBusNamespaceManager.CreateSubscriptionAsync( this.CreateSubscriptionDescription() );

            this.InitCommon();

            return this;
        }

        private SubscriptionDescription CreateSubscriptionDescription()
        {
            return new SubscriptionDescription( this._topic.Path, this._subscriptionName ) { AutoDeleteOnIdle = TimeSpan.FromMinutes( 5 ) };
        }

        private void InitCommon()
        {
            this._subscription =
                SubscriptionClient.CreateFromConnectionString(
                    this._options.ConnectionString,
                    this._topic.Path,
                    this._subscriptionName,
                    ReceiveMode.ReceiveAndDelete );

            this._processMessageTask = Task.Run( this.ProcessMessages );
        }

        private async Task ProcessMessages()
        {
            while ( !this._isStopped )
            {
                try
                {
                    using ( var message = await this._subscription.ReceiveAsync() )
                    {
                        if ( message == null )
                        {
                            continue;
                        }

                        var value = message.GetBody<string>();

                        this.OnMessageReceived( value );
                    }
                }
                catch ( OperationCanceledException ) { }
#pragma warning disable CA1031 // Do not catch general exception types
                catch ( Exception e )
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    _logger.Error.Write( FormattedMessageBuilder.Formatted( "Exception while processing Azure Service Bus message." ), e );
                    this._backgroundTaskExceptions++;
                }
            }
        }

        /// <inheritdoc />
        protected override Task SendMessageAsync( string message, CancellationToken cancellationToken )
        {
            var brokeredMessage = new BrokeredMessage( message );

            return this._topic.SendAsync( brokeredMessage );
        }

        /// <inheritdoc />
        protected override void DisposeCore( bool disposing )
        {
            this._isStopped = true;
            this._subscription.Close();
            this._topic.Close();
            this._serviceBusNamespaceManager.DeleteSubscription( this._topic.Path, this._subscriptionName );
            this._processMessageTask.Wait();

            if ( disposing )
            {
                GC.SuppressFinalize( this );
            }
        }

        /// <inheritdoc />
        protected override async Task DisposeAsyncCore( CancellationToken cancellationToken )
        {
            this._isStopped = true;
            await this._subscription.CloseAsync();
            await this._topic.CloseAsync();
            await this._serviceBusNamespaceManager.DeleteSubscriptionAsync( this._topic.Path, this._subscriptionName );
            await this._processMessageTask;
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="AzureCacheInvalidator"/> class.
        /// </summary>
        ~AzureCacheInvalidator()
        {
            this.DisposeCore( false );
        }
    }
}
#endif