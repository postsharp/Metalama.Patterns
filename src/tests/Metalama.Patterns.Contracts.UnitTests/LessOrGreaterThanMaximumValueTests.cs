using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Patterns.Contracts;


// This is a modified copy of LessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace PostSharp.Patterns.Contracts.Tests
{
    public class LessOrGreaterThanMaximumValueTests : RangeContractTestsBase
    {
        private const long longLimit = long.MaxValue;
        private const ulong ulongLimit = ulong.MaxValue;
        private const double doubleLimit = double.MaxValue;

        // This has to be double because decimal is not allowed as attribute constructor value.
        // Loss of precision is a consequence.
        private const double decimalLimit = (double) decimal.MaxValue/(1 + DoubleTolerance);

        private static readonly double doubleStep = FloatingPointHelper.GetDoubleStep( doubleLimit );
        private static readonly decimal decimalStep = decimal.MaxValue*DecimalTolerance;

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Success()
        {
            TestMethodsWithGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Failure()
        {
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit - 1, ulongLimit - 1, doubleLimit - doubleStep, (decimal) decimalLimit - decimalStep );
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit/2, ulongLimit/2, doubleLimit/2, (decimal) decimalLimit/2 );
            AssertFails( TestMethodsWithGreaterThanAspect, 0, 0, 0, 0 );
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit/-2, null, doubleLimit/-2, (decimal) decimalLimit/-2 );
            AssertFails( TestMethodsWithGreaterThanAspect, long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Success()
        {
            MethodWithLongLessThanLong( long.MaxValue );
            MethodWithUlongLessThanLong( long.MaxValue );
            MethodWithDoubleLessThanLong( long.MaxValue );
            MethodWithDecimalLessThanLong( long.MaxValue );

            MethodWithLongLessThanUlong( long.MaxValue );
            MethodWithUlongLessThanUlong( ulong.MaxValue );
            MethodWithDoubleLessThanUlong( ulong.MaxValue );
            MethodWithDecimalLessThanUlong( ulong.MaxValue );

            MethodWithLongLessThanDouble( long.MaxValue );
            MethodWithUlongLessThanDouble( ulong.MaxValue );
            MethodWithDoubleLessThanDouble( double.MaxValue );
            MethodWithDecimalLessThanDouble( (decimal) decimalLimit );

            TestMethodsWithLessThanAspect( 0, 0, 0, 0 );
            TestMethodsWithLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Failure()
        {
            AssertFails( MethodWithUlongLessThanLong, (ulong) long.MaxValue + 1 );
            AssertFails( MethodWithUlongLessThanLong, ulong.MaxValue );

            AssertFails( MethodWithDoubleLessThanLong, (double) long.MaxValue + doubleStep );
            AssertFails( MethodWithDoubleLessThanLong, double.MaxValue );

            AssertFails( MethodWithDecimalLessThanLong, (decimal) long.MaxValue + decimalStep );
            AssertFails( MethodWithDecimalLessThanLong, decimal.MaxValue );

            AssertFails( MethodWithDoubleLessThanUlong, (double) ulong.MaxValue + doubleStep );
            AssertFails( MethodWithDoubleLessThanUlong, double.MaxValue );

            AssertFails( MethodWithDecimalLessThanUlong, (decimal) ulong.MaxValue + decimalStep );
            AssertFails( MethodWithDecimalLessThanUlong, decimal.MaxValue );
        }

        private static void TestMethodsWithGreaterThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
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

        private static void TestMethodsWithLessThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
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

        // Cannot use ulongLimit by design. Covered by build test.
        private static void MethodWithLongGreaterThanUlong( [GreaterThan( longLimit )] long? a )
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

        // Cannot use doubleLimit by design. Covered by build test.
        private static void MethodWithLongGreaterThanDouble( [GreaterThan( (double) longLimit )] long? a )
        {
        }

        // Cannot use doubleLimit by design. Covered by build test.
        private static void MethodWithUlongGreaterThanDouble( [GreaterThan( (double) ulongLimit )] ulong? a )
        {
        }

        private static void MethodWithDoubleGreaterThanDouble( [GreaterThan( doubleLimit )] double? a )
        {
        }

        // Cannot use doubleLimit by design. Covered by build test.
        private static void MethodWithDecimalGreaterThanDouble( [GreaterThan( (double) decimalLimit )] decimal? a )
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
}
