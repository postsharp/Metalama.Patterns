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
    public class LessOrGreaterThanNegativeValueTests : RangeContractTestsBase
    {
        private const long longLimit = -100;
        private const double doubleLimit = -100;

        private static readonly double doubleStep = FloatingPointHelper.GetDoubleStep( doubleLimit );
        private static readonly decimal decimalStep = FloatingPointHelper.GetDecimalStep( (decimal) doubleLimit );

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Success()
        {
            TestMethodsWithGreaterThanAspect( longLimit, null, doubleLimit, ((decimal) doubleLimit) );
            TestMethodsWithGreaterThanAspect( longLimit/2, null, doubleLimit/2, ((decimal) doubleLimit)/2 );
            TestMethodsWithGreaterThanAspect( 0, 0, 0, 0 );
            TestMethodsWithGreaterThanAspect( longLimit*-2, (ulong) Math.Abs( longLimit )*2, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            TestMethodsWithGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Failure()
        {
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit - 1, null, doubleLimit - doubleStep, (decimal) doubleLimit - decimalStep );
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit*2, null, doubleLimit*2, ((decimal) doubleLimit)*2 );
            AssertFails( TestMethodsWithGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Success()
        {
            TestMethodsWithLessThanAspect( longLimit, null, doubleLimit, ((decimal) doubleLimit) );
            TestMethodsWithLessThanAspect( longLimit*2, null, doubleLimit*2, ((decimal) doubleLimit)*2 );
            TestMethodsWithLessThanAspect( long.MinValue, null, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Failure()
        {
            AssertFails( TestMethodsWithLessThanAspect, longLimit + 1, null, doubleLimit + doubleStep, (decimal) doubleLimit + decimalStep );
            AssertFails( TestMethodsWithLessThanAspect, longLimit/2, null, doubleLimit/2, ((decimal) doubleLimit)/2 );
            AssertFails( TestMethodsWithLessThanAspect, 0, 0, 0, 0 );
            AssertFails( TestMethodsWithLessThanAspect, longLimit*-2, (ulong) Math.Abs( longLimit )*2, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            AssertFails( TestMethodsWithLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        private static void TestMethodsWithGreaterThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
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

        private static void TestMethodsWithLessThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
        {
            MethodWithLongLessThanLong( longValue );
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

        private static void MethodWithLongLessThanDouble( [LessThan( doubleLimit )] long? a )
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