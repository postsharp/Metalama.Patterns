// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Represents an item being added to the cache.
/// </summary>
[PublicAPI]
public sealed class CacheItem : IEquatable<CacheItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheItem"/> class.
    /// </summary>
    /// <param name="value">The value to be cached (<c>null</c> is a valid value).</param>
    /// <param name="dependencies">A list of dependencies, or <c>null</c> if there is no dependency.</param>
    /// <param name="configuration">The configuration of the cache item, or <c>null</c> to use the default configuration.</param>
    public CacheItem( object? value, IImmutableList<string>? dependencies = null, IRunTimeCacheItemConfiguration? configuration = null )
    {
        this.Value = value;
        this.Dependencies = dependencies;
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the value to be cached.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the list of dependencies of the current item, or <c>null</c>
    /// if there is no dependency.
    /// </summary>
    public IImmutableList<string>? Dependencies { get; }

    /// <summary>
    /// Gets the <see cref="IRunTimeCacheItemConfiguration"/> for the current item,
    /// or <c>null</c> to use the default configuration.
    /// </summary>
    public IRunTimeCacheItemConfiguration? Configuration { get; }

    internal CacheItem WithValue( object value ) => new( value, this.Dependencies, this.Configuration );

    /// <inheritdoc />
    public override bool Equals( object? obj )
    {
        if ( obj is CacheItem item )
        {
            return this.Equals( item );
        }

        return ReferenceEquals( obj, this );
    }

    /// <summary>
    /// Determines whether two instances of the <see cref="CacheItem"/> class are structurally equal.
    /// </summary>
    /// <param name="first">The first <see cref="CacheItem"/>.</param>
    /// <param name="second">The second <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> if <paramref name="first"/> equals <paramref name="second"/>, otherwise <c>false</c>.</returns>
    public static bool operator ==( CacheItem? first, CacheItem? second )
    {
        if ( ReferenceEquals( first, null ) )
        {
            return ReferenceEquals( second, null );
        }

        return first.Equals( second );
    }

    /// <summary>
    /// Determines whether two instances of the <see cref="CacheItem"/> class are structurally different.
    /// </summary>
    /// <param name="first">The first <see cref="CacheItem"/>.</param>
    /// <param name="second">The second <see cref="CacheItem"/>.</param>
    /// <returns><c>true</c> if <paramref name="first"/> is different to <paramref name="second"/>, otherwise <c>false</c>.</returns>
    public static bool operator !=( CacheItem first, CacheItem second ) => !(first == second);

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