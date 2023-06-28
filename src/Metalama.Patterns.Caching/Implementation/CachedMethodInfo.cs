// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Immutable;
using System.Reflection;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a method being cached.
/// </summary>
[PublicAPI] // TODO: [Porting] Does CachedMethodInfo need to be public?
public sealed class CachedMethodInfo
{
    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the method.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the <c>this</c> parameter
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