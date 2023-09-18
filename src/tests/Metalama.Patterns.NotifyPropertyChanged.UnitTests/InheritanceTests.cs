// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Inheritance;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class InheritanceTests : InpcTestsBase
{
    private void Test( Simple v )
    {
        v.S1.Should().Be( 0 );
        v.S2.Should().Be( 0 );
        v.S3.Should().Be( 0 );

        this.SubscribeTo( v );

        this.EventsFrom( () => v.S1 = 0 )
            .Should().BeEmpty();

        this.EventsFrom( () => v.S2 = 0 )
            .Should().BeEmpty();

        this.EventsFrom( () => v.S3 = 0 )
            .Should().BeEmpty();

        this.EventsFrom( () => v.S1 = 1 )
            .Should().Equal( "S1" );

        this.EventsFrom( () => v.S2 = 1 )
            .Should().Equal( "S2" );

        this.EventsFrom( () => v.S3 = 1 )
            .Should().Equal( "S3" );
    }

    private void Test( C2 v )
    {
        v.C2P1.Should().Be( 0 );
        
        this.Test( (Simple) v );

        this.EventsFrom( () => v.C2P1 = 1 )
            .Should().Equal( "C2P1" );
    }

    private void Test( C3 v )
    {
        v.C3P1.Should().Be( 0 );

        this.Test( (C2) v );

        this.EventsFrom( () => v.C3P1 = 1 )
            .Should().Equal( "C3P1" );
    }

    private void Test( C4 v )
    {
        v.C4P1.Should().Be( 0 );

        this.Test( (C3) v );

        this.EventsFrom( () => v.C4P1 = 1 )
            .Should().Equal( "C4P1" );
    }

    private void Test( C5 v )
    {
        v.C5P1.Should().Be( 0 );

        this.Test( (C4) v );

        this.EventsFrom( () => v.C5P1 = 1 )
            .Should().Equal( "C5P1" );
    }

    [Fact]
    public void Trivial()
    {
        this.Test( new C1() );
    }

    [Fact]
    public void Depth2()
    {
        this.Test( new C2() );
    }

    [Fact]
    public void Depth3()
    {
        this.Test( new C3() );
    }

    [Fact]
    public void Depth4()
    {
        this.Test( new C4() );
    }

    [Fact]
    public void Depth5()
    {
        this.Test( new C5() );
    }

    [Fact]
    public void RefToBaseProperty()
    {
        var v = new C6();

        v.S1.Should().Be( 0 );
        v.C6P1.Should().Be( 0 );
        v.S1 = -1;
        v.C6P1.Should().Be( -1 );

        this.SubscribeTo( v );

        this.EventsFrom( () => v.S1 = 1 )
            .Should().Equal( "C6P1", "S1" );

        this.EventsFrom( () => v.S2 = 1 )
            .Should().Equal( "S2");

        this.EventsFrom( () => v.S3 = 1 )
            .Should().Equal( "S3" );
    }

    [Fact]
    public void RefToBaseBaseProperty()
    {
        var v = new C7();

        v.S1.Should().Be( 0 );
        v.C6P1.Should().Be( 0 );
        v.C7P1.Should().Be( 0 );
        v.S1 = -1;
        v.C6P1.Should().Be( -1 );
        v.C7P1.Should().Be( -1 );

        this.SubscribeTo( v );

        this.EventsFrom( () => v.S1 = 1 )
            .Should().Equal( "C7P1", "C6P1", "S1" );

        this.EventsFrom( () => v.S2 = 1 )
            .Should().Equal( "S2" );

        this.EventsFrom( () => v.S3 = 1 )
            .Should().Equal( "S3" );
    }

    [Fact]
    public void DerivedReferencesUnreferencedChildOfBaseInpcProperty()
    {
        var v = new C16();

        var sv = this.SubscribeTo( v );

        var a = new Simple();

        var sa = this.SubscribeTo( a );

        // TODO: C16 does not look OPC R1 (and then fire C16P1).
        this.EventsFrom( () => v.R1 = a )
            .Should().Equal( (sv, "C16P1"), (sv, "A2"), (sv, "R1") );

        this.EventsFrom( () => a.S1 = 1 )
            .Should().Equal( (sv, "A2"), (sa, "S1") );

        this.EventsFrom( () => a.S2 = 1 )
            .Should().Equal( (sv, "C16P1"), (sa, "S2") );
    }
}