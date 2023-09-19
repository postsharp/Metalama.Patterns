// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Dependencies;

/// <summary>
/// Interface that must be implemented by classes that need to be used as cache dependencies,
/// for use with the <see cref="ICachingService.AddDependency"/> method.
/// Alternatively, custom classes may implement the <see cref="Flashtrace.Formatters.IFormattable"/> interface or simply
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
    string GetCacheKey();
}