// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Serializers;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Represents an item being added to the cache.
/// </summary>
[PSerializable]
[ImportSerializer( typeof(ImmutableList<>), typeof(ImmutableListPortableSerializer<>) )]
public sealed class CacheItem : IEquatable<CacheItem>
{
    /// <summary>
    /// Initializes a new <see cref="CacheItem"/>.
    /// </summary>
    /// <param name="value">The value to be cached (<c>null</c> is a valid value).</param>
    /// <param name="dependencies">A list of dependencies, or <c>null</c> if there is no dependency.</param>
    /// <param name="configuration">The configuration of the cache item, or <c>null</c> to use the default configuration.</param>
    public CacheItem( object value, IImmutableList<string> dependencies = null, CacheItemConfiguration configuration = null )
    {
        this.Value = value;
        this.Dependencies = dependencies;
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the value to be cached (<c>null</c> is a valid value).
    /// </summary>
    public object Value { get; private set; }

    /// <summary>
    /// Gets the list of dependencies of the current item, or <c>null</c>
    /// if there is no dependency.
    /// </summary>
    public IImmutableList<string> Dependencies { get; private set; }

    /// <summary>
    /// Gets the <see cref="CacheItemConfiguration"/> for the current item,
    /// or <c>null</c> to use the default configuration.
    /// </summary>
    public CacheItemConfiguration Configuration { get; private set; }

    internal CacheItem WithValue( object value )
    {
        var clone = (CacheItem) this.MemberwiseClone();
        clone.Value = value;

        return clone;
    }

    /// <inheritdoc />
    public override bool Equals( object obj )
    {
        if ( obj is CacheItem )
        {
            return this.Equals( (CacheItem) obj );
        }

        return base.Equals( obj );
    }

    /// <summary>
    /// Determines whether two instances of the <see cref="CacheItem"/> class are structurally equal.
    /// </summary>
    /// <param name="first">A <see cref="CacheItem"/>.</param>
    /// <param name="second">A <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> if <paramref name="first"/> equals <paramref name="second"/>, otherwise <c>false</c>.</returns>
    public static bool operator ==( CacheItem first, CacheItem second )
    {
        if ( (object) first == null )
        {
            return (object) second == null;
        }

        return first.Equals( second );
    }

    /// <summary>
    /// Determines whether two instances of the <see cref="CacheItem"/> class are structurally different.
    /// </summary>
    /// <param name="first">A <see cref="CacheItem"/>.</param>
    /// <param name="second">A <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> if <paramref name="first"/> is differnt to <paramref name="second"/>, otherwise <c>false</c>.</returns>
    public static bool operator !=( CacheItem first, CacheItem second )
    {
        return !(first == second);
    }

    /// <summary>
    /// Determines whether the current <see cref="CacheItem"/> is structurally equal to another <see cref="CacheItem"/>.
    /// </summary>
    /// <param name="other">A <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> both items are equal, otherwise <c>false</c>.</returns>
    public bool Equals( CacheItem other )
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

            hashCode = (hashCode * 53) ^ this.Value.GetHashCode();

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