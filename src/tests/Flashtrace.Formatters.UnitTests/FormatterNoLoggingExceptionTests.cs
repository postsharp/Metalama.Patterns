// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable UseArrayEmptyMethod
#pragma warning disable CA1825

namespace Flashtrace.Formatters.UnitTests;

public sealed class FormatterNoLoggingExceptionTests : FormattersTestsBase
{
    public FormatterNoLoggingExceptionTests( ITestOutputHelper logger ) : base( logger ) { }

    [Fact]
    public void ThrowingConstructor()
    {
        this.DefaultRepository.Register( typeof(IEnumerable<>), typeof(ThrowingFormatter<>) );

        var result = this.FormatDefault<IEnumerable<int>>( new int[0] );

        Assert.True( ThrowingFormatter<int>.Ran );
        Assert.Equal( "{int[]}", result );
    }

    [Fact]
    public void PrivateConstructor()
    {
        this.DefaultRepository.Register( typeof(IEnumerable<>), typeof(NoConstructorFormatter<>) );

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