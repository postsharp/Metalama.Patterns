// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of LessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.Tests;

public class LessOrGreaterThanZeroTests : RangeContractTestsBase
{
    private const long longLimit = 0;
    private const ulong ulongLimit = 0;
    private const double doubleLimit = 0;

    private static readonly double doubleStep = double.Epsilon;
    private static readonly decimal decimalStep = DecimalTolerance;

    [Fact]
    public void TestMethodsWithGreaterThanAspect_Success()
    {
        TestMethodsWithGreaterThanAspect( 0, 0, 0, 0 );
        TestMethodsWithGreaterThanAspect( 100, 100, 100, 100 );
        TestMethodsWithGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    [Fact]
    public void TestMethodsWithGreaterThanAspect_Failure()
    {
        AssertFails( TestMethodsWithGreaterThanAspect,
            longLimit - 1,
            null,
            doubleLimit - doubleStep,
            (decimal) doubleLimit - decimalStep );
        AssertFails( TestMethodsWithGreaterThanAspect, -100, null, -100, -100 );
        AssertFails( TestMethodsWithGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithLessThanAspect_Success()
    {
        TestMethodsWithLessThanAspect( 0, 0, 0, 0 );
        TestMethodsWithLessThanAspect( -100, null, -100, -100 );
        TestMethodsWithLessThanAspect( long.MinValue, null, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithLessThanAspect_Failure()
    {
        AssertFails( TestMethodsWithLessThanAspect,
            longLimit + 1,
            1,
            doubleLimit + doubleStep,
            (decimal) doubleLimit + decimalStep );
        AssertFails( TestMethodsWithLessThanAspect, 100, 100, 100, 100 );
        AssertFails( TestMethodsWithLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    private static void TestMethodsWithGreaterThanAspect( long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongGreaterThanLong( longValue );
        MethodWithUlongGreaterThanLong( ulongValue );
        MethodWithDoubleGreaterThanLong( doubleValue );
        MethodWithDecimalGreaterThanLong( decimalValue );

        MethodWithLongGreaterThanUlong( longValue );
        MethodWithUlongGreaterThanUlong( ulongValue );
        MethodWithDoubleGreaterThanUlong( doubleValue );
        MethodWithDecimalGreaterThanUlong( decimalValue );

        MethodWithLongGreaterThanDouble( longValue );
        MethodWithUlongGreaterThanDouble( ulongValue );
        MethodWithDoubleGreaterThanDouble( doubleValue );
        MethodWithDecimalGreaterThanDouble( decimalValue );
    }

    private static void TestMethodsWithLessThanAspect( long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongLessThanLong( longValue );
        MethodWithUlongLessThanLong( ulongValue );
        MethodWithDoubleLessThanLong( doubleValue );
        MethodWithDecimalLessThanLong( decimalValue );

        MethodWithLongLessThanUlong( longValue );
        MethodWithUlongLessThanUlong( ulongValue );
        MethodWithDoubleLessThanUlong( doubleValue );
        MethodWithDecimalLessThanUlong( decimalValue );

        MethodWithLongLessThanDouble( longValue );
        MethodWithUlongLessThanDouble( ulongValue );
        MethodWithDoubleLessThanDouble( doubleValue );
        MethodWithDecimalLessThanDouble( decimalValue );
    }

    #region Long

    private static void MethodWithLongGreaterThanLong( [GreaterThan( longLimit )] long? a )
    {
    }

    private static void MethodWithUlongGreaterThanLong( [GreaterThan( longLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleGreaterThanLong( [GreaterThan( longLimit )] double? a )
    {
    }

    private static void MethodWithDecimalGreaterThanLong( [GreaterThan( longLimit )] decimal? a )
    {
    }

    private static void MethodWithLongLessThanLong( [LessThan( longLimit )] long? a )
    {
    }

    private static void MethodWithUlongLessThanLong( [LessThan( longLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleLessThanLong( [LessThan( longLimit )] double? a )
    {
    }

    private static void MethodWithDecimalLessThanLong( [LessThan( longLimit )] decimal? a )
    {
    }

    #endregion Long

    #region Ulong

    private static void MethodWithLongGreaterThanUlong( [GreaterThan( ulongLimit )] long? a )
    {
    }

    private static void MethodWithUlongGreaterThanUlong( [GreaterThan( ulongLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleGreaterThanUlong( [GreaterThan( ulongLimit )] double? a )
    {
    }

    private static void MethodWithDecimalGreaterThanUlong( [GreaterThan( ulongLimit )] decimal? a )
    {
    }

    private static void MethodWithLongLessThanUlong( [LessThan( ulongLimit )] long? a )
    {
    }

    private static void MethodWithUlongLessThanUlong( [LessThan( ulongLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleLessThanUlong( [LessThan( ulongLimit )] double? a )
    {
    }

    private static void MethodWithDecimalLessThanUlong( [LessThan( ulongLimit )] decimal? a )
    {
    }

    #endregion Ulong

    #region Double

    private static void MethodWithLongGreaterThanDouble( [GreaterThan( doubleLimit )] long? a )
    {
    }

    private static void MethodWithUlongGreaterThanDouble( [GreaterThan( doubleLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleGreaterThanDouble( [GreaterThan( doubleLimit )] double? a )
    {
    }

    private static void MethodWithDecimalGreaterThanDouble( [GreaterThan( doubleLimit )] decimal? a )
    {
    }

    private static void MethodWithLongLessThanDouble( [LessThan( doubleLimit )] long? a )
    {
    }

    private static void MethodWithUlongLessThanDouble( [LessThan( doubleLimit )] ulong? a )
    {
    }

    private static void MethodWithDoubleLessThanDouble( [LessThan( doubleLimit )] double? a )
    {
    }

    private static void MethodWithDecimalLessThanDouble( [LessThan( doubleLimit )] decimal? a )
    {
    }

    #endregion Double
}