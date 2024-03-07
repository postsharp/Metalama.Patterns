// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

// This is a modified copy of StrictlyLessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace Metalama.Patterns.Contracts.UnitTests;

public sealed class StrictlyLessOrGreaterThanNegativeValueTests : RangeContractTestsBase
{
    private const long _longMin = -100;
    private const double _doubleMin = -100;

    private static readonly double _doubleStep = FloatingPointHelper.GetDoubleStep( _doubleMin );
    private static readonly decimal _decimalStep = FloatingPointHelper.GetDecimalStep( (decimal) _doubleMin );

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Success()
    {
        TestMethodsWithStrictlyGreaterThanAspect(
            _longMin + 1,
            null,
            _doubleMin + _doubleStep,
            (decimal) _doubleMin + _decimalStep );

        TestMethodsWithStrictlyGreaterThanAspect( _longMin / 2, null, _doubleMin / 2, (decimal) _doubleMin / 2 );
        TestMethodsWithStrictlyGreaterThanAspect( 0, 0, 0, 0 );

        TestMethodsWithStrictlyGreaterThanAspect(
            _longMin * -2,
            (ulong) Math.Abs( _longMin ) * 2,
            _doubleMin * -2,
            (decimal) _doubleMin * -2 );

        TestMethodsWithStrictlyGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyGreaterThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, _longMin, null, _doubleMin, (decimal) _doubleMin );

        AssertFails(
            TestMethodsWithStrictlyGreaterThanAspect,
            _longMin * 2,
            null,
            _doubleMin * 2,
            (decimal) _doubleMin * 2 );

        AssertFails( TestMethodsWithStrictlyGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Success()
    {
        TestMethodsWithStrictlyLessThanAspect(
            _longMin - 1,
            null,
            _doubleMin - _doubleStep,
            (decimal) _doubleMin - _decimalStep );

        TestMethodsWithStrictlyLessThanAspect( _longMin * 2, null, _doubleMin * 2, (decimal) _doubleMin * 2 );
        TestMethodsWithStrictlyLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
    }

    [Fact]
    public void TestMethodsWithStrictlyLessThanAspect_Failure()
    {
        AssertFails( TestMethodsWithStrictlyLessThanAspect, _longMin, null, _doubleMin, (decimal) _doubleMin );

        AssertFails(
            TestMethodsWithStrictlyLessThanAspect,
            _longMin / 2,
            null,
            _doubleMin / 2,
            (decimal) _doubleMin / 2 );

        AssertFails( TestMethodsWithStrictlyLessThanAspect, 0, 0, 0, 0 );

        AssertFails(
            TestMethodsWithStrictlyLessThanAspect,
            _longMin * -2,
            (ulong) Math.Abs( _longMin ) * 2,
            _doubleMin * -2,
            (decimal) _doubleMin * -2 );

        AssertFails(
            TestMethodsWithStrictlyLessThanAspect,
            long.MaxValue,
            ulong.MaxValue,
            double.MaxValue,
            decimal.MaxValue );
    }

    private static void TestMethodsWithStrictlyGreaterThanAspect(
        long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
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

    private static void TestMethodsWithStrictlyLessThanAspect(
        long? longValue,
        ulong? ulongValue,
        double? doubleValue,
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

    private static void MethodWithLongStrictlyGreaterThanLong( [StrictlyGreaterThan( _longMin )] long? a ) { }

    private static void MethodWithUlongStrictlyGreaterThanLong( [StrictlyGreaterThan( _longMin )] ulong? a ) { }

    private static void MethodWithDoubleStrictlyGreaterThanLong( [StrictlyGreaterThan( _longMin )] double? a ) { }

    private static void MethodWithDecimalStrictlyGreaterThanLong( [StrictlyGreaterThan( _longMin )] decimal? a ) { }

    private static void MethodWithLongStrictlyLessThanLong( [StrictlyLessThan( _longMin )] long? a ) { }

    private static void MethodWithUlongStrictlyLessThanLong( [StrictlyLessThan( 100 )] ulong? a ) { }

    private static void MethodWithDoubleStrictlyLessThanLong( [StrictlyLessThan( _longMin )] double? a ) { }

    private static void MethodWithDecimalStrictlyLessThanLong( [StrictlyLessThan( _longMin )] decimal? a ) { }

    #endregion Long

    #region Double

    private static void MethodWithLongStrictlyGreaterThanDouble( [StrictlyGreaterThan( _doubleMin )] long? a ) { }

    private static void MethodWithUlongStrictlyGreaterThanDouble( [StrictlyGreaterThan( _doubleMin )] ulong? a ) { }

    private static void MethodWithDoubleStrictlyGreaterThanDouble( [StrictlyGreaterThan( _doubleMin )] double? a ) { }

    private static void MethodWithDecimalStrictlyGreaterThanDouble( [StrictlyGreaterThan( _doubleMin )] decimal? a ) { }

    private static void MethodWithLongStrictlyLessThanDouble( [StrictlyLessThan( -100 )] long? a ) { }

    private static void MethodWithDoubleStrictlyLessThanDouble( [StrictlyLessThan( _doubleMin )] double? a ) { }

    private static void MethodWithDecimalStrictlyLessThanDouble( [StrictlyLessThan( _doubleMin )] decimal? a ) { }

    #endregion Double
}