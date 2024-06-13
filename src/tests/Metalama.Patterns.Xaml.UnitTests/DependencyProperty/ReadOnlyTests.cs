// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Xaml.UnitTests.Assets.DependencyPropertyNS;
using Xunit;

namespace Metalama.Patterns.Xaml.UnitTests.DependencyPropertyNS;

public sealed class ReadOnlyTests
{
    [Fact]
    public void PublicSetFails()
    {
        var o = new ReadOnlyTestClass();
        Assert.Throws<InvalidOperationException>( () => o.SetValue( ReadOnlyTestClass.NameProperty, "x" ) );
    }

    [Fact]
    public void PrivateSetSucceeds()
    {
        var o = new ReadOnlyTestClass();
        o.SetName( "x" );
    }
}