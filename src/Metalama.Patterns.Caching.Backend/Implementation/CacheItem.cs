// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Represents an item being added to the cache.
/// </summary>
[PublicAPI]
public sealed record CacheItem(
    object? Value,
    IImmutableList<string>? Dependencies = null,
    ICacheItemConfiguration? Configuration = null )
{
    /// <summary>
    /// Determines whether the current <see cref="CacheItem"/> is structurally equal to another <see cref="CacheItem"/>.
    /// </summary>
    /// <param name="other">A <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> both items are equal, otherwise <c>false</c>.</returns>
    public bool Equals( CacheItem? other )
    {
        if ( ReferenceEquals( null, other ) )
        {
            return false;
        }

        if ( ReferenceEquals( this, other ) )
        {
            return true;
        }

        if ( !Equals( this.Value, other.Value ) )
        {
            return false;
        }

        if ( this.Dependencies == null )
        {
            if ( other.Dependencies != null )
            {
                return false;
            }
        }
        else
        {
            if ( other.Dependencies == null )
            {
                return false;
            }

            if ( other.Dependencies.Count != this.Dependencies.Count )
            {
                return false;
            }

            for ( var i = 0; i < this.Dependencies.Count; i++ )
            {
                if ( !string.Equals( this.Dependencies[i], other.Dependencies[i], StringComparison.Ordinal ) )
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = 47;

            hashCode = (hashCode * 53) ^ (this.Value == null ? 0 : this.Value.GetHashCode());

            if ( this.Dependencies != null )
            {
                foreach ( var dependency in this.Dependencies )
                {
                    hashCode = (hashCode * 53) ^ StringComparer.Ordinal.GetHashCode( dependency );
                }
            }

            return hashCode;
        }
    }
}