// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Return value of the <see cref="CachingBackend.GetItem(string, bool)"/> method, i.e. represents an item retrieved from the cache (items being stored in the cache are represented by the <see cref="CacheItem"/> class).
/// </summary>
[PublicAPI]
public class CacheValue
{
    /// <summary>
    /// Gets the cached value.
    /// </summary>
    public object? Value { get; }

    // We need to store dependencies to be able to automatically add dependencies to the calling context
    // when we're accessing a cached method calling another cached method.

    /// <summary>
    /// Gets the list of dependencies of the cache. 
    /// </summary>
    public IImmutableList<string>? Dependencies { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheValue"/> class.
    /// </summary>
    /// <param name="value">The cached value.</param>
    /// <param name="dependencies">The list of dependencies (or <c>null</c> if there are no dependencies or dependencies were not requested).</param>
    public CacheValue( object? value, IImmutableList<string>? dependencies = null )
    {
        this.Value = value;
        this.Dependencies = dependencies;
    }

    /// <summary>
    /// Returns a new <see cref="CacheValue"/> with different <see cref="Value"/> but identical <see cref="Dependencies"/>.
    /// </summary>
    /// <param name="value">The new value.</param>
    /// <returns>A new <see cref="CacheValue"/> with different <see cref="Value"/> but identical <see cref="Dependencies"/>.</returns>
    public CacheValue WithValue( object? value ) => new( value, this.Dependencies );
}