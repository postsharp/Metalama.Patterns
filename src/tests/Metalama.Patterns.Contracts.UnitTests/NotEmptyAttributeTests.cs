// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts.UnitTests.Assets;
using System.Collections.Immutable;
using Xunit;

namespace Metalama.Patterns.Contracts.UnitTests;

// ReSharper disable RedundantSuppressNullableWarningExpression
public sealed class NotEmptyAttributeTests
{
    [Fact]
    public void Given_StringMethodWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        var cut = new NotEmptyTestClass();

        cut.StringMethod( "1234567890" );
    }

    [Fact]
    public void Given_StringMethodWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethod( string.Empty ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_StringFieldWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        var cut = new NotEmptyTestClass();

        cut.StringField = "1234567890";
    }

    [Fact]
    public void Given_StringFieldWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringField = string.Empty );

        Assert.NotNull( e );
        Assert.Contains( "StringField", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_ListMethodWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        var cut = new NotEmptyTestClass();

        cut.ListMethod( new List<int> { 1, 2, 3 } );
    }

    [Fact]
    public void Given_ListMethodWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.ListMethod( new List<int>() ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_ListFieldWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        var cut = new NotEmptyTestClass();

        cut.ListField = new List<int> { 1, 2, 3 };
    }

    [Fact]
    public void Given_ListFieldWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.ListField = new List<int>() );

        Assert.NotNull( e );
        Assert.Contains( "ListField", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_ICollectionPropertyWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionIsThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.GenericCollectionProperty = new List<int>() );

        Assert.NotNull( e );
        Assert.Contains( "GenericCollectionProperty", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyRef_When_IncorrectValuePassed_Then_ExceptionThrown()
    {
        var cut = new NotEmptyTestClass();

        var p = "";
        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethodWithRef( "abc", ref p ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [SkippableFact( Skip = "#33302" )]
    public void Given_StringMethodWithNotEmptyRef_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        var cut = new NotEmptyTestClass();

        var p = "abc";
        var e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithRef( "", ref p ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyOut_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithOut( "", out _ ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyRetVal_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithRetVal( "" ) );

        Assert.NotNull( e );
        Assert.Contains( "return value", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_IReadOnlyCollectionMethodWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionThrown()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.IReadOnlyCollectionMethod( new List<int>().AsReadOnly() ) );

        Assert.NotNull( e );
        Assert.Contains( "parameter", e!.Message, StringComparison.Ordinal );
    }

    [Fact]
    public void Given_ArrayWithNotEmptyAndNotNull_When_NullPassed_Then_ArgumentNullException()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.Array( null! ) );

        Assert.IsType<ArgumentNullException>( e );
    }

    [Fact]
    public void Given_ArrayWithNotEmpty_When_SingletonPassed_Then_Succeeds()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.Array( new[] { 1 } ) );

        Assert.Null( e );
    }

    [Fact]
    public void Given_ArrayWithNotEmpty_When_EmptyPassed_Then_Succeeds()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.Array( Array.Empty<int>() ) );

        Assert.NotNull( e );
    }

    [Fact]
    public void Given_ImmutableArrayWithNotEmpty_When_DefaultPassed_Then_Succeeds()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.ImmutableArray( default ) );

        Assert.Null( e );
    }

    [Fact]
    public void Given_ImmutableArrayWithNotEmpty_When_SingletonPassed_Then_Succeeds()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.ImmutableArray( ImmutableArray.Create( 1 ) ) );

        Assert.Null( e );
    }

    [Fact]
    public void Given_ImmutableArrayWithNotEmpty_When_EmptyPassed_Then_Succeeds()
    {
        var cut = new NotEmptyTestClass();

        var e = TestHelpers.RecordException<ArgumentException>(
            () =>
                cut.ImmutableArray( ImmutableArray<int>.Empty ) );

        Assert.NotNull( e );
    }
}