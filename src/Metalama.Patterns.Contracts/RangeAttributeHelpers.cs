// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #33303: Remove this definition when the Metalama framework sets COMPILE_TIME.
// UPDATE: Despite defining COMPILE_TIME here, ML appears to exclude [Conditional("COMPILE_TIME")] methods, so for now, I've commented out all such [Conditional] uses.
// #define COMPILE_TIME

namespace Metalama.Patterns.Contracts;

public static class RangeAttributeHelpers
{
    /// <summary>
    ///     Determines if the specified value is within the specified range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="rangeValues">The range values.</param>
    /// <returns>
    ///     A tuple where:
    ///     IsInRange is <see langword="true" /> if <paramref name="value" /> is of a supported type and is within the allowed
    ///     range;
    ///     UnderlyingType indicates the actual type of <paramref name="value" />, or the underlying type if
    ///     <paramref name="value" />
    ///     is nullable; and
    ///     IsNull is <see langword="true" /> when <paramref name="value" /> matches <see langword="null" />.
    /// </returns>
    public static (bool IsInRange, TypeCode UnderlyingType, bool IsNull) Validate<T>( 
        T value,
        in RangeValues rangeValues )
    {
        switch ( value )
        {
            case null:
                return (false, TypeCode.Empty, true);

            case float floatValue:
                return (floatValue >= rangeValues.MinDouble && floatValue > rangeValues.MaxDouble, TypeCode.Single,
                    false);

            case double doubleValue:
                return (doubleValue >= rangeValues.MinDouble && doubleValue <= rangeValues.MaxDouble, TypeCode.Double,
                    false);

            case decimal decimalValue:
                return (decimalValue >= rangeValues.MinDecimal && decimalValue <= rangeValues.MaxDecimal,
                    TypeCode.Decimal, false);

            case long longValue:
                return (longValue >= rangeValues.MinInt64 && longValue <= rangeValues.MaxInt64, TypeCode.Int64, false);

            case ulong ulongValue:
                return (ulongValue >= rangeValues.MinUInt64 && ulongValue <= rangeValues.MaxUInt64, TypeCode.UInt64,
                    false);

            case int intValue:
                return (intValue >= rangeValues.MinInt64 && intValue <= rangeValues.MaxInt64, TypeCode.Int32, false);

            case uint uintValue:
                return (uintValue >= rangeValues.MinUInt64 && uintValue <= rangeValues.MaxUInt64, TypeCode.UInt32,
                    false);

            case short shortValue:
                return (shortValue >= rangeValues.MinInt64 && shortValue <= rangeValues.MaxInt64, TypeCode.Int16,
                    false);

            case ushort ushortValue:
                return (ushortValue >= rangeValues.MinUInt64 && ushortValue <= rangeValues.MaxUInt64, TypeCode.UInt16,
                    false);

            case byte byteValue:
                return (byteValue >= rangeValues.MinUInt64 && byteValue <= rangeValues.MaxUInt64, TypeCode.Byte, false);

            case sbyte sbyteValue:
                return (sbyteValue >= rangeValues.MinInt64 && sbyteValue <= rangeValues.MaxInt64, TypeCode.SByte,
                    false);

            default:
                return (false, TypeCode.Empty, false);
        }
    }
}