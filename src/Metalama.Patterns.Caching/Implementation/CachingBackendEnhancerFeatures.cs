// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.Implementation
{

    /// <summary>
    /// An abstract implementation of <see cref="CachingBackendEnhancerFeatures"/> for use in implementations of <see cref="CachingBackendEnhancer"/>,
    /// where the default behavior is to return the features of the underlying <see cref="CachingBackend"/>.
    /// </summary>
    public abstract class CachingBackendEnhancerFeatures : CachingBackendFeatures
    {
        /// <summary>
        /// Gets the feature of the underlying <see cref="CachingBackend"/>.
        /// </summary>
        protected CachingBackendFeatures UnderlyingBackendFeatures { get; }

        /// <summary>
        /// Initializes a new <see cref="CachingBackendEnhancerFeatures"/>.
        /// </summary>
        /// <param name="underlyingBackendFeatures">The feature of the underlying <see cref="CachingBackend"/>.</param>
        protected CachingBackendEnhancerFeatures( [Required] CachingBackendFeatures underlyingBackendFeatures )
        {
            this.UnderlyingBackendFeatures = underlyingBackendFeatures;
        }

        /// <inheritdoc />
        public override bool Clear => this.UnderlyingBackendFeatures.Clear;

        /// <inheritdoc />  
        public override bool Events => this.UnderlyingBackendFeatures.Events;

        /// <inheritdoc />
        public override bool Blocking => this.UnderlyingBackendFeatures.Blocking;

        /// <inheritdoc />
        public override bool Dependencies => this.UnderlyingBackendFeatures.Dependencies;

        /// <inheritdoc />
        public override bool ContainsDependency => this.UnderlyingBackendFeatures.ContainsDependency;
    }

}
