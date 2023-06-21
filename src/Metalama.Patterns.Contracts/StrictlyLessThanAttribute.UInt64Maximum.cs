// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

// Resharper disable RedundantCast

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts;

public partial class StrictlyLessThanAttribute
{
    [RunTimeOrCompileTime]
    internal static class UInt64Maximum
    {
        public static long ToInt64( ulong max )
        {
            if ( max > (ulong) long.MaxValue + 1 )
            {
                return long.MaxValue;
            }

            return (long) max - 1;
        }

        public static ulong ToUInt64( ulong max )
        {
            if ( max == 0 )
            {
                return 0;
            }

            return max - 1;
        }

        public static double ToDouble( ulong max ) => (double) max - FloatingPointHelper.GetDoubleStep( (double) max );

        public static decimal ToDecimal( ulong max ) => (decimal) max + FloatingPointHelper.GetDecimalStep( (decimal) max );
    }
}