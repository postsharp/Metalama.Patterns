// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Dependencies
{
    /// <summary>
    /// A cache dependency that is already represented as a string.
    /// </summary>
    public sealed class StringDependency : ICacheDependency
    {
        private readonly string key;

#pragma warning disable CA1024 // Use properties where appropriate
                              /// <inheritdoc />
        public string GetCacheKey()
        {
            return this.key;
        }
#pragma warning restore CA1024 // Use properties where appropriate

        /// <summary>
        /// Initializes a new <see cref="StringDependency"/>.
        /// </summary>
        /// <param name="key">The cache dependency.</param>
        public StringDependency([Required] string key)
        {
            this.key = key;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public bool Equals(ICacheDependency other)
        {
            StringDependency otherObjectDependency = other as StringDependency;
            if (otherObjectDependency == null)
            {
                return false;
            }

            return string.Equals(this.GetCacheKey(), otherObjectDependency.GetCacheKey(), StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public override bool Equals( object obj )
        {
            return this.Equals( obj as StringDependency);
        }

        /// <inheritdoc />  
        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(this.GetCacheKey());
        }
    }
}
