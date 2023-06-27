// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// List of features that can be implemented or not by a <see cref="CachingBackend"/>.
/// </summary>
public class CachingBackendFeatures
{
    /// <summary>
    /// Determines whether the <see cref="CachingBackend.Clear"/> method is supported.
    /// </summary>
    public virtual bool Clear => true;

    /// <summary>
    /// Determines whether the <see cref="CachingBackend.ItemRemoved"/> and <see cref="CachingBackend.DependencyInvalidated"/> events are raised.
    /// </summary>
    public virtual bool Events => true;

    /// <summary>
    /// Determines whether methods that modify the cache content run to completion before the control is given back to the calling method.
    /// If <c>false</c>, methods may run in the background, and the calling thread may not have a consistent view of the cache.
    /// </summary>
    public virtual bool Blocking => true;

    /// <summary>
    /// Determines whether dependencies are supported.
    /// </summary>
    public virtual bool Dependencies => true;

    /// <summary>
    /// Determines whether the <see cref="CachingBackend.ContainsDependency(string)"/> method is supported.
    /// </summary>
    public virtual bool ContainsDependency => true;
}