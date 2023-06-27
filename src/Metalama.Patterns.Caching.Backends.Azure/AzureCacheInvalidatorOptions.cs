// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// Options for <see cref="AzureCacheInvalidator"/>.
    /// </summary>
    [PublicAPI]
    public sealed class AzureCacheInvalidatorOptions : CacheInvalidatorOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCacheInvalidatorOptions"/> class with the specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public AzureCacheInvalidatorOptions( [Required] string connectionString )
        {
            this.ConnectionString = connectionString;
        }
        
        /// <summary>
        /// Gets the connection string for the Azure Service Bus topic. The value must be a valid argument of the <c>TopicClient.CreateFromConnectionString</c> method.
        /// </summary>
        public string ConnectionString { get; }
    }
}
#endif