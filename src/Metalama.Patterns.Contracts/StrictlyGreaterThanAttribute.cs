// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

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
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.StrictlyGreaterThanErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used.</para>
/// </remarks>
public class StrictlyGreaterThanAttribute : RangeAttribute
{
    internal static class Int64Minimum
    {
        public static long ToInt64( long min )
        {
            if ( min == long.MaxValue )
            {
                return long.MaxValue;
            }

            return min + 1;
        }

        public static ulong ToUInt64( long min )
        {
            if ( min < 0 )
            {
                return 0;
            }

            return (ulong) min + 1;
        }

        public static double ToDouble( long min ) => (double) min + FloatingPointHelper.GetDoubleStep( (double) min );

        public static decimal ToDecimal( long min ) =>
            (decimal) min + FloatingPointHelper.GetDecimalStep( (decimal) min );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( long min )
        : base( 
            min,
            long.MaxValue,
            Int64Minimum.ToInt64( min ),
            long.MaxValue,
            Int64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            Int64Minimum.ToDouble( min ),
            double.MaxValue,
            Int64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( Int64Minimum.ToInt64( min ), long.MaxValue )
        )
    {
    }

    internal static class UInt64Minimum
    {
        public static long ToInt64( ulong min )
        {
            if ( min > (ulong) long.MaxValue + 1 )
            {
                return long.MaxValue;
            }

            return (long) min + 1;
        }

        public static ulong ToUInt64( ulong min )
        {
            if ( min == ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return min + 1;
        }

        public static double ToDouble( ulong min ) => (double) min + FloatingPointHelper.GetDoubleStep( (double) min );

        public static decimal ToDecimal( ulong min ) =>
            (decimal) min + FloatingPointHelper.GetDecimalStep( (decimal) min );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( ulong min )
        : base( 
            min,
            ulong.MaxValue,
            UInt64Minimum.ToInt64( min ),
            long.MaxValue,
            UInt64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            UInt64Minimum.ToDouble( min ),
            double.MaxValue,
            UInt64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( UInt64Minimum.ToUInt64( min ) )
        )
    {
    }

    internal static class DoubleMinimum
    {
        public static long ToInt64( double min )
        {
            if ( min < (double) long.MinValue )
            {
                return long.MinValue;
            }

            if ( min + 1 > (double) long.MaxValue )
            {
                return long.MaxValue;
            }

            return (long) min + 1;
        }

        public static ulong ToUInt64( double min )
        {
            if ( min < 0 )
            {
                return 0;
            }

            if ( min + 1 > (double) ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return (ulong) min + 1;
        }

        public static double ToDouble( double min )
        {
            if ( Math.Abs( min ) <= double.Epsilon )
            {
                return double.Epsilon;
            }

            var step = FloatingPointHelper.GetDoubleStep( min );

            if ( min >= double.MaxValue - step )
            {
                return double.MaxValue;
            }

            return min + step;
        }

        public static decimal ToDecimal( double min )
        {
            if ( min > (double) decimal.MaxValue )
            {
                return decimal.MaxValue;
            }

            if ( min < (double) decimal.MinValue )
            {
                return decimal.MinValue;
            }

            var step = FloatingPointHelper.GetDecimalStep( (decimal) min );

            if ( min > (double) decimal.MaxValue - (double) step )
            {
                return decimal.MaxValue;
            }

            if ( Math.Abs( min ) <= (double) step )
            {
                return step;
            }

            return (decimal) min + step;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( double min )
        : base( 
            min,
            double.MaxValue,
            DoubleMinimum.ToInt64( min ),
            long.MaxValue,
            DoubleMinimum.ToUInt64( min ),
            ulong.MaxValue,
            DoubleMinimum.ToDouble( min ),
            double.MaxValue,
            DoubleMinimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( min < double.MaxValue - 1 ? min + 1 : double.MaxValue, double.MaxValue ) )
    {
    }

    /// <inheridoc />
    protected override (IExpression MessageIdExpression, bool IncludeMinValue, bool IncludeMaxValue) GetExceptioninfo()
        => (
            CompileTimeHelpers.GetContractLocalizedTextProviderField( nameof(ContractLocalizedTextProvider
                .StrictlyGreaterThanErrorMessage) ),
            true, false);
}