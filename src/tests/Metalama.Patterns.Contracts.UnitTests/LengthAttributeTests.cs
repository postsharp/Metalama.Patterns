// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using PostSharp.Patterns.Common.Tests;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;

using PostSharp.Patterns.Contracts;

namespace PostSharp.Patterns.Contracts.Tests
{
    // ReSharper disable InconsistentNaming
    public class LengthAttributeTests
    {
        [Fact]
        public void Given_MethodWithMaxLength_When_CorrectValuePassed_Then_Success()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            cut.StringMethod( "1234567890" );
        }

        [Fact]
        public void Given_MethodWithMaxLength_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethod( "12345678901" ) );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "parameter", e.Message );
        }

        [Fact]
        public void Given_FieldWithMinLengthAndMaxLength_When_CorrectValuePassed_Then_Success()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            cut.StringField = "1234567890";
        }

        [Fact]
        public void Given_FieldWithMaxLength_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.StringField = "12345678901" );

            Assert.NotNull( e );
            Xunit.Assert.Contains( "StringField", e.Message );
        }

        [Fact]
        public void Given_FieldWithMinLength_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            ArgumentException e = TestHelpers.RecordException<ArgumentException>(() => cut.StringField = "1234");

            Assert.NotNull(e);
            Xunit.Assert.Contains( "StringField", e.Message );
        }

        [Fact]
        public void Given_FieldWithMinLength_When_NullValuePassed_Then_Success()
        {
            MaxLengthTestClass cut = new MaxLengthTestClass();

            cut.StringField = null;

            Assert.Null( cut.StringField );
        }


    }

    // ReSharper restore InconsistentNaming

    public class MaxLengthTestClass
    {
        [StringLength(5,10)]
        public string StringField;

        public string StringMethod( [StringLength( 10 )] string parameter )
        {
            return parameter;
        }
    }
}