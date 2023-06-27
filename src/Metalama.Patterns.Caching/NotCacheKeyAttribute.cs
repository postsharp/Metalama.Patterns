// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching
{
    /// <summary>
    /// Custom attribute that, when applied to a parameter of a cached method (i.e. a method enhanced by the <see cref="CacheAttribute"/> aspect),
    /// excludes this parameter from being a part of the cache key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class NotCacheKeyAttribute : Attribute
    {
        
    }
}
