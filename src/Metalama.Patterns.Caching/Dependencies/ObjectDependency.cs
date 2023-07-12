// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Dependencies;

/// <summary>
/// Wraps an <see cref="object"/> into an <see cref="ObjectDependency"/>. The <see cref="GetCacheKey"/>
/// relies on the <see cref="CachingServices.DefaultKeyBuilder"/> to create the cache key of the wrapped object.
/// </summary>
[PublicAPI]
public sealed class ObjectDependency : ICacheDependency
{
    /// <summary>
    /// Gets the wrapped object.
    /// </summary>
    public object Object { get; }

    /// <inheritdoc />
    public string GetCacheKey() => CachingServices.DefaultKeyBuilder.BuildDependencyKey( this.Object );

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDependency"/> class.
    /// </summary>
    /// <param name="dependencyObject">The wrapped object.</param>
    public ObjectDependency( [Required] object dependencyObject )
    {
        this.Object = dependencyObject;
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public bool Equals( ICacheDependency? other )
    {
        // [Porting] NB: 'other' was [Required] but this did not make much sense, especially given the impl of Equals( object? obj ),
        // and it broke nullable.

        if ( other is ObjectDependency otherObjectDependency )
        {
            return Equals( this.Object, otherObjectDependency.Object );
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc />
    public override bool Equals( object? obj ) => this.Equals( obj as ICacheDependency );

    /// <inheritdoc />
    public override int GetHashCode() => this.Object.GetHashCode();
}