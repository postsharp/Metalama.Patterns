// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.BasicReferenceAssets;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class BasicReferenceTests : InpcTestsBase
{
    [Fact]
    public void Test1()
    {
        var c = new C();
        var d = new D();

        var subC = this.SubscribeTo( c );
        var subD = this.SubscribeTo( d );

        c.C1 = 1;
        d.D1 = 2;
        c.C2 = d;
        d.D1 = 3;

        this.Events.Should().Equal(
            (subC, "C1"),
            (subD, "D1"),
            (subC, "C2"),
            (subD, "D1") );
    }
}