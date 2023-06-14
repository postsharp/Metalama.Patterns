// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is smaller than a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.GreaterThanErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used.</para>
/// </remarks>
public class GreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( long min )
        : base( 
            min,
            long.MaxValue,
            min,
            long.MaxValue,
            min < 0 ? 0 : (ulong) min,
            ulong.MaxValue,
            min,
            double.MaxValue,
            min,
            decimal.MaxValue,
            GetInvalidTypes( min, long.MaxValue )
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( ulong min )
        : base( 
            min,
            ulong.MaxValue,
            min > (ulong) long.MaxValue ? long.MaxValue : (long) min,
            long.MaxValue,
            min,
            ulong.MaxValue,
            min,
            double.MaxValue,
            min,
            decimal.MaxValue,
            GetInvalidTypes( min )
        )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( double min )
        : base( 
            min,
            double.MaxValue,
            DoubleMinimum.ToInt64( min ),
            long.MaxValue,
            DoubleMinimum.ToUInt64( min ),
            ulong.MaxValue,
            min,
            double.MaxValue,
            DoubleMinimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( min, double.MaxValue )
        )
    {
    }

    private static class DoubleMinimum
    {
        public static long ToInt64( double min )
        {
            if ( min < (double) long.MinValue )
            {
                return long.MinValue;
            }

            if ( min > (double) long.MaxValue )
            {
                return long.MaxValue;
            }

            return (long) min;
        }

        public static ulong ToUInt64( double min )
        {
            if ( min < 0 )
            {
                return 0;
            }

            if ( min > (double) ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return (ulong) min;
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

            return (decimal) min;
        }
    }

    /// <inheritdoc/>
    protected override (IExpression MessageIdExpression, bool IncludeMinValue, bool IncludeMaxValue) GetExceptioninfo()
        => (
            CompileTimeHelpers.GetContractLocalizedTextProviderField( nameof(ContractLocalizedTextProvider
                .GreaterThanErrorMessage) ),
            true, false);
}