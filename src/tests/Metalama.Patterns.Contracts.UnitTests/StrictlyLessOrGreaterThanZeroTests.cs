// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of StrictlyLessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.Tests;

public class StrictlyLessOrGreaterThanZeroTests : RangeContractTestsBase
{
    private const long longLimit = 0;
    private const ulong ulongLimit = 0;
    private const double doubleLimit = 0;

    private static readonly double doubleStep = double.Epsilon;
    private static readonly decimal decimalStep = DecimalTolerance;

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Success()
    {
        TestMethodsWithStrictlyGreaterThanAspect( longLimit + 1,
            ulongLimit + 1,
            doubleLimit + doubleStep,
            (decimal) doubleLimit + decimalStep );
        TestMethodsWithStrictlyGreaterThanAspect( 100, 100, 100, 100 );
        TestMethodsWithStrictlyGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, 0, 0, 0, 0 );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, -100, null, -100, -100 );
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Success()
    {
        TestMethodsWithStrictlyLessThanAspect( longLimit - 1,
            null,
            doubleLimit - doubleStep,
            (decimal) doubleLimit - decimalStep );
        TestMethodsWithStrictlyLessThanAspect( -100, null, -100, -100 );
        TestMethodsWithStrictlyLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyLessThanAspect, 0, 0, 0, 0 );
        AssertFails( TestMethodsWithStrictlyLessThanAspect, 100, 100, 100, 100 );
        AssertFails( TestMethodsWithStrictlyLessThanAspect,
            long.MaxValue,
            ulong.MaxValue,
            double.MaxValue,
            decimal.MaxValue );
    }

    private static void TestMethodsWithStrictlyGreaterThanAspect( long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
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

    private static void TestMethodsWithStrictlyLessThanAspect( long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongStrictlyLessThanLong( longValue );
        MethodWithUlongStrictlyLessThanLong( ulongValue );
        MethodWithDoubleStrictlyLessThanLong( doubleValue );
        MethodWithDecimalStrictlyLessThanLong( decimalValue );

        MethodWithLongStrictlyLessThanUlong( longValue );
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

    private static void MethodWithLongStrictlyGreaterThanUlong( [StrictlyGreaterThan( ulongLimit )] long? a )
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

    private static void MethodWithDoubleStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] double? a )
    {
    }

    private static void MethodWithDecimalStrictlyLessThanUlong( [StrictlyLessThan( ulongLimit )] decimal? a )
    {
    }

    #endregion Ulong

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