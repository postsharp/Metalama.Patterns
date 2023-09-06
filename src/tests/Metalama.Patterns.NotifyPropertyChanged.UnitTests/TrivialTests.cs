// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.TrivialAssets;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class TrivialTests : InpcTestsBase
{
    [Fact]
    public void Test1()
    {
        var a = new A();

        var sub = this.SubscribeTo( a );

        a.A1 = 42;

        this.Events.Should().Equal( (sub, "A1") );
    }

    [Fact]
    public void Test2()
    {
        var a = new A();

        var sub = this.SubscribeTo( a );

        a.A1 = 1;
        a.A2 = 2;
        a.A1 = 3;

        this.Events.Should().Equal( (sub, "A1"), (sub, "A2"), (sub, "A1") );
    }
}