// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace Metalama.Patterns.Caching.Implementation
{
    /// <summary>
    /// Options for the <see cref="CacheInvalidator"/> class.
    /// </summary>
    public class CacheInvalidatorOptions
    {
        /// <summary>
        /// Gets or sets the prefix of messages sent by the <see cref="CacheInvalidator"/>.
        /// Messages received by the <see cref="CacheInvalidator.OnMessageReceived"/> method are
        /// ignored if they don't start with the proper prefix.
        /// </summary>
        public string Prefix { get; set; } = "invalidate";
    }
}
