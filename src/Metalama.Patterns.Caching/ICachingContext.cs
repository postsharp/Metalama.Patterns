// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Dependencies;

namespace Metalama.Patterns.Caching
{
    /// <summary>
    /// Represents the context in which a method being cached is executing. 
    /// </summary>
    public interface ICachingContext 
    {
        /// <summary>
        /// Gets the parent context.
        /// </summary>
        ICachingContext Parent { get; }

        /// <summary>
        /// Gets the kind of <see cref="ICachingContext"/>.
        /// </summary>
        CachingContextKind Kind { get; }

        /// <summary>
        /// Adds a set of dependencies represented as keys to the current context.
        /// </summary>
        /// <param name="dependencies">A set of dependency keys.</param>
        void AddDependencies(IEnumerable<string> dependencies);

        /// <summary>
        /// Adds a set of dependencies represented as <see cref="ICacheDependency"/> to the current context.
        /// </summary>
        /// <param name="dependencies">A set of <see cref="ICacheDependency"/>.</param>
        void AddDependencies(IEnumerable<ICacheDependency> dependencies);

        /// <summary>
        /// Adds a dependency represented as a key to the current context.
        /// </summary>
        /// <param name="dependency">A dependency key.</param>
        void AddDependency(string dependency);

        /// <summary>
        /// Adds a dependency <see cref="object"/> the current context. Calling this method is equivalent to wrapping the <see cref="object"/>
        /// into an <see cref="ObjectDependency"/> and calling the <see cref="AddDependency(ICacheDependency)"/> overload.
        /// </summary>
        /// <param name="dependency">A dependency object.</param>
        void AddDependency(object dependency);

        /// <summary>
        /// Adds a dependency represented as an <see cref="ICacheDependency"/> to the current context.
        /// </summary>
        /// <param name="dependency">A dependency.</param>
        void AddDependency(ICacheDependency dependency);
    }
}
