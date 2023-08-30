// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is not strictly within the given range.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be strictly within the given bounds
///     with a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the bounds according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// </remarks>
[PublicAPI]
public class StrictRangeAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( long min, long max )
        : base(
            min,
            max,
            FloatingPointHelper.Int64Minimum.ToInt64( min ),
            FloatingPointHelper.Int64Maximum.ToInt64( max ),
            FloatingPointHelper.Int64Minimum.ToUInt64( min ),
            FloatingPointHelper.Int64Maximum.ToUInt64( max ),
            FloatingPointHelper.Int64Minimum.ToDouble( min ),
            FloatingPointHelper.Int64Maximum.ToDouble( max ),
            FloatingPointHelper.Int64Minimum.ToDecimal( min ),
            FloatingPointHelper.Int64Maximum.ToDecimal( max ),
            GetInvalidTypes(
                FloatingPointHelper.Int64Minimum.ToInt64( min ),
                FloatingPointHelper.Int64Maximum.ToInt64( max ) ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying unsigned integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( ulong min, ulong max )
        : base(
            min,
            max,
            FloatingPointHelper.UInt64Minimum.ToInt64( min ),
            FloatingPointHelper.UInt64Maximum.ToInt64( max ),
            FloatingPointHelper.UInt64Minimum.ToUInt64( min ),
            FloatingPointHelper.UInt64Maximum.ToUInt64( max ),
            FloatingPointHelper.UInt64Minimum.ToDouble( min ),
            FloatingPointHelper.UInt64Maximum.ToDouble( max ),
            FloatingPointHelper.UInt64Minimum.ToDecimal( min ),
            FloatingPointHelper.UInt64Maximum.ToDecimal( max ),
            GetInvalidTypes( FloatingPointHelper.UInt64Minimum.ToUInt64( min ) ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying floating-point bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( double min, double max )
        : base(
            min,
            max,
            FloatingPointHelper.DoubleMinimum.ToInt64( min ),
            FloatingPointHelper.DoubleMaximum.ToInt64( max ),
            FloatingPointHelper.DoubleMinimum.ToUInt64( min ),
            FloatingPointHelper.DoubleMaximum.ToUInt64( max ),
            FloatingPointHelper.DoubleMinimum.ToDouble( min ),
            FloatingPointHelper.DoubleMaximum.ToDouble( max ),
            FloatingPointHelper.DoubleMinimum.ToDecimal( min ),
            FloatingPointHelper.DoubleMaximum.ToDecimal( max ),
            GetInvalidTypes(
                min < double.MaxValue - 1 ? min + 1 : double.MaxValue,
                max > double.MinValue + 1 ? max - 1 : double.MinValue ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.Project.ContractOptions().Templates.OnStrictRangeContractViolated( value, this.DisplayMinValue, this.DisplayMaxValue );
    }
}