// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

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
        : base(
            long.MinValue,
            max,
            long.MinValue,
            FloatingPointHelper.Int64Maximum.ToInt64( max ),
            ulong.MinValue,
            FloatingPointHelper.Int64Maximum.ToUInt64( max ),
            double.MinValue,
            FloatingPointHelper.Int64Maximum.ToDouble( max ),
            decimal.MinValue,
            FloatingPointHelper.Int64Maximum.ToDecimal( max ),
            GetInvalidTypes( long.MinValue, FloatingPointHelper.Int64Maximum.ToInt64( max ) ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( ulong max )
        : base(
            ulong.MinValue,
            max,
            long.MinValue,
            FloatingPointHelper.UInt64Maximum.ToInt64( max ),
            ulong.MinValue,
            FloatingPointHelper.UInt64Maximum.ToUInt64( max ),
            double.MinValue,
            FloatingPointHelper.UInt64Maximum.ToDouble( max ),
            decimal.MinValue,
            FloatingPointHelper.UInt64Maximum.ToDecimal( max ),
            GetInvalidTypes( 0 ) | (max == 0 ? TypeFlag.UInt16 | TypeFlag.UInt32 | TypeFlag.UInt64 : TypeFlag.None) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( double max )
        : base(
            double.MinValue,
            max,
            long.MinValue,
            FloatingPointHelper.DoubleMaximum.ToInt64( max ),
            ulong.MinValue,
            FloatingPointHelper.DoubleMaximum.ToUInt64( max ),
            double.MinValue,
            FloatingPointHelper.DoubleMaximum.ToDouble( max ),
            decimal.MinValue,
            FloatingPointHelper.DoubleMaximum.ToDecimal( max ),
            GetInvalidTypes( double.MinValue, max > double.MinValue + 1 ? max + 1 : double.MinValue ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictlyLessThanContractViolated( value, this.DisplayMaxValue );
    }
}