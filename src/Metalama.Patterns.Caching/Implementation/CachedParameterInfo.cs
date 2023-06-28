// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Reflection;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a parameter of a method
/// being cached. Exposed by the <see cref="CachedMethodInfo"/> class.
/// </summary>
[PublicAPI] // TODO: [Porting] Does CachedParameterInfo need to be public? 
public sealed class CachedParameterInfo
{
    /// <summary>
    /// Gets the <see cref="ParameterInfo"/> of the parameter.
    /// </summary>
    public ParameterInfo Parameter { get; }

    /// <summary>
    /// Gets a value indicating whether the parameter should be excluded
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