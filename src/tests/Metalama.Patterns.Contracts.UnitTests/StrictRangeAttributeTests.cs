// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;


public class StrictRangeAttributeTests
{
    private const long longMin = 100;
    private const long longMax = 200;

    private const ulong ulongMin = 100;
    private const ulong ulongMax = 200;

    private const double doubleMin = 100;
    private const double doubleMax = 200;

    private const decimal decimalMin = 100;
    private const decimal decimalMax = 200;

    [Fact]
    public void TestValuesInsideRange()
    {
        TestAllMethods(150, 150, 150, 150);
    }

    [Fact]
    public void TestValuesOutsideRange()
    {
        AssertEx.Throws<ArgumentOutOfRangeException>( () => TestAllMethods( longMin - 1, ulongMin - 1, doubleMin - 1, decimalMin - 1 ) );
        AssertEx.Throws<ArgumentOutOfRangeException>( () => TestAllMethods( longMax + 1, ulongMax + 1, doubleMax + 1, decimalMax + 1 ) );
    }

    [Fact]
    public void TestValuesOnEdges()
    {
        AssertEx.Throws<ArgumentOutOfRangeException>( () => TestAllMethods( longMin, ulongMin, doubleMin, decimalMin ) );
        AssertEx.Throws<ArgumentOutOfRangeException>( () => TestAllMethods( longMax, ulongMax, doubleMax, decimalMax ) );
    }

    [Fact]
    public void TestDoubleTolerance()
    {
        MethodWithDoubleInDoubleStrictRange( doubleMin + FloatingPointHelper.GetDoubleStep( doubleMin ) );
        MethodWithDoubleInDoubleStrictRange( doubleMax - FloatingPointHelper.GetDoubleStep( doubleMax ) );
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

    private static void MethodWithLongInLongStrictRange( [StrictRange( longMin, longMax )] long? value )
    {
    }

    private static void MethodWithUlongInLongStrictRange( [StrictRange( longMin, longMax )] ulong? a )
    {
    }

    private static void MethodWithDoubleInLongStrictRange( [StrictRange( longMin, longMax )] double? a )
    {
    }

    private static void MethodWithDecimalInLongStrictRange( [StrictRange( longMin, longMax )] decimal? a )
    {
    }

    #endregion

    #region ulong

    private static void MethodWithLongInULongStrictRange( [StrictRange( ulongMin, ulongMax )] long? value )
    {
    }

    private static void MethodWithUlongInULongStrictRange( [StrictRange( ulongMin, ulongMax )] ulong? a )
    {
    }

    private static void MethodWithDoubleInULongStrictRange( [StrictRange( ulongMin, ulongMax )] double? a )
    {
    }

    private static void MethodWithDecimalInULongStrictRange( [StrictRange( ulongMin, ulongMax )] decimal? a )
    {
    }

    #endregion

    #region double

    private static void MethodWithLongInDoubleStrictRange( [StrictRange( doubleMin, doubleMax )] long? value )
    {
    }

    private static void MethodWithUlongInDoubleStrictRange( [StrictRange( doubleMin, doubleMax )] ulong? a )
    {
    }

    private static void MethodWithDoubleInDoubleStrictRange( [StrictRange( doubleMin, doubleMax )] double? a )
    {
    }

    private static void MethodWithDecimalInDoubleStrictRange( [StrictRange( doubleMin, doubleMax )] decimal? a )
    {
    }

    #endregion
}
