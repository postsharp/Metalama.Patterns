// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Reflection;

namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Encapsulates information about a parameter of a method
    /// being cached. Exposed by the <see cref="CachedMethodInfo"/> class.
    /// </summary>
    public sealed class CachedParameterInfo
    {
        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> of the parameter.
        /// </summary>
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// Determines whether the parameter should be excluded
        /// from the cache key. When the value of this property is <c>false</c>,
        /// the parameter should be included in the cache key.
        /// </summary>
        public bool IsIgnored { get; }

        internal CachedParameterInfo( ParameterInfo parameter, bool isIgnored )
        {
            this.Parameter = parameter;
            this.IsIgnored = isIgnored;
        }
    }
}
