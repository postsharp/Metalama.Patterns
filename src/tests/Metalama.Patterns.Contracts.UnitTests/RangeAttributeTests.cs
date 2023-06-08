// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Common.Tests;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using System;

using PostSharp.Patterns.Contracts;
#if TEST_PLATFORM
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif NUNIT_LIGHT
using NUnit.Framework;
using RangeAttribute = PostSharp.Patterns.Contracts.RangeAttribute;
#else
#endif

namespace PostSharp.Patterns.Contracts.Tests
{
    // ReSharper disable InconsistentNaming
    public class RangeAttributeTests
    {
        [Fact]
        public void Given_MethodWithInRangeParameter_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.ZeroToTenMethod( 0 );
            cut.ZeroToTenMethod( 5 );
            cut.ZeroToTenMethod( 10 );
            cut.ZeroToTenNullableInt( 10 );
        }

        [Fact]
        public void Given_DecimalMethodWithInRangeParameter_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.ZeroToTenDecimal( 0.0m );
            cut.ZeroToTenDecimal( 0.1m );
            cut.ZeroToTenDecimal( 5m );
            cut.ZeroToTenDecimal( 10.0m );
            cut.ZeroToTenNullableDecimal( 10.0m );
        }

        [Fact]
        public void Given_DecimalMethodWithLargeInRangeParameter_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.LargeDecimalRange( decimal.MinValue );
            cut.LargeDecimalRange( decimal.MaxValue );
            cut.LargeDecimalRange( 0m );
        }

        [Fact]
        public void Given_DoubleMethodWithInRangeParameter_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.ZeroToTenDouble( 0.0 );
            cut.ZeroToTenDouble( 0.1 );
            cut.ZeroToTenDouble( 5 );
            cut.ZeroToTenDouble( 10.0 );
        }


        [Fact]
        public void Given_FloatMethodWithInRangeParameter_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.ZeroToTenFloat( 0.0f );
            cut.ZeroToTenFloat( 0.1f );
            cut.ZeroToTenFloat( 5 );
            cut.ZeroToTenFloat( 10.0f );
            cut.ZeroToTenNullableFloat( 10.0f );
        }


        [Fact]
        public void Given_FieldWithInRangeAttribute_When_CorrectValuePassed_Then_Success()
        {
            RangeTestCalss cut = new RangeTestCalss();

            cut.GreaterThanZeroField = 0;
            cut.GreaterThanZeroField = 5;

            cut.LessThanZeroField = 0;
            cut.LessThanZeroField = -5;
        }

        [Fact]
        public void Given_MethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenMethod( -10 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_MethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenMethod( 20 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }


        [Fact]
        public void Given_DoubleMethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenDouble( 10.1 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_DoubleMethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenDouble( -10.0 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }


        [Fact]
        public void Given_FloatMethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenFloat( 10.1f ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_FloatMethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenFloat( -10.0f ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }


        [Fact]
        public void Given_DecimalMethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenDecimal( 20.0m ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_DecimalMethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenDecimal( -10.0m ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_NullableDecimalMethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenNullableDecimal( 20.0m ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_NullableDecimalMethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenNullableDecimal( -10.0m ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_NullableIntMethodWithInRangeParameter_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenNullableInt( 20 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_NullableIntMethodWithInRangeParameter_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenNullableInt( -10 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_FieldWithInRangeAttribute_When_ToSmallValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.GreaterThanZeroField = -10 );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "GreaterThanZeroField", e.Message );
        }

        [Fact]
        public void Given_FieldWithInRangeAttribute_When_ToLargeValuePassed_Then_ExceptionIsThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.LessThanZeroField = 20 );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "LessThanZeroField", e.Message );
        }

        [Fact]
        public void Given_MethodWithInRangeRef_When_IncorrectValuePassed_Then_ExceptionThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            long? p = -1;
            ArgumentOutOfRangeException e = TestHelpers.RecordException<ArgumentOutOfRangeException>( () => cut.ZeroToTenNullableIntRef( 1, ref p ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_MethodWithInRangeRef_When_IncorrectValueReturned_Then_ExceptionThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            long? p = 1;
            PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.ZeroToTenNullableIntRef( -1, ref p ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_MethodWithInRangeOut_When_IncorrectValueReturned_Then_ExceptionThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            long? p;
            PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.ZeroToTenNullableIntOut( -1, out p ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_MethodWithInRangeRetVal_When_IncorrectValueReturned_Then_ExceptionThrown()
        {
            RangeTestCalss cut = new RangeTestCalss();

            PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.ZeroToTenNullableIntRetVal( -1 ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "return value", e.Message );
        }
    }

    // ReSharper restore InconsistentNaming

    public class RangeTestCalss
    {
        [GreaterThan( 0 )] public int GreaterThanZeroField;

        [LessThan( 0 )]
        public long LessThanZeroField { get; set; }

        public int ZeroToTenMethod( [Range( 0, 10 )] short parameter )
        {
            return parameter;
        }

        public double ZeroToTenDouble( [Range( 0d, 10d )] double parameter )
        {
            return parameter;
        }

        public decimal ZeroToTenDecimal( [Range( 0d, 10d )] decimal parameter )
        {
            return parameter;
        }

        public decimal? ZeroToTenNullableDecimal( [Range( 0d, 10d )] decimal? parameter )
        {
            return parameter;
        }

        public long? ZeroToTenNullableInt( [Range( 0, 10 )] long? parameter )
        {
            return parameter;
        }

        public float ZeroToTenFloat( [Range(0, 10)] float parameter )
        {
            return parameter;
        }

        public float? ZeroToTenNullableFloat( [Range( 0, 10 )] float? parameter )
        {
            return parameter;
        }

        public decimal LargeDecimalRange( [Range( double.MinValue, double.MaxValue )] decimal parameter )
        {
            return parameter;
        }

        public void ZeroToTenNullableIntRef( long? newVal, [Range( 0, 10 )] ref long? parameter )
        {
            parameter = newVal;
        }

        public void ZeroToTenNullableIntOut( long? newVal, [Range( 0, 10 )] out long? parameter )
        {
            parameter = newVal;
        }

        [return: Range( 0, 10 )]
        public long? ZeroToTenNullableIntRetVal( long? retVal )
        {
            return retVal;
        }
    }
}