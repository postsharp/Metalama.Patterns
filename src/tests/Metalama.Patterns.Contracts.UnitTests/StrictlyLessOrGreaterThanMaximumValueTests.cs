// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of StrictlyLessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.Tests;

public class StrictlyLessOrGreaterThanMaximumValueTests : RangeContractTestsBase
{
    private const long longLimit = long.MaxValue - 1;
    private const ulong ulongLimit = ulong.MaxValue - 1;
    private const double doubleLimit = double.MaxValue / (1 + DoubleTolerance);

    // This has to be double because decimal is not allowed as attribute constructor value.
    // Loss of precision is a consequence.
    private const double decimalLimit = (double) decimal.MaxValue / (1 + DoubleTolerance);

    private const double doubleStep = double.MaxValue - doubleLimit;
    private const decimal decimalStep = decimal.MaxValue - (decimal) decimalLimit;

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Success() =>
        TestMethodsWithStrictlyGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit, ulongLimit, doubleLimit,
            (decimal) decimalLimit );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit / 2, ulongLimit / 2, doubleLimit / 2,
            (decimal) decimalLimit / 2 );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, 0, 0, 0, 0 );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit / -2, null, doubleLimit / -2,
            (decimal) decimalLimit / -2 );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, long.MinValue, ulong.MinValue, double.MinValue,
            decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Success()
    {
        MethodWithLongStrictlyLessThanLong( longLimit - 1 );
        MethodWithUlongStrictlyLessThanLong( (ulong) longLimit - 1 );
        MethodWithDoubleStrictlyLessThanLong( (double) longLimit - doubleStep );
        MethodWithDecimalStrictlyLessThanLong( (decimal) longLimit - decimalStep );

        MethodWithLongStrictlyLessThanUlong( long.MaxValue );
        MethodWithUlongStrictlyLessThanUlong( ulongLimit - 1 );
        MethodWithDoubleStrictlyLessThanUlong( (double) ulongLimit - doubleStep );
        MethodWithDecimalStrictlyLessThanUlong( (decimal) ulongLimit - decimalStep );

        MethodWithLongStrictlyLessThanDouble( long.MaxValue );
        MethodWithUlongStrictlyLessThanDouble( ulong.MaxValue );
        MethodWithDoubleStrictlyLessThanDouble( doubleLimit - doubleStep );
        MethodWithDecimalStrictlyLessThanDouble( (decimal) decimalLimit - decimalStep );

        TestMethodsWithStrictlyLessThanAspect( 0, 0, 0, 0 );
        TestMethodsWithStrictlyLessThanAspect( longLimit / -2, null, doubleLimit / -2, (decimal) decimalLimit / -2 );
        TestMethodsWithStrictlyLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit, ulongLimit, doubleLimit,
            (decimal) decimalLimit );
        AssertFails( TestMethodsWithStrictlyLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue,
            decimal.MaxValue );
    }

    private static void TestMethodsWithStrictlyGreaterThanAspect( long? longValue, ulong? ulongValue,
        double? doubleValue, decimal? decimalValue )
    {
        MethodWithLongStrictlyGreaterThanLong( longValue );
        MethodWithUlongStrictlyGreaterThanLong( ulongValue );
        MethodWithDoubleStrictlyGreaterThanLong( doubleValue );
        MethodWithDecimalStrictlyGreaterThanLong( decimalValue );

        MethodWithLongStrictlyGreaterThanUlong( longValue );
        MethodWithUlongStrictlyGreaterThanUlong( ulongValue );
        MethodWithDoubleStrictlyGreaterThanUlong( doubleValue );
        MethodWithDecimalStrictlyGreaterThanUlong( decimalValue );

        MethodWithLongStrictlyGreaterThanDouble( longValue );
        MethodWithUlongStrictlyGreaterThanDouble( ulongValue );
        MethodWithDoubleStrictlyGreaterThanDouble( doubleValue );
        MethodWithDecimalStrictlyGreaterThanDouble( decimalValue );
    }

    private static void TestMethodsWithStrictlyLessThanAspect( long? longValue, ulong? ulongValue, double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongStrictlyLessThanLong( longValue );
        MethodWithUlongStrictlyLessThanLong( ulongValue );
        MethodWithDoubleStrictlyLessThanLong( doubleValue );
        MethodWithDecimalStrictlyLessThanLong( decimalValue );

        MethodWithLongStrictlyLessThanUlong( longValue );
        MethodWithUlongStrictlyLessThanUlong( ulongValue );
        MethodWithDoubleStrictlyLessThanUlong( doubleValue );
        MethodWithDecimalStrictlyLessThanUlong( decimalValue );

        MethodWithLongStrictlyLessThanDouble( longValue );
        MethodWithUlongStrictlyLessThanDouble( ulongValue );
        MethodWithDoubleStrictlyLessThanDouble( doubleValue );
        MethodWithDecimalStrictlyLessThanDouble( decimalValue );
    }

    #region Long

    private static void MethodWithLongStrictlyGreaterThanLong( [StrictlyGreaterThan( longLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyGreaterThanLong( [StrictlyGreaterThan( longLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyGreaterThanLong( [StrictlyGreaterThan( longLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyGreaterThanLong( [StrictlyGreaterThan( longLimit )] decimal? a )
    {
    }

    private static void MethodWithLongStrictlyLessThanLong( [StrictlyLessThan( longLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyLessThanLong( [StrictlyLessThan( longLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyLessThanLong( [StrictlyLessThan( longLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyLessThanLong( [StrictlyLessThan( longLimit )] decimal? a )
    {
    }

    #endregion Long

    #region Ulong

    // Cannot use ulongLimit by design. Covered by build test.
    private static void MethodWithLongStrictlyGreaterThanUlong( [StrictlyGreaterThan( longLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyGreaterThanUlong( [StrictlyGreaterThan( ulongLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyGreaterThanUlong( [StrictlyGreaterThan( ulongLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyGreaterThanUlong( [StrictlyGreaterThan( ulongLimit )] decimal? a )
    {
    }

    private static void MethodWithLongStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] decimal? a )
    {
    }

    #endregion Ulong

    #region Double

    // Cannot use doubleLimit by design. Covered by build test.
    private static void MethodWithLongStrictlyGreaterThanDouble( [StrictlyGreaterThan( (double) longLimit )] long? a )
    {
    }

    // Cannot use doubleLimit by design. Covered by build test.
    private static void MethodWithUlongStrictlyGreaterThanDouble(
        [StrictlyGreaterThan( (double) ulongLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyGreaterThanDouble( [StrictlyGreaterThan( doubleLimit )] double? a )
    {
    }

    // Cannot use doubleLimit by design. Covered by build test.
    private static void MethodWithDecimalStrictlyGreaterThanDouble(
        [StrictlyGreaterThan( (double) decimalLimit )] decimal? a )
    {
    }

    private static void MethodWithLongStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] decimal? a )
    {
    }

    #endregion Double
}