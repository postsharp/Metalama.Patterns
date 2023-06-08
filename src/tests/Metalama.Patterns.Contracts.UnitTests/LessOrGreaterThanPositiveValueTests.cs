using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Patterns.Contracts;


namespace PostSharp.Patterns.Contracts.Tests
{
    public class LessOrGreaterThanPositiveValueTests : RangeContractTestsBase
    {
        private const long longLimit = 100;
        private const ulong ulongLimit = 100;
        private const double doubleLimit = 100;

        private static readonly double doubleStep = FloatingPointHelper.GetDoubleStep( doubleLimit );
        private static readonly decimal decimalStep = FloatingPointHelper.GetDecimalStep( (decimal) doubleLimit );

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Success()
        {
            TestMethodsWithGreaterThanAspect( longLimit, ulongLimit, doubleLimit, (decimal) doubleLimit );
            TestMethodsWithGreaterThanAspect( longLimit*2, ulongLimit*2, doubleLimit*2, ((decimal) doubleLimit)*2 );
            TestMethodsWithGreaterThanAspect( long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
        }

        [Fact]
        public void TestMethodsWithGreaterThanAspect_Failure()
        {
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit - 1, ulongLimit - 1, doubleLimit - doubleStep, (decimal) doubleLimit - decimalStep );
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit/2, ulongLimit/2, doubleLimit/2, ((decimal) doubleLimit)/2 );
            AssertFails( TestMethodsWithGreaterThanAspect, 0, 0, 0, 0 );
            AssertFails( TestMethodsWithGreaterThanAspect, longLimit*-2, null, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            AssertFails( TestMethodsWithGreaterThanAspect, long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Success()
        {
            TestMethodsWithLessThanAspect( longLimit, ulongLimit, doubleLimit, (decimal) doubleLimit );
            TestMethodsWithLessThanAspect( longLimit/2, ulongLimit/2, doubleLimit/2, ((decimal) doubleLimit)/2 );
            TestMethodsWithLessThanAspect( 0, 0, 0, 0 );
            TestMethodsWithLessThanAspect( longLimit*-2, null, doubleLimit*-2, ((decimal) doubleLimit)*-2 );
            TestMethodsWithLessThanAspect( long.MinValue, ulong.MinValue, double.MinValue, decimal.MinValue );
        }

        [Fact]
        public void TestMethodsWithLessThanAspect_Failure()
        {
            AssertFails( TestMethodsWithLessThanAspect, longLimit + 1, ulongLimit + 1, doubleLimit + doubleStep, (decimal) doubleLimit + decimalStep );
            AssertFails( TestMethodsWithLessThanAspect, longLimit*2, ulongLimit*2, doubleLimit*2, ((decimal) doubleLimit)*2 );
            AssertFails( TestMethodsWithLessThanAspect, long.MaxValue, ulong.MaxValue, double.MaxValue, decimal.MaxValue );
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
}