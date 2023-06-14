// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

public class NotNullAttributeTests
{
    [Fact]
    public void Given_MethodWithNotNullObjectParameter_When_NotNullPassed_Then_Success()
    {
        var cut = new NotNullTestClass();

        cut.ObjectParameterMethod( new object() );
    }

    [Fact]
    public void Given_MethodWithNotNullObjectParameter_When_NullPassed_Then_ExceptionIsThrown()
    {
        var cut = new NotNullTestClass();

        var e = TestHelpers.RecordException<ArgumentNullException>( () => cut.ObjectParameterMethod( null! ) );

        Assert.NotNull( e );
        Assert.Equal( "parameter", e!.ParamName );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_MethodWithNotNullReferenceParameter_When_NullPassed_Then_ExceptionIsThrown()
    {
        var cut = new NotNullTestClass();

        var e = TestHelpers.RecordException<ArgumentNullException>( () => cut.ClassParameterMethod( null! ) );

        Assert.NotNull( e );
        Assert.Equal( "parameter", e!.ParamName );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_NotNullObjectProperty_When_NullAssigned_Then_ExceptionIsThrown()
    {
        var cut = new NotNullTestClass();

        var e = TestHelpers.RecordException<ArgumentNullException>( () => cut.ObjectProperty = null! );

        Assert.NotNull( e );
        Assert.Equal( "value", e!.ParamName );
        Assert.Contains( "ObjectProperty", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_NotNullObjectProperty_When_NotNullAssigned_Then_Success()
    {
        var cut = new NotNullTestClass();

        cut.ObjectProperty = new object();
    }

    [Fact]
    public void Given_NotNullObjectField_When_NotNullAssigned_Then_Success()
    {
        var cut = new NotNullTestClass();

        cut.ObjectField = new object();
    }

    [Fact]
    public void Given_NotNullObjectField_When_NullAssigned_Then_ExceptionIsThrown()
    {
        var cut = new NotNullTestClass();

        var e = TestHelpers.RecordException<ArgumentNullException>( () => cut.ObjectField = null! );

        Assert.NotNull( e );
        Assert.Equal( "value", e!.ParamName );
        Assert.Contains( "ObjectField", e!.Message, StringComparison.Ordinal );
    }
}