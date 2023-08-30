// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching;

/// <summary>
/// The entry point to configure <c>Metalama.Patterns.Caching</c> at run-time.
/// </summary>
[PublicAPI]
public static class CachingServices
{
    public static CachingService Default { get; set; } = new();

    /// <summary>
    /// Gets the current caching context, so dependencies can be added.
    /// </summary>
    public static ICachingContext CurrentContext => CachingContext.Current;

    /// <summary>
    /// Temporarily suspends propagation of dependencies from subsequently called methods to the caller method.
    /// </summary>
    /// <returns><see cref="IDisposable"/> representation of the suspension. Disposing this object resumes the normal dependency propagation.</returns>
    /// <remarks>
    /// <para>
    /// By default, calling a cached method while another <see cref="CachingContext"/> is active automatically adds the former as a dependency of the later. 
    /// Since the <see cref="CurrentContext"/> is stored in an <see cref="System.Threading.AsyncLocal{T}"/> variable, it may be inadvertently used after the method call associated with it
    /// had already ended. This can happen, for example, when method calls <see cref="System.Threading.Tasks.Task.Run(Action)"/> and does not depend on the resulting <see cref="System.Threading.Tasks.Task"/>.
    /// </para>
    /// <para>
    /// This context leak does not break correctness but may lead to unnecessary dependency invalidations. Therefore it is recommended to use this method when calling asynchronous code
    /// in the context of cached methods and not being dependent on its result.
    /// </para>
    /// </remarks>
    public static IDisposable SuspendDependencyPropagation()
    {
        return CachingContext.OpenSuspendedCacheContext();
    }
}