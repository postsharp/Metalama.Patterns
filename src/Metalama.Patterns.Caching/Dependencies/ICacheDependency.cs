// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.Dependencies
{
    /// <summary>
    /// Interface that must be implemented by classes that need to be used as cache dependencies,
    /// for use with the <see cref="ICachingContext.AddDependency(ICacheDependency)"/> method.
    /// Alternatively, custom classes may implement the <see cref="Patterns.Formatters.IFormattable"/> interface or simply
    /// the <see cref="object.ToString"/> method.
    /// </summary>
    public interface ICacheDependency 
    {
        /// <summary>
        /// Gets a string that uniquely represents the current object.
        /// </summary>
        /// <returns>A string that uniquely represents the current object.</returns>
        /// <remarks>
        /// <para>The returned key should be globally unique, not just unique within the class implementing the method.</para>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetCacheKey();
    }
}
