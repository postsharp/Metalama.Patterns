// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Azure.Core;
using Azure.Messaging.ServiceBus;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends.Azure;

/// <summary>
/// Options that determine the mode of operation of an <see cref="AzureCacheInvalidator"/> instance.
/// </summary>
[PublicAPI]
public abstract partial class AzureCacheInvalidatorOptions : CacheInvalidatorOptions
{
    public string TopicName { get; init; }

    public string ConnectionString { get; }

    public ServiceBusClientOptions ClientOptions { get; init; } = new();

    public TimeSpan RetryOnReceiveError { get; init; } = TimeSpan.FromSeconds( 5 );

    protected AzureCacheInvalidatorOptions( string connectionString, string topicName )
    {
        this.ConnectionString = connectionString;
        this.TopicName = topicName;
    }
}