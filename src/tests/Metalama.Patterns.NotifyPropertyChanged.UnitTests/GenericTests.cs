// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Generic;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class GenericTests : InpcTestsBase
{
    // TODO: Pending #33805
#if false
    [Fact]
    public void PropertyOfGenericTypeThatIsClassAndINPC() 
    {
        var v = new AOfSimple();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.A1 = new() )
            .Should().Equal( "A1", "RefA1S1" );

        this.EventsFrom( () => v.A1.S1 = 1 )
            .Should().Equal( "RefA1S1" );
    }
#endif

    [Fact]
    public void PropertyofGenericTypeThatIsClassButNotINPC()
    {
        var v = new B<string>();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.B1 = "hello" )
            .Should().Equal( "B1" );
    }

    [Fact]
    public void PropertyOfGenericTypeThatIsStructButNotINPC()
    {
        var v = new C<int>();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.C1 = 123 )
            .Should().Equal( "C1" );
    }

    [Fact]
    public void PropertyOfGenericTypeThatIsClassAndINPCAndIFoo()
    {
        var v = new D<MyFoo>();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.D1 = new() )
            .Should().Equal( "FooX", "D1" );

        this.EventsFrom( () => v.D1.X = 1 )
            .Should().Equal( "FooX" );

        this.EventsFrom( () => v.D1.Y = 1 )
            .Should().BeEmpty();
    }

    // TODO: Pending #33805
#if false
    [Fact]
    public void PropertyOfGenericTypeThatIsClassAndINPCAndIFooDepth2()
    {
        var v = new DD<MyFoo>();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.D1 = new() )
            .Should().Equal( "D1", "FooX", "FooY" );

        this.EventsFrom( () => v.D1.X = 1 )
            .Should().Equal( "FooX" );

        this.EventsFrom( () => v.D1.Y = 1 )
            .Should().Equal( "FooY" );
    }
#endif
}