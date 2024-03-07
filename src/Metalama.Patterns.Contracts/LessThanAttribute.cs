// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Implementation;

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is greater than a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used.</para>
/// </remarks>
[PublicAPI]
public class LessThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public LessThanAttribute( long max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( int max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( short max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( sbyte max )
        : base( default, RangeBound.Create( max ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public LessThanAttribute( ulong max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( uint max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( ushort max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( byte max )
        : base( default, RangeBound.Create( max ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public LessThanAttribute( double max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( float max )
        : base( default, RangeBound.Create( max ) ) { }

    public LessThanAttribute( decimal max )
        : base( default, RangeBound.Create( max ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnLessThanContractViolated( value, this.Range.MaxValue.ObjectValue );
    }
}