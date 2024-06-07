﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts.UnitTests.Assets;
using Xunit;

namespace Metalama.Patterns.Contracts.UnitTests;

public sealed class StringLengthAttributeTests
{
    [Fact]
    public void Given_MethodWithMaxLength_When_CorrectValuePassed_Then_Success()
    {
        var cut = new StringLengthTestClass();

        cut.StringMethod( "1234567890" );
    }

    [Fact]
    public void Given_MethodWithMaxLength_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new StringLengthTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethod( "12345678901" ) );

        Assert.NotNull( e );

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_FieldWithMinLengthAndMaxLength_When_CorrectValuePassed_Then_Success()
    {
        var cut = new StringLengthTestClass();

        cut.StringField = "1234567890";
    }

    [Fact]
    public void Given_FieldWithMaxLength_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new StringLengthTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringField = "12345678901" );

        Assert.NotNull( e );

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        Assert.Contains( "StringField", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_FieldWithMinLength_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new StringLengthTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringField = "1234" );

        Assert.NotNull( e );

        // ReSharper disable once RedundantSuppressNullableWarningExpression
        Assert.Contains( "StringField", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_FieldWithMinLength_When_NullValuePassed_Then_NullReferenceException()
    {
        var cut = new StringLengthTestClass();

        var e = TestHelpers.RecordException<NullReferenceException>( () => cut.StringField = null! );

        Assert.NotNull( e );
    }
}