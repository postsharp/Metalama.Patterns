// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Implementation;

// TODO: Modernization - consider deprecating WithValue and relying on C# 'with' expressions. Requires conversion to record in C#9, C#10 supports 'with' with struct.
/// <summary>
/// Return value of the <see cref="CachingBackend.GetItem(string, bool)"/> method, i.e. represents an item retrieved from the cache (items being stored in the cache are represented by the <see cref="CacheItem"/> class).
/// </summary>
/// <param name="Value">The cached value.</param>
/// <param name="Dependencies">An optional list of dependencies.</param>
[PublicAPI]
public record class CacheValue( object? Value, IImmutableList<string>? Dependencies = null );