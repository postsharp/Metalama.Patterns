// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Azure.Core;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends.Azure;

/// <summary>
/// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator"/> instance.
/// </summary>
[PublicAPI]
public sealed class AzureCacheInvalidatorOptions : CacheInvalidatorOptions
{
    public string TopicName { get; init; }

    public string ConnectionString { get; }

    public ServiceBusClientOptions ClientOptions { get; init; } = new();

    public TimeSpan RetryOnReceiveError { get; init; } = TimeSpan.FromSeconds( 5 );

    /// <summary>
    /// Gets the topic subscription name. Not to be confused with the Azure subscription id. If this property is not supplied,
    /// a new auto-deleted is created every time a <see cref="AzureCacheInvalidator"/> is instantiated. 
    /// </summary>
    public string? SubscriptionName { get; init; }

    public ServiceBusAdministrationClientOptions AdministrationClientOptions { get; init; } = new();

    public TimeSpan AutoDeleteOnIdle { get; init; } = TimeSpan.FromMinutes( 5 );

    public int MaxDeliveryCount { get; init; } = 10;

    public AzureCacheInvalidatorOptions( string connectionString, string? topicName = null )
    {
        this.ConnectionString = connectionString;

        if ( topicName == null )
        {
            var parsedConnectionString = ParseAzureConnectionString( connectionString );

            if ( !parsedConnectionString.TryGetValue( "EntityPath", out topicName ) )
            {
                throw new ArgumentNullException(
                    nameof(topicName),
                    "The 'topicName' parameter must be supplied because the connection string does not contain it." );
            }
        }

        this.TopicName = topicName;
    }

    private static Dictionary<string, string> ParseAzureConnectionString( string connectionString )
    {
        var keyValuePairs = connectionString.Split( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
        var result = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

        foreach ( var keyValuePair in keyValuePairs )
        {
            var parts = keyValuePair.Split( new[] { '=' }, 2 ); // We split only on the first '='

            if ( parts.Length == 2 )
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                result[key] = value;
            }
        }

        return result;
    }
}