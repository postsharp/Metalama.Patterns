// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching
{
    /// <summary>
    /// Enumerates the kinds of <see cref="ICachingContext"/>.
    /// </summary>
    [Flags]
#pragma warning disable CA1714 // Flags enums should have plural names
    public enum CachingContextKind
    {
        /// <summary>
        /// None (a null implementation of <see cref="ICachingContext"/>).
        /// </summary>
        None,

        /// <summary>
        /// The <see cref="ICachingContext"/> of a method being evaluated and added to the cache.
        /// </summary>
        Cache = 1,

        /// <summary>
        /// The <see cref="ICachingContext"/> of a method being re-evaluated, ignoring the previous value, and replaced into the cache, using the
        /// <see cref="CachingServices.Invalidation.Recache{TReturn}"/> method.
        /// </summary>
        Recache = 2
    }
}
