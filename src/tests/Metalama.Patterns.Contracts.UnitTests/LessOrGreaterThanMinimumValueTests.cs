// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of LessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.Tests;

public class LessOrGreaterThanMinimumValueTests : RangeContractTestsBase
{
    private const long longLimit = long.MinValue;
    private const double doubleLimit = double.MinValue;

    // This has to be double because decimal is not allowed as attribute constructor value.
    // Loss of precision is a consequence.
    private const double decimalLimit = (double) decimal.MinValue / (1 + DoubleTolerance);

    private static readonly double doubleStep = FloatingPointHelper.GetDoubleStep( doubleLimit );
    private static readonly decimal decimalStep = Math.Abs( decimal.MinValue ) * DecimalTolerance;

    [Fact]
    public void TestMethodsWithGreaterThanAspect_Success()
    {
        MethodWithLongGreaterThanLong( long.MinValue );
        MethodWithDoubleGreaterThanLong( long.MinValue );
        MethodWithDecimalGreaterThanLong( long.MinValue );

        MethodWithLongGreaterThanDouble( long.MinValue );
        MethodWithDoubleGreaterThanDouble( double.MinValue );
        MethodWithDecimalGreaterThanDouble( (decimal) decimalLimit );

        TestMethodsWithGreaterThanAspect( 0, 0, 0, 0 );
        TestMethodsWithGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    [Fact]
    public void TestMethodsWithGreaterThanAspect_Failure()
    {
        AssertFails( MethodWithDoubleGreaterThanLong, (double) long.MinValue - doubleStep );
        AssertFails( MethodWithDoubleGreaterThanLong, double.MinValue );

        AssertFails( MethodWithDecimalGreaterThanLong, (decimal) long.MinValue - decimalStep );
        AssertFails( MethodWithDecimalGreaterThanLong, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithLessThanAspect_Success()
    {
        MethodWithLongGreaterThanLong( long.MinValue );
        MethodWithDoubleGreaterThanLong( long.MinValue );
        MethodWithDecimalGreaterThanLong( long.MinValue );

        MethodWithLongGreaterThanDouble( long.MinValue );
        MethodWithDoubleGreaterThanDouble( double.MinValue );
        MethodWithDecimalGreaterThanDouble( (decimal) decimalLimit );
    }

    [Fact]
    public void TestMethodsWithLessThanAspect_Failure()
    {
        AssertFails( MethodWithLongLessThanLong, long.MinValue + 1 );
        AssertFails( MethodWithDoubleLessThanLong, (double) long.MinValue + doubleStep );
        AssertFails( MethodWithDecimalLessThanLong, (decimal) long.MinValue + 1 );

        AssertFails( MethodWithLongLessThanDouble, long.MinValue + 1 );
        AssertFails( MethodWithDoubleLessThanDouble, double.MinValue + doubleStep );
        AssertFails( MethodWithDecimalLessThanDouble, (decimal) decimalLimit + decimalStep );

        AssertFails( TestMethodsWithLessThanAspect, 0, 0, 0, 0 );
        AssertFails( TestMethodsWithLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    private static void TestMethodsWithGreaterThanAspect( long? longValue, ulong? ulongValue, double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongGreaterThanLong( longValue );
        MethodWithUlongGreaterThanLong( ulongValue );
        MethodWithDoubleGreaterThanLong( doubleValue );
        MethodWithDecimalGreaterThanLong( decimalValue );

        MethodWithLongGreaterThanDouble( longValue );
        MethodWithUlongGreaterThanDouble( ulongValue );
        MethodWithDoubleGreaterThanDouble( doubleValue );
        MethodWithDecimalGreaterThanDouble( decimalValue );
    }

    private static void TestMethodsWithLessThanAspect( long? longValue, ulong? ulongValue, double? doubleValue,
        decimal? decimalValue )
    {
        MethodWithLongLessThanLong( longValue );
        MethodWithUlongLessThanLong( ulongValue );
        MethodWithDoubleLessThanLong( doubleValue );
        MethodWithDecimalLessThanLong( decimalValue );

        MethodWithLongLessThanDouble( longValue );
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

    private static void MethodWithLongLessThanDouble( [LessThan( (double) longLimit )] long? a )
    {
    }

    private static void MethodWithDoubleLessThanDouble( [LessThan( doubleLimit )] double? a )
    {
    }

    private static void MethodWithDecimalLessThanDouble( [LessThan( decimalLimit )] decimal? a )
    {
    }

    #endregion Double
}