// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;

namespace Metalama.Patterns.Contracts.UnitTests;

public sealed class StrictRangeAttributeTests
{
    private const long _longMin = 100;
    private const long _longMax = 200;

    private const ulong _ulongMin = 100;
    private const ulong _ulongMax = 200;

    private const double _doubleMin = 100;
    private const double _doubleMax = 200;

    private const decimal _decimalMin = 100;
    private const decimal _decimalMax = 200;

    [Fact]
    public void TestValuesInsideRange() => TestAllMethods( 150, 150, 150, 150 );

    [Fact]
    public void TestValuesOutsideRange()
    {
        AssertEx.Throws<ArgumentOutOfRangeException>(
            () =>
                TestAllMethods( _longMin - 1, _ulongMin - 1, _doubleMin - 1, _decimalMin - 1 ) );

        AssertEx.Throws<ArgumentOutOfRangeException>(
            () =>
                TestAllMethods( _longMax + 1, _ulongMax + 1, _doubleMax + 1, _decimalMax + 1 ) );
    }

    [Fact]
    public void TestValuesOnEdges()
    {
        TestAllMethods( _longMin, _ulongMin, _doubleMin, _decimalMin );
        TestAllMethods( _longMax, _ulongMax, _doubleMax, _decimalMax );
    }

    [Fact]
    public void TestDoubleTolerance()
    {
        MethodWithDoubleInDoubleStrictRange( _doubleMin + FloatingPointHelper.GetDoubleStep( _doubleMin ) );
        MethodWithDoubleInDoubleStrictRange( _doubleMax - FloatingPointHelper.GetDoubleStep( _doubleMax ) );
    }

    private static void TestAllMethods( long longValue, ulong ulongValue, double doubleValue, decimal decimalValue )
    {
        MethodWithLongInLongStrictRange( longValue );
        MethodWithUlongInLongStrictRange( ulongValue );
        MethodWithDoubleInLongStrictRange( doubleValue );
        MethodWithDecimalInLongStrictRange( decimalValue );

        MethodWithLongInULongStrictRange( longValue );
        MethodWithUlongInULongStrictRange( ulongValue );
        MethodWithDoubleInULongStrictRange( doubleValue );
        MethodWithDecimalInULongStrictRange( decimalValue );

        MethodWithLongInDoubleStrictRange( longValue );
        MethodWithUlongInDoubleStrictRange( ulongValue );
        MethodWithDoubleInDoubleStrictRange( doubleValue );
        MethodWithDecimalInDoubleStrictRange( decimalValue );
    }

    #region long

    private static void MethodWithLongInLongStrictRange( [StrictRange( _longMin, _longMax )] long? value ) { }

    private static void MethodWithUlongInLongStrictRange( [StrictRange( _longMin, _longMax )] ulong? a ) { }

    private static void MethodWithDoubleInLongStrictRange( [StrictRange( _longMin, _longMax )] double? a ) { }

    private static void MethodWithDecimalInLongStrictRange( [StrictRange( _longMin, _longMax )] decimal? a ) { }

    #endregion

    #region ulong

    private static void MethodWithLongInULongStrictRange( [StrictRange( _ulongMin, _ulongMax )] long? value ) { }

    private static void MethodWithUlongInULongStrictRange( [StrictRange( _ulongMin, _ulongMax )] ulong? a ) { }

    private static void MethodWithDoubleInULongStrictRange( [StrictRange( _ulongMin, _ulongMax )] double? a ) { }

    private static void MethodWithDecimalInULongStrictRange( [StrictRange( _ulongMin, _ulongMax )] decimal? a ) { }

    #endregion

    #region double

    private static void MethodWithLongInDoubleStrictRange( [StrictRange( _doubleMin, _doubleMax )] long? value ) { }

    private static void MethodWithUlongInDoubleStrictRange( [StrictRange( _doubleMin, _doubleMax )] ulong? a ) { }

    private static void MethodWithDoubleInDoubleStrictRange( [StrictRange( _doubleMin, _doubleMax )] double? a ) { }

    private static void MethodWithDecimalInDoubleStrictRange( [StrictRange( _doubleMin, _doubleMax )] decimal? a ) { }

    #endregion
}