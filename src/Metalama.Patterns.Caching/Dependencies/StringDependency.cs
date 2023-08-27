// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching.Dependencies;

/// <summary>
/// A cache dependency that is already represented as a string.
/// </summary>
[PublicAPI]
public sealed class StringDependency : ICacheDependency
{
    private readonly string _key;

    /// <inheritdoc />
    public string GetCacheKey() => this._key;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringDependency"/> class.
    /// </summary>
    /// <param name="key">The cache dependency.</param>
    public StringDependency( [Required] string key )
    {
        this._key = key;
    }

    /// <summary>Determines whether the specified object is equal to the current object.</summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>
    /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
    public bool Equals( ICacheDependency? other )
    {
        if ( other is StringDependency otherStringDependency )
        {
            return string.Equals( this.GetCacheKey(), otherStringDependency.GetCacheKey(), StringComparison.Ordinal );
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc />
    public override bool Equals( object? obj ) => this.Equals( obj as StringDependency );

    /// <inheritdoc />  
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode( this.GetCacheKey() );
}