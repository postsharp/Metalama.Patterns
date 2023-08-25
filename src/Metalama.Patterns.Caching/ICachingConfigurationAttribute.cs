// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

[RunTimeOrCompileTime]
internal interface ICachingConfigurationAttribute
{
    /// <summary>
    /// Gets or sets the name of the <see cref="CachingProfile"/> that contains the configuration of the cached methods.
    /// </summary>
    string ProfileName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    bool AutoReload { get; set; }

    /// <summary>
    /// Gets or sets the total duration, in minutes, during which the result of the cached method  is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    double AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the duration, in minutes, during which the result of the cached method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    double SlidingExpiration { get; set; }

    /// <summary>
    /// Gets or sets the priority of the cached method.
    /// </summary>
    CacheItemPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
    /// which means that by default the <c>this</c> instance is a part of the cache key.
    /// </summary>
    bool IgnoreThisParameter { get; set; }

    bool UseDependencyInjection { get; set; }
}