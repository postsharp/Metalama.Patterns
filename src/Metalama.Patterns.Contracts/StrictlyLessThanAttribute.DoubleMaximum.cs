// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

// Resharper disable RedundantCast

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts;

public partial class StrictlyLessThanAttribute
{
    [RunTimeOrCompileTime]
    internal static class DoubleMaximum
    {
        public static long ToInt64( double max )
        {
            if ( max - 1 < (double) long.MinValue )
            {
                return long.MinValue;
            }

            if ( max > (double) long.MaxValue )
            {
                return long.MaxValue;
            }

            return (long) max - 1;
        }

        public static ulong ToUInt64( double max )
        {
            if ( max - 1 < 0 )
            {
                return 0;
            }

            if ( max > (double) ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return (ulong) max - 1;
        }

        public static double ToDouble( double max )
        {
            if ( Math.Abs( max ) <= double.Epsilon )
            {
                return double.Epsilon;
            }

            var step = FloatingPointHelper.GetDoubleStep( max );

            if ( max <= double.MinValue + step )
            {
                return double.MinValue;
            }

            return max - step;
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

            var step = FloatingPointHelper.GetDecimalStep( (decimal) max );

            if ( max < (double) decimal.MinValue + (double) step )
            {
                return decimal.MinValue;
            }

            if ( Math.Abs( max ) <= (double) step )
            {
                return step;
            }

            return (decimal) max - step;
        }
    }
}