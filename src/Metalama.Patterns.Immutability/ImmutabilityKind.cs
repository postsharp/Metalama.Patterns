// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Immutability;

[RunTimeOrCompileTime]
public enum ImmutabilityKind
{
    /// <summary>
    /// The type is mutable.
    /// </summary>
    None,

    /// <summary>
    /// The type itself is mutable, but some of its fields or properties may be assigned to a mutable object.
    /// </summary>
    Shallow,

    /// <summary>
    /// The type and all values assigned to its fields and properties are deeply immutable. 
    /// </summary>
    Deep
}