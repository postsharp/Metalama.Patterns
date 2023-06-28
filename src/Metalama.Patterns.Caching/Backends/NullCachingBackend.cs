// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.Backends;

/// <summary>
/// An implementation of <see cref="CachingBackend"/> that does not cache at all.
/// </summary>
[PublicAPI]
public class NullCachingBackend : CachingBackend
{
    /// <inheritdoc />
    protected override void SetItemCore( string key, CacheItem item ) { }

    /// <inheritdoc />
    protected override bool ContainsItemCore( string key ) => false;

    /// <inheritdoc />
    protected override CacheValue? GetItemCore( string key, bool includeDependencies ) => null;

    /// <inheritdoc />
    protected override void InvalidateDependencyCore( string key ) { }

    /// <inheritdoc />
    protected override bool ContainsDependencyCore( string key ) => false;

    /// <inheritdoc />
    protected override void ClearCore() { }

    /// <inheritdoc />
    protected override void RemoveItemCore( string key ) { }
}