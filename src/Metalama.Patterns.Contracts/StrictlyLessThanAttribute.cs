// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Implementation;

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is greater than or equal to a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be greater than or equal to the maximum value
///     plus a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the maximum value according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// </remarks>
[PublicAPI]
public class StrictlyLessThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( long max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( int max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( short max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( sbyte max )
        : base( null, RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( ulong max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( uint max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( ushort max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( byte max )
        : base( null, RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( double max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( float max )
        : base( null, RangeBound.Create( max, false ) ) { }

    public StrictlyLessThanAttribute( decimal max )
        : base( null, RangeBound.Create( max, false ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictlyLessThanContractViolated( value, this.Range.MaxValue!.ObjectValue );
    }
}