// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value
/// greater than zero.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// </remarks>
[PublicAPI]
public class NegativeAttribute : LessThanAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeAttribute"/> class.
    /// </summary>
    public NegativeAttribute() : base( 0 ) { }
}