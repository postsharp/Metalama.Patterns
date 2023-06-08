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
    public class RegularExpressionAttributeTests
    {
        [Fact]
        public void Given_MethodWithRegexMatch_When_CorrectValuePassed_Then_Success()
        {
            var cut = new RegexTestClass();

            cut.SetEmail("test@postsharp.test");
        }

        [Fact]
        public void Given_MethodWithRegexMatch_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            var cut = new RegexTestClass();

            var e = TestHelpers.RecordException<ArgumentException>(() => cut.SetEmail("asd"));

            Assert.NotNull(e);
            Xunit.Assert.Contains( "email", e.Message );
        }

        [Fact]
        public void Given_FieldWithRegexMatch_When_CorrectValuePassed_Then_Success()
        {
            var cut = new RegexTestClass();

            cut.Email = "test@postsharp.test";
        }

        [Fact]
        public void Given_FieldWithRegexMatch_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            var cut = new RegexTestClass();

            var e = TestHelpers.RecordException<ArgumentException>(() => cut.Email = "asd");

            Assert.NotNull(e);
            Xunit.Assert.Contains( "Email", e.Message );
        }

        [Fact]
        public void Given_FieldWithEmail_When_CorrectValuePassed_Then_Success()
        {
            var cut = new RegexTestClass();

            cut.Email2 = "test@postsharp.test";
        }

        [Fact]
        public void Given_FieldEmail_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            var cut = new RegexTestClass();

            var e = TestHelpers.RecordException<ArgumentException>(() => cut.Email2 = "asd");

            Assert.NotNull(e);
            Xunit.Assert.Contains( "Email2", e.Message );
        }

        [Fact]
        public void Given_FieldWithPhone_When_CorrectValuePassed_Then_Success()
        {
            var cut = new RegexTestClass();

            cut.PhoneField = "644-14-90";
        }

        [Fact]
        public void Given_FieldPhone_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            var cut = new RegexTestClass();

            var e = TestHelpers.RecordException<ArgumentException>(() => cut.PhoneField = "a123");

            Assert.NotNull(e);
            Xunit.Assert.Contains( "PhoneField", e.Message );
        }

        [Fact]
        public void Given_FieldWithUrl_When_CorrectValuePassed_Then_Success()
        {
            var cut = new RegexTestClass();

            cut.UrlField = "http://www.sharpcrafters.com/";
        }

        [Fact]
        public void Given_FieldUrl_When_IncorrecValuePassed_Then_ExceptionIsThrown()
        {
            var cut = new RegexTestClass();

            var e = TestHelpers.RecordException<ArgumentException>(() => cut.UrlField = "dslkfusd");

            Assert.NotNull(e);
            Xunit.Assert.Contains( "UrlField", e.Message );
        }
    }
    // ReSharper restore InconsistentNaming

    public class RegexTestClass
    {
        [RegularExpression(".+@.+")] public string Email;

        [EmailAddress] public string Email2;

        [Phone] public string PhoneField;

        [Url] public string UrlField;

        public string SetEmail([RegularExpression(".+@.+")]string email)
        {
            return email;
        }
    }
}