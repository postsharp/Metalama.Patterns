// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests.Formatters;

public class FormattersTests : FormattersTestsBase
{
    public FormattersTests( ITestOutputHelper logger )
        : base( logger ) { }

    private struct TestStruct
    {
        public override string ToString()
        {
            return "ToString";
        }
    }

    private class TestStructFormatter : Formatter<TestStruct>
    {
        public TestStructFormatter( IFormatterRepository repository ) : base( repository ) { }

        public override void Write( UnsafeStringBuilder stringBuilder, TestStruct value )
        {
            stringBuilder.Append( "formatter" );
        }
    }

    [Fact]
    public void StronglyTypedUnregisteredType()
    {
        _ = this.DefaultRepository.Get<TestStruct>()!;
    }

    [Fact]
    public void NullableUsesFormatter()
    {
        var repo = GetNewRepository();
        repo.Register( new TestStructFormatter( repo ) );

        Assert.Equal( "formatter", this.Format<TestStruct?>( repo, new TestStruct() ) );
    }

    [Fact]
    public void NullableNull()
    {
        var repo = GetNewRepository();
        repo.Register( new TestStructFormatter( repo ) );

        Assert.Equal( "null", this.Format<TestStruct?>( repo, null ) );
    }

    [Fact]
    public void Formattable()
    {
        Assert.Equal( "Formattable", this.FormatDefault( new FormattableObject() ) );
    }

    [Fact]
    public void NullToString()
    {
        Assert.Equal( "{null}", this.FormatDefault( new NullToStringClass() ) );
    }

    [Fact]
    public void StronglyTypedAnonymous()
    {
        var anonymous = new { A = "a", B = 0 };
        Assert.True( anonymous.GetType().IsAnonymous() );

        Assert.Equal( "{ A = \"a\", B = 0 }", this.FormatDefault( anonymous ) );
    }

    [Fact]
    public void WeaklyTypedAnonymous()
    {
        object anonymous = new { A = "a", B = 0 };
        Assert.True( anonymous.GetType().IsAnonymous() );

        Assert.Equal( "{ A = \"a\", B = 0 }", this.FormatDefault( anonymous ) );
    }

    [Fact]
    public void Types()
    {
        Assert.Equal( "null", this.FormatDefault<Type>( null ) );
        Assert.Equal( "int[]", this.FormatDefault( typeof(int[]) ) );
        Assert.Equal( "List<int>", this.FormatDefault( typeof(List<int>) ) );
    }

    [Fact]
    public void MethodInfo()
    {
        Assert.Equal( "null", this.FormatDefault<MethodInfo>( null ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.Method1()",
            this.FormatDefault(
                typeof(SomeType).GetMethod( nameof(SomeType.Method1), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.Method2(int)",
            this.FormatDefault(
                typeof(SomeType).GetMethod( nameof(SomeType.Method2), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.Method3(int&)",
            this.FormatDefault(
                typeof(SomeType).GetMethod( nameof(SomeType.Method3), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.Method4<T>(List<T>)",
            this.FormatDefault(
                typeof(SomeType).GetMethod( nameof(SomeType.Method4), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );
    }

    [Fact]
    public void ConstructorInfo()
    {
        Assert.Equal( "null", this.FormatDefault<ConstructorInfo>( null ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.new()",
            this.FormatDefault(
                typeof(SomeType).GetConstructors( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                    .Single( c => c.GetParameters().Length == 0 ) ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.new(int)",
            this.FormatDefault(
                typeof(SomeType).GetConstructors( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
                    .Single( c => c.GetParameters().Length == 1 ) ) );

        Assert.Equal(
            "Flashtrace.Formatters.UnitTests.Formatters.FormattersTests.SomeType.StaticConstructor()",
            this.FormatDefault(
                typeof(SomeType).GetConstructors( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic )
                    .Single( c => c.GetParameters().Length == 0 ) ) );
    }

    private class SomeType
    {
#pragma warning disable CS0414
        private static int staticField;
        private int instanceField;
#pragma warning restore CS0414

        static SomeType()
        {
            staticField = 1;
        }

        public SomeType()
        {
            this.instanceField = 1;
        }

        public SomeType( int a )
        {
            this.instanceField = a;
        }

        public void Method1() { }

        public int Method2( int a ) { return a; }

        public void Method3( out int a ) { a = 0; }

        public void Method4<T>( List<T> l ) { }
    }

    [Fact]
    public void DifferentRepositoriesAreDifferent()
    {
        var repo1 = GetNewRepository();
        repo1.Register( new TestStructFormatter( repo1 ) );

        var repo2 = GetNewRepository();

        Assert.Equal( "formatter", this.Format( repo1, new TestStruct() ) );
        Assert.Equal( "{ToString}", this.Format( repo2, new TestStruct() ) );
    }

    private class FormattableObject : IFormattable
    {
        public void Format( UnsafeStringBuilder stringBuilder, IFormatterRepository formatterRepository )
        {
            stringBuilder.Append( "Formattable" );
        }
    }

    private class NullToStringClass
    {
        public override string ToString()
        {
            return null;
        }
    }
}