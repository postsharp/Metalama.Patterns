// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is smaller than or equal to a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be greater than or equal to the minimum value
///     plus a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the minimum value according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// </remarks>
[PublicAPI]
public class StrictlyGreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( long min )
        : base(
            min,
            long.MaxValue,
            FloatingPointHelper.Int64Minimum.ToInt64( min ),
            long.MaxValue,
            FloatingPointHelper.Int64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            FloatingPointHelper.Int64Minimum.ToDouble( min ),
            double.MaxValue,
            FloatingPointHelper.Int64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( FloatingPointHelper.Int64Minimum.ToInt64( min ), long.MaxValue ),
            shouldTestMaxBound: false ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( ulong min )
        : base(
            min,
            ulong.MaxValue,
            FloatingPointHelper.UInt64Minimum.ToInt64( min ),
            long.MaxValue,
            FloatingPointHelper.UInt64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            FloatingPointHelper.UInt64Minimum.ToDouble( min ),
            double.MaxValue,
            FloatingPointHelper.UInt64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( FloatingPointHelper.UInt64Minimum.ToUInt64( min ) ),
            shouldTestMaxBound: false ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( double min )
        : base(
            min,
            double.MaxValue,
            FloatingPointHelper.DoubleMinimum.ToInt64( min ),
            long.MaxValue,
            FloatingPointHelper.DoubleMinimum.ToUInt64( min ),
            ulong.MaxValue,
            FloatingPointHelper.DoubleMinimum.ToDouble( min ),
            double.MaxValue,
            FloatingPointHelper.DoubleMinimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( min < double.MaxValue - 1 ? min + 1 : double.MaxValue, double.MaxValue ),
            shouldTestMaxBound: false ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictlyGreaterThanContractViolated( value, this.DisplayMinValue );
    }
}