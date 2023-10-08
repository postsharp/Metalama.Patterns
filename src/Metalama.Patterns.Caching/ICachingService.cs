// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Front-end interface used by the caching aspects.
/// </summary>
public interface ICachingService
{
    FlashtraceSource Logger { get; }

    ICacheKeyBuilder KeyBuilder { get; }

    ImmutableArray<CachingBackend> AllBackends { get; }

    TResult? GetFromCacheOrExecute<TResult>(
        CachedMethodMetadata metadata,
        object? instance,
        object?[] args,
        Func<object?, object?[], object?> func,
        CacheItemConfiguration? configuration = null,
        CancellationToken cancellationToken = default );

    Task<TTaskResultType?> GetFromCacheOrExecuteTaskAsync<TTaskResultType>(
        CachedMethodMetadata metadata,
        object? instance,
        object?[] args,
        Func<object?, object?[], CancellationToken, Task<object?>> func,
        CacheItemConfiguration? configuration = null,
        CancellationToken cancellationToken = default );

    ValueTask<TTaskResultType?> GetFromCacheOrExecuteValueTaskAsync<TTaskResultType>(
        CachedMethodMetadata metadata,
        object? instance,
        object?[] args,
        Func<object?, object?[], CancellationToken, ValueTask<object?>> func,
        CacheItemConfiguration? configuration = null,
        CancellationToken cancellationToken = default );
}