using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Patterns.Contracts;


// This is a modified copy of StrictlyLessOrGreaterThanPositiveValueTests.cs
// Keep the testing logic equal for all the copies!

namespace PostSharp.Patterns.Contracts.Tests
{
    public class StrictlyLessOrGreaterThanNegativeValueTests : RangeContractTestsBase
    {
        private const long longLimit = -100;
        private const double doubleLimit = -100;

        private static readonly double doubleStep = FloatingPointHelper.GetDoubleStep( doubleLimit );
        private static readonly decimal decimalStep = FloatingPointHelper.GetDecimalStep( (decimal) doubleLimit );

        [Fact]
        public void TestMethodsWithStrictlyGreaterThanAspect_Success()
        {
            TestMethodsWithStrictlyGreaterThanAspect( longLimit + 1, null, doubleLimit + doubleStep, ((decimal) doubleLimit) + decimalStep );
            TestMethodsWithStrictlyGreaterThanAspect( longLimit/2, null, doubleLimit/2, ((decimal) doubleLimit)/2 );
            TestMethodsWithStrictlyGreaterThanAspect( 0, 0, 0, 0 );
            TestMethodsWithStrictlyGreaterThanAspect( longLimit*-2, (ulong) Math.Abs( longLimit )*2, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            TestMethodsWithStrictlyGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        [Fact]
        public void TestMethodsWithStrictlyGreaterThanAspect_Failure()
        {
            AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit, null, doubleLimit, (decimal) doubleLimit );
            AssertFails( TestMethodsWithStrictlyGreaterThanAspect, longLimit*2, null, doubleLimit*2, ((decimal) doubleLimit)*2 );
            AssertFails( TestMethodsWithStrictlyGreaterThanAspect, long.MinValue, null, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithStrictlyLessThanAspect_Success()
        {
            TestMethodsWithStrictlyLessThanAspect( longLimit - 1, null, doubleLimit - doubleStep, ((decimal) doubleLimit) - decimalStep );
            TestMethodsWithStrictlyLessThanAspect( longLimit*2, null, doubleLimit*2, ((decimal) doubleLimit)*2 );
            TestMethodsWithStrictlyLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithStrictlyLessThanAspect_Failure()
        {
            AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit, null, doubleLimit, (decimal) doubleLimit );
            AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit/2, null, doubleLimit/2, ((decimal) doubleLimit)/2 );
            AssertFails( TestMethodsWithStrictlyLessThanAspect, 0, 0, 0, 0 );
            AssertFails( TestMethodsWithStrictlyLessThanAspect, longLimit*-2, (ulong) Math.Abs( longLimit )*2, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            AssertFails( TestMethodsWithStrictlyLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        private static void TestMethodsWithStrictlyGreaterThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
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

        private static void TestMethodsWithStrictlyLessThanAspect( long? longValue, ulong? ulongValue, double? doubleValue, decimal? decimalValue )
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

        private static void MethodWithLongStrictlyLessThanDouble( [StrictlyLessThan( doubleLimit )] long? a )
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
}