// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

// ReSharper disable InconsistentNaming
public class EnumDataTypeTests
{
    [Fact]
    public void Given_FieldWithEnumDataType_When_CorrectEnumObjectPassed_Then_NoExceptionIsThrown()
    {
        var cut = new EnumTestClass();
        cut.ObjectEnum = TestEnum.Foo;
    }

    [Fact]
    public void Given_FieldWithEnumDataType_When_CorrectEnumStringPassed_Then_NoExceptionIsThrown()
    {
        var cut = new EnumTestClass();
        cut.StringEnum = TestEnum.Foo.ToString();
    }

    [Fact]
    public void Given_FieldWithEnumDataType_When_CorrectEnumIntPassed_Then_NoExceptionIsThrown()
    {
        var cut = new EnumTestClass();
        cut.IntEnum = (int) TestEnum.Foo;
    }

    [Fact]
    public void Given_FieldWithEnumDataType_When_IncorrectIntPassed_Then_ExceptionIsThrown()
    {
        var cut = new EnumTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.IntEnum = 10 );

        Assert.NotNull( e );
        Assert.Contains( "IntEnum", e.Message );
    }

    [Fact]
    public void Given_FieldWithEnumDataType_When_IncorrectStringPassed_Then_ExceptionIsThrown()
    {
        var cut = new EnumTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.StringEnum = "asd" );

        Assert.NotNull( e );
        Assert.Contains( "StringEnum", e.Message );
    }

    [Fact]
    public void Given_FieldWithEnumDataType_When_IncorrectObjectPassed_Then_ExceptionIsThrown()
    {
        var cut = new EnumTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.ObjectEnum = new object() );

        Assert.NotNull( e );
        Assert.Contains( "ObjectEnum", e.Message );
    }

    [Fact]
    public void Given_FieldWithFlagsEnumDataType_When_CorrectEnumIntPassed_Then_NoExceptionIsThrown()
    {
        var cut = new EnumTestClass();
        cut.IntFlag = (int) TestFlagsEnum.Foo;
    }

    [Fact]
    public void Given_FieldWithFlagsEnumDataType_When_IncorrectIntPassed_Then_ExceptionIsThrown()
    {
        var cut = new EnumTestClass();

        var e = TestHelpers.RecordException<ArgumentException>( () => cut.IntFlag = 10 );

        Assert.NotNull( e );
        Assert.Contains( "IntFlag", e.Message );
    }
}
// ReSharper restore InconsistentNaming

public class EnumTestClass
{
    [EnumDataType( typeof(TestEnum) )]
    public string StringEnum;

    [EnumDataType( typeof(TestEnum) )]
    public int IntEnum;

    [EnumDataType( typeof(TestEnum) )]
    public object ObjectEnum;

    [EnumDataType( typeof(TestFlagsEnum) )]
    public int IntFlag;
}

public enum TestEnum
{
    Foo,
    Bar
}

[Flags]
public enum TestFlagsEnum
{
    Foo = 1,
    Bar = 2
}