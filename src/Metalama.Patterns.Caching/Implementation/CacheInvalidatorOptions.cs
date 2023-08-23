// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Options for the <see cref="CacheInvalidator"/> class.
/// </summary>
[PublicAPI]
public class CacheInvalidatorOptions
{
    /// <summary>
    /// Gets or sets the prefix of messages sent by the <see cref="CacheInvalidator"/>.
    /// Messages received by the <see cref="CacheInvalidator.OnMessageReceived"/> method are
    /// ignored if they don't start with the proper prefix.
    /// </summary>
    public string Prefix { get; init; } = "invalidate";
}