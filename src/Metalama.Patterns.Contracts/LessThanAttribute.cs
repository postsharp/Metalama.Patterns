// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

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
        : base(
            long.MinValue,
            max,
            long.MinValue,
            max,
            0,
            max < 0 ? 0 : (ulong) max,
            double.MinValue,
            max,
            decimal.MinValue,
            max,
            GetInvalidTypes( long.MinValue, max ),
            shouldTestMinBound: false ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public LessThanAttribute( ulong max )
        : base(
            ulong.MinValue,
            max,
            long.MinValue,
            max > (ulong) long.MaxValue ? long.MaxValue : (long) max,
            0,
            (ulong) max,
            double.MinValue,
            max,
            decimal.MinValue,
            max,
            GetInvalidTypes( ulong.MinValue ),
            shouldTestMinBound: false ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LessThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public LessThanAttribute( double max )
        : base(
            double.MinValue,
            max,
            long.MinValue,
            DoubleMaximum.ToInt64( max ),
            0,
            DoubleMaximum.ToUInt64( max ),
            double.MinValue,
            max,
            decimal.MinValue,
            DoubleMaximum.ToDecimal( max ),
            GetInvalidTypes( double.MinValue, max ),
            shouldTestMinBound: false ) { }

    private static class DoubleMaximum
    {
        public static long ToInt64( double max )
        {
            if ( max < (double) long.MinValue )
            {
                return long.MinValue;
            }

            if ( max > (double) long.MaxValue )
            {
                return long.MaxValue;
            }

            return (long) max;
        }

        public static ulong ToUInt64( double max )
        {
            if ( max < 0 )
            {
                return 0;
            }

            if ( max > (double) ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return (ulong) max;
        }

        public static decimal ToDecimal( double max )
        {
            if ( max > (double) decimal.MaxValue )
            {
                return decimal.MaxValue;
            }

            if ( max < (double) decimal.MinValue )
            {
                return decimal.MinValue;
            }

            return (decimal) max;
        }
    }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnLessThanContractViolated( value, this.DisplayMaxValue );
    }
}