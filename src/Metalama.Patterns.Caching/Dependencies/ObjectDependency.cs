// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Dependencies
{
    /// <summary>
    /// Wraps an <see cref="object"/> into an <see cref="ObjectDependency"/>. The <see cref="GetCacheKey"/>
    /// relies on the <see cref="CachingServices.DefaultKeyBuilder"/> to create the cache key of the wrapped object.
    /// </summary>
    public sealed class ObjectDependency : ICacheDependency
    {
        /// <summary>
        /// Gets the wrapped object.
        /// </summary>
        public object Object { get; }

        /// <inheritdoc />
        public string GetCacheKey()
        {
            return CachingServices.DefaultKeyBuilder.BuildDependencyKey( this.Object );
        }

        /// <summary>
        /// Initializes a new <see cref="ObjectDependency"/>.
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
        public bool Equals( [Required] ICacheDependency other )
        {
            if ( !(other is ObjectDependency otherObjectDependency) )
            {
                return false;
            }
            else
            {
                return Equals( this.Object, otherObjectDependency.Object );
            }
        }

        /// <inheritdoc />
        public override bool Equals( object obj )
        {
            return this.Equals( obj as ICacheDependency );
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Object.GetHashCode();
        }
    }
}