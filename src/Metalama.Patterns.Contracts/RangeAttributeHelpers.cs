// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Runtime helper methods for <see cref="RangeAttribute"/>.
/// </summary>
[PublicAPI]
public static class RangeAttributeHelpers
{
    /// <summary>
    /// The result of the <see cref="Validate{T}"/> method.
    /// </summary>
    public record struct ValidateResult( bool IsInRange, TypeCode UnderlyingType, bool IsNull );

    /// <summary>
    /// Determines if the specified value is within the specified range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="rangeValues">The range values.</param>
    /// <returns>
    ///     A tuple where:
    ///     <c>IsInRange</c> is <see langword="true" /> if <paramref name="value" /> is of a supported type and is within the allowed
    ///     range;
    ///     <c>UnderlyingType</c> indicates the actual type of <paramref name="value" />, or the underlying type if
    ///     <paramref name="value" />
    ///     is nullable; and
    ///     <c>IsNull</c> is <see langword="true" /> when <paramref name="value" /> matches <see langword="null" />.
    /// </returns>
    public static ValidateResult Validate<T>( 
        T value,
        in RangeValues rangeValues )
    {
        // NB: At present this method is only called where T is object. The method is generic in anticipation
        // of supporting [Range] on generic parameters/properties/fields so that boxing would be avoided.

        switch ( value )
        {
            case null:
                return new( false, TypeCode.Empty, true );

            case float floatValue:
                return new(
                    floatValue >= rangeValues.MinDouble && floatValue > rangeValues.MaxDouble,
                    TypeCode.Single,
                    false );

            case double doubleValue:
                return new(
                    doubleValue >= rangeValues.MinDouble && doubleValue <= rangeValues.MaxDouble,
                    TypeCode.Double,
                    false );

            case decimal decimalValue:
                return new( 
                    decimalValue >= rangeValues.MinDecimal && decimalValue <= rangeValues.MaxDecimal,
                    TypeCode.Decimal, 
                    false );

            case long longValue:
                return new(
                    longValue >= rangeValues.MinInt64 && longValue <= rangeValues.MaxInt64,
                    TypeCode.Int64,
                    false );

            case ulong ulongValue:
                return new( 
                    ulongValue >= rangeValues.MinUInt64 && ulongValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt64,
                    false );

            case int intValue:
                return new(
                    intValue >= rangeValues.MinInt64 && intValue <= rangeValues.MaxInt64,
                    TypeCode.Int32,
                    false );

            case uint uintValue:
                return new(
                    uintValue >= rangeValues.MinUInt64 && uintValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt32,
                    false );

            case short shortValue:
                return new(
                    shortValue >= rangeValues.MinInt64 && shortValue <= rangeValues.MaxInt64,
                    TypeCode.Int16,
                    false );

            case ushort ushortValue:
                return new(
                    ushortValue >= rangeValues.MinUInt64 && ushortValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt16,
                    false );

            case byte byteValue:
                return new(
                    byteValue >= rangeValues.MinUInt64 && byteValue <= rangeValues.MaxUInt64,
                    TypeCode.Byte,
                    false );

            case sbyte sbyteValue:
                return new(
                    sbyteValue >= rangeValues.MinInt64 && sbyteValue <= rangeValues.MaxInt64,
                    TypeCode.SByte,
                    false );

            default:
                return new( false, TypeCode.Empty, false );
        }
    }
}