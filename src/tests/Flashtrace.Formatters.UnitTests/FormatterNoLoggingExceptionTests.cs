// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters.UnitTests.Formatters;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests;

public class FormatterNoLoggingExceptionTests : FormattersTestsBase
{
    public FormatterNoLoggingExceptionTests( ITestOutputHelper logger ) : base( logger )
    {
    }

    [Fact]
    public void ThrowingConstructor()
    {
        this.DefaultRepository.Register( typeof( IEnumerable<> ), typeof( ThrowingFormatter<> ) );

        var result = this.FormatDefault<IEnumerable<int>>( new int[0] );

        Assert.True( ThrowingFormatter<int>.Ran );
        Assert.Equal( "{int[]}", result );
    }

    [Fact]
    public void PrivateConstructor()
    {
        this.DefaultRepository.Register( typeof( IEnumerable<> ), typeof( NoConstructorFormatter<> ) );

        var result = this.FormatDefault<IEnumerable<int>>( new int[0] );

        Assert.Equal( "{int[]}", result );
    }

    [Fact]
    public void BadRegistration()
    {
        this.DefaultRepository.Register( typeof(IComparable<>), typeof(ThrowingFormatter<>) );

        var result = this.FormatDefault<IComparable<int>>( 0 );

        Assert.True( ThrowingFormatter<int>.Ran );
        Assert.Equal( "0", result );
    }
}

internal class ThrowingFormatter<T> : Formatter<IEnumerable<T>>
{
    public static bool Ran;

    public ThrowingFormatter( IFormatterRepository repository ) : base( repository )
    {
        Ran = true;
        throw new Exception();
    }

    public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
    {
        throw new NotSupportedException();
    }
}

internal class NoConstructorFormatter<T> : Formatter<IEnumerable<T>>
{
    private NoConstructorFormatter() : base( null! )
    {
    }

    public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
    {
        throw new NotSupportedException();
    }
}