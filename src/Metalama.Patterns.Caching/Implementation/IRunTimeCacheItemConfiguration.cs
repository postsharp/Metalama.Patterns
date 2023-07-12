// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a cached method determined at runtime.
/// </summary>
[PublicAPI]
public interface IRunTimeCacheItemConfiguration : ICompileTimeCacheItemConfiguration
{
    /// <summary>
    /// Gets a value indicating whether caching is enabled.
    /// </summary>
    bool? IsEnabled { get; }
}