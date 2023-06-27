// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if NETFRAMEWORK
using PostSharp.Patterns.Caching.Implementation;

namespace PostSharp.Patterns.Caching.Backends.Azure
{
    /// <summary>
    /// Options for <see cref="AzureCacheInvalidator"/>.
    /// </summary>
    public class AzureCacheInvalidatorOptions : CacheInvalidatorOptions
    {
        /// <summary>
        /// Gets or sets the connection string for the Azure Service Bus topic. The value must be a valid argument of the <c>TopicClient.CreateFromConnectionString</c> method.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
#endif