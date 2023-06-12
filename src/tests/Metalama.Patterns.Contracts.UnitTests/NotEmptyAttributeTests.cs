// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

// ReSharper disable InconsistentNaming
public class NotEmptyAttributeTests
{
    [Fact]
    public void Given_StringMethodWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        cut.StringMethod( "1234567890" );
    }

    [Fact]
    public void Given_StringMethodWithNotEmpty_When_IncorrecValuePassed_Then_ExceptionIsThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethod( string.Empty ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }

    [Fact]
    public void Given_StringFieldWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        cut.StringField = "1234567890";
    }

    [Fact]
    public void Given_StringFieldWithNotEmpty_When_IncorrecValuePassed_Then_ExceptionIsThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.StringField = string.Empty );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "StringField", e.Message );
    }

    [Fact]
    public void Given_ListMethodWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        cut.ListMethod( new List<int> {1, 2, 3} );
    }

    [Fact]
    public void Given_ListMethodWithNotEmpty_When_IncorrecValuePassed_Then_ExceptionIsThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.ListMethod( new List<int>() ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }

    [Fact]
    public void Given_ListFieldWithNotEmpty_When_CorrectValuePassed_Then_Success()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        cut.ListField = new List<int> { 1, 2, 3 };
    }

    [Fact]
    public void Given_ListFieldWithNotEmpty_When_IncorrecValuePassed_Then_ExceptionIsThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.ListField = new List<int>() );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "ListField", e.Message );
    }

    [Fact]
    public void Given_ICollectionPropertyWithNotEmpty_When_IncorrecValuePassed_Then_ExceptionIsThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.GenericCollectionProperty = new List<int>() );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "GenericCollectionProperty", e.Message );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyRef_When_IncorrectValuePassed_Then_ExceptionThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();
            
        string p = "";
        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.StringMethodWithRef( "abc", ref p ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyRef_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        string p = "abc";
        PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithRef( "", ref p ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyOut_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        string p;
        PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithOut( "", out p ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }

    [Fact]
    public void Given_StringMethodWithNotEmptyRetVal_When_IncorrectValueReturned_Then_ExceptionThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        PostconditionFailedException e = TestHelpers.RecordException<PostconditionFailedException>( () => cut.StringMethodWithRetVal( "" ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "return value", e.Message );
    }

    [Fact]
    public void Given_IReadOnlyCollectionMethodWithNotEmpty_When_IncorrectValuePassed_Then_ExceptionThrown()
    {
        NotEmptyTestClass cut = new NotEmptyTestClass();

        ArgumentException e = TestHelpers.RecordException<ArgumentException>( () => cut.IReadOnlyCollectionMethod( new List<int>().AsReadOnly() ) );

        Assert.NotNull( e );
        Xunit.Assert.Contains( "parameter", e.Message );
    }
}

// ReSharper restore InconsistentNaming

public class NotEmptyTestClass
{
    [NotEmpty] public string StringField;

    [NotEmpty] public List<int> ListField;

    [NotEmpty]
    public ICollection<int> GenericCollectionProperty { get; set; }

    public string StringMethod( [NotEmpty] string parameter )
    {
        return parameter;
    }

    public List<int> ListMethod( [NotEmpty] List<int> parameter )
    {
        return parameter;
    }

    public void StringMethodWithRef( string newVal, [NotEmpty] ref string parameter )
    {
        parameter = newVal;
    }

    public void StringMethodWithOut( string newVal, [NotEmpty] out string parameter )
    {
        parameter = newVal;
    }

    [return: NotEmpty]
    public string StringMethodWithRetVal( string retVal )
    {
        return retVal;
    }

    public void IReadOnlyCollectionMethod<T>( [NotEmpty] IReadOnlyCollection<T> parameter )
    {
    }
}