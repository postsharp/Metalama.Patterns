// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Enable when INPC-type properties with initializers are supported.

#if false
using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Initializers;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class InitializerTests : InpcTestsBase
{
    [Fact]
    public void InpcAutoPropertyWithInitializer() 
    {
        var v = new A();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.A1.S1 = 1 ).Should().Equal( "A2" );
    }
}

#endif