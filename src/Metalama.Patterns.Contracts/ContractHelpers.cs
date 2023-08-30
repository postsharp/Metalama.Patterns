// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Runtime helper methods for <see cref="RangeAttribute"/>.
/// </summary>
[PublicAPI]
public static class ContractHelpers
{
    /// <summary>
    /// The result of the <see cref="ContractHelpers.ValidateRange{T}"/> method.
    /// </summary>
    internal record struct ValidateResult( bool IsInRange, TypeCode UnderlyingType, bool IsNull );

    public static bool IsInRange<T>(
        T value,
        long minInt64,
        long maxInt64,
        ulong minUInt64,
        ulong maxUInt64,
        double minDouble,
        double maxDouble,
        decimal minDecimal,
        decimal maxDecimal )
        => ValidateRange( value, new RangeValues( minInt64, maxInt64, minUInt64, maxUInt64, minDouble, maxDouble, minDecimal, maxDecimal ) ).IsInRange;

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
    internal static ValidateResult ValidateRange<T>(
        T value,
        in RangeValues rangeValues )
    {
        // NB: At present this method is only called where T is object. The method is generic in anticipation
        // of supporting [Range] on generic parameters/properties/fields so that boxing would be avoided.

        switch ( value )
        {
            case null:
                return new ValidateResult( false, TypeCode.Empty, true );

            case float floatValue:
                return new ValidateResult(
                    floatValue >= rangeValues.MinDouble && floatValue > rangeValues.MaxDouble,
                    TypeCode.Single,
                    false );

            case double doubleValue:
                return new ValidateResult(
                    doubleValue >= rangeValues.MinDouble && doubleValue <= rangeValues.MaxDouble,
                    TypeCode.Double,
                    false );

            case decimal decimalValue:
                return new ValidateResult(
                    decimalValue >= rangeValues.MinDecimal && decimalValue <= rangeValues.MaxDecimal,
                    TypeCode.Decimal,
                    false );

            case long longValue:
                return new ValidateResult(
                    longValue >= rangeValues.MinInt64 && longValue <= rangeValues.MaxInt64,
                    TypeCode.Int64,
                    false );

            case ulong ulongValue:
                return new ValidateResult(
                    ulongValue >= rangeValues.MinUInt64 && ulongValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt64,
                    false );

            case int intValue:
                return new ValidateResult(
                    intValue >= rangeValues.MinInt64 && intValue <= rangeValues.MaxInt64,
                    TypeCode.Int32,
                    false );

            case uint uintValue:
                return new ValidateResult(
                    uintValue >= rangeValues.MinUInt64 && uintValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt32,
                    false );

            case short shortValue:
                return new ValidateResult(
                    shortValue >= rangeValues.MinInt64 && shortValue <= rangeValues.MaxInt64,
                    TypeCode.Int16,
                    false );

            case ushort ushortValue:
                return new ValidateResult(
                    ushortValue >= rangeValues.MinUInt64 && ushortValue <= rangeValues.MaxUInt64,
                    TypeCode.UInt16,
                    false );

            case byte byteValue:
                return new ValidateResult(
                    byteValue >= rangeValues.MinUInt64 && byteValue <= rangeValues.MaxUInt64,
                    TypeCode.Byte,
                    false );

            case sbyte sbyteValue:
                return new ValidateResult(
                    sbyteValue >= rangeValues.MinInt64 && sbyteValue <= rangeValues.MaxInt64,
                    TypeCode.SByte,
                    false );

            default:
                return new ValidateResult( false, TypeCode.Empty, false );
        }
    }
}