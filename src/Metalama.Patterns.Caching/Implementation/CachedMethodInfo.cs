// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Immutable;
using System.Reflection;

namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Encapsulates information about a method being cached.
    /// </summary>
    public sealed class CachedMethodInfo
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of the method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Determines whether the value of the <c>this</c> parameter
        /// (for non-static methods) should be included in the cache key.
        /// </summary>
        public bool IsThisParameterIgnored { get; }

        /// <summary>
        /// Gets an array of <see cref="CachedParameterInfo"/>.
        /// </summary>
        public ImmutableArray<CachedParameterInfo> Parameters { get; }
        
        internal CachedMethodInfo( MethodInfo method, bool isThisParameterIgnored, ImmutableArray<CachedParameterInfo> parameters )
        {
            this.Method = method;
            this.Parameters = parameters;
            this.IsThisParameterIgnored = isThisParameterIgnored;
        }
    }
}
