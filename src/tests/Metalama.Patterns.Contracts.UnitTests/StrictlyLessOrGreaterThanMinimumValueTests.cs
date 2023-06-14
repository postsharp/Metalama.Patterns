// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of StrictlyLessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.Tests;

public class StrictlyLessOrGreaterThanMinimumValueTests : RangeContractTestsBase
{
    private const long longLimit = long.MinValue + 1;
    private const double doubleLimit = double.MinValue / (1 + DoubleTolerance);

    // This has to be double because decimal is not allowed as attribute constructor value.
    // Loss of precision is a consequence.
    private const double decimalLimit = (double) decimal.MinValue / (1 + DoubleTolerance);

    private const double doubleStep = doubleLimit - double.MinValue;
    private const decimal decimalStep = (decimal) decimalLimit - decimal.MinValue;

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Success()
    {
        MethodWithLongStrictlyGreaterThanLong( longLimit + 1 );
        MethodWithDoubleStrictlyGreaterThanLong( (double) longLimit + doubleStep );
        MethodWithDecimalStrictlyGreaterThanLong( (decimal) longLimit + decimalStep );

        MethodWithLongStrictlyGreaterThanDouble( long.MinValue );
        MethodWithDoubleStrictlyGreaterThanDouble( doubleLimit + doubleStep );
        MethodWithDecimalStrictlyGreaterThanDouble( (decimal) decimalLimit + decimalStep );

        TestMethodsWithStrictlyGreaterThanAspect( 0, 0, 0, 0 );
        TestMethodsWithStrictlyGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit, null, doubleLimit, (decimal) decimalLimit );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Success() =>
        TestMethodsWithStrictlyLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit, null, doubleLimit, (decimal) decimalLimit );
        AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit / 2, null, doubleLimit / 2,
            (decimal) decimalLimit / 2 );
        AssertFails( TestMethodsWithStrictlyLessThanAspect, 0, 0, 0, 0 );
        AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit / -2, null, doubleLimit / -2,
            (decimal) decimalLimit / -2 );
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

        MethodWithLongStrictlyLessThanDouble( longValue );
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

    #region Double

    private static void MethodWithLongStrictlyGreaterThanDouble( [StrictlyGreaterThan( doubleLimit )] long? a )
    {
    }

    private static void MethodWithUlongStrictlyGreaterThanDouble( [StrictlyGreaterThan( doubleLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleStrictlyGreaterThanDouble( [StrictlyGreaterThan( doubleLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyGreaterThanDouble( [StrictlyGreaterThan( doubleLimit )] decimal? a )
    {
    }

    // Cannot use doubleLimit by design. Covered by build test.
    private static void MethodWithLongStrictlyLessThanDouble( [StrictlyLessThan( (double) longLimit )] long? a )
    {
    }

    private static void MethodWithDoubleStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] double? a )
    {
    }

    // Cannot use doubleLimit by design. Covered by build test.
    private static void MethodWithDecimalStrictlyLessThanDouble(
        [StrictlyLessThan( (double) decimalLimit )] decimal? a )
    {
    }

    #endregion Double
}