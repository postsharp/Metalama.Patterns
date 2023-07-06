// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a cached method determined at compile time.
/// </summary>
[PublicAPI]
public interface ICompileTimeCacheItemConfiguration
{
    /// <summary>
    /// Gets the total duration during which the result of the cached methods  is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    TimeSpan? AbsoluteExpiration { get; }

    /// <summary>
    /// Gets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    bool? AutoReload { get; }

    /// <summary>
    /// Gets the priority of the cached method.
    /// </summary>
    CacheItemPriority? Priority { get; }

    /// <summary>
    /// Gets the name of the <see cref="CachingProfile"/>  that contains the configuration of the cached methods.
    /// </summary>
    string? ProfileName { get; }

    /// <summary>
    /// Gets the duration during which the result of the cached methods is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    TimeSpan? SlidingExpiration { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
    /// which means that by default the <c>this</c> instance is a part of the cache key.
    /// </summary>
    bool? IgnoreThisParameter { get; }
}
