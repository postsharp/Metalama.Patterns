// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Interface used by the caching aspects.
/// </summary>
public interface ICachingService
{
    TResult? GetFromCacheOrExecute<TResult>(
        CachedMethodMetadata metadata,
        Func<object?, object?[], object?> func,
        object? instance,
        object?[] args,
        CancellationToken cancellationToken = default );

    Task<TTaskResultType?> GetFromCacheOrExecuteTaskAsync<TTaskResultType>(
        CachedMethodMetadata metadata,
        Func<object?, object?[], CancellationToken, Task<object?>> func,
        object? instance,
        object?[] args,
        CancellationToken cancellationToken = default );

    ValueTask<TTaskResultType?> GetFromCacheOrExecuteValueTaskAsync<TTaskResultType>(
        CachedMethodMetadata metadata,
        Func<object?, object?[], CancellationToken, ValueTask<object?>> func,
        object? instance,
        object?[] args,
        CancellationToken cancellationToken = default );

    void Invalidate( MethodInfo method, object? instance, object?[] args );

    public ValueTask InvalidateAsync( MethodInfo method, object? instance, object[] args, CancellationToken cancellationToken = default );

    void AddDependency( string key );

    void AddDependencies( IEnumerable<string> keys );

    /// <summary>
    /// Temporarily suspends propagation of dependencies from subsequently called methods to the caller method.
    /// </summary>
    /// <returns><see cref="IDisposable"/> representation of the suspension. Disposing this object resumes the normal dependency propagation.</returns>
    /// <remarks>
    /// <para>
    /// By default, calling a cached method while another caching is active automatically adds the former as a dependency of the later. 
    /// Since the current context is stored in an <see cref="System.Threading.AsyncLocal{T}"/> variable, it may be inadvertently used after the method call associated with it
    /// had already ended. This can happen, for example, when method calls <see cref="System.Threading.Tasks.Task.Run(Action)"/> and does not depend on the resulting <see cref="System.Threading.Tasks.Task"/>.
    /// </para>
    /// <para>
    /// This context leak does not break correctness but may lead to unnecessary dependency invalidations. Therefore it is recommended to use this method when calling asynchronous code
    /// in the context of cached methods and not being dependent on its result.
    /// </para>
    /// </remarks>
    IDisposable SuspendDependencyPropagation();

    string GetDependencyKey( object o );
}