// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a parameter of a method
/// being cached.
/// </summary>
internal sealed class CachedParameterInfo
{
    /// <summary>
    /// Gets a value indicating whether the parameter should be excluded
    /// from the cache key. When the value of this property is <c>false</c>,
    /// the parameter should be included in the cache key.
    /// </summary>
    public bool IsIgnored { get; }

    internal CachedParameterInfo( bool isIgnored )
    {
        this.IsIgnored = isIgnored;
    }
}