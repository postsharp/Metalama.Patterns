// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Runtime helper methods for <see cref="RangeAttribute"/>.
/// </summary>
[PublicAPI]
public static class ContractHelpers
{
    private static readonly ConcurrentDictionary<(string Pattern, RegexOptions Options), Regex> _cachedRegexes = new();

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

    public static Func<string?, bool> IsValidCreditCardNumber { get; set; } = value =>
    {
        if ( value == null )
        {
            return true;
        }

#if NET5_0_OR_GREATER
        var str2 =
            value.Replace( "-", "", StringComparison.OrdinalIgnoreCase )
                .Replace( " ", "", StringComparison.OrdinalIgnoreCase );
#else
        var str2 = value.Replace( "-", "" ).Replace( " ", "" );
#endif
        var checksum = 0;
        var toggle = false;

        foreach ( var digit in str2.Reverse() )
        {
            if ( digit < 48 || digit > 57 )
            {
                return false;
            }

            var digitChecksum = (digit - 48) * (toggle ? 2 : 1);
            toggle = !toggle;

            while ( digitChecksum > 0 )
            {
                checksum += digitChecksum % 10;
                digitChecksum /= 10;
            }
        }

        return checksum % 10 == 0;
    };

    public static Regex EmailAddressRegex { get; set; } = new(
        "^((([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+(\\.([a-z]|\\d|[!#\\$%&'\\*\\+\\-\\/=\\?\\^_`{\\|}~]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])+)*)|((\\x22)((((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(([\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x7f]|\\x21|[\\x23-\\x5b]|[\\x5d-\\x7e]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(\\\\([\\x01-\\x09\\x0b\\x0c\\x0d-\\x7f]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF]))))*(((\\x20|\\x09)*(\\x0d\\x0a))?(\\x20|\\x09)+)?(\\x22)))@((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.?$",
        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled );

    public static Regex PhoneRegex { get; set; } = new(
        "^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$",
        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled );

    public static Regex UrlRegex { get; set; } = new(
        "^(https?|ftp):\\/\\/(((([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:)*@)?(((\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d|[1-9]\\d|1\\d\\d|2[0-4]\\d|25[0-5]))|((([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|\\d|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.)+(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])*([a-z]|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])))\\.?)(:\\d*)?)(\\/((([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)+(\\/(([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)*)*)?)?(\\?((([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|[\\uE000-\\uF8FF]|\\/|\\?)*)?(\\#((([a-z]|\\d|-|\\.|_|~|[\\u00A0-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFEF])|(%[\\da-f]{2})|[!\\$&'\\(\\)\\*\\+,;=]|:|@)|\\/|\\?)*)?$",
        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled );

    public static Regex GetRegex( string pattern, RegexOptions options )
        => _cachedRegexes.GetOrAdd( (pattern, options), x => new Regex( x.Pattern, x.Options ) );
}