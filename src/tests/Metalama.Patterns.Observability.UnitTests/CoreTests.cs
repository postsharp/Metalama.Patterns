﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Observability.UnitTests.Assets.Core;
using Xunit;

namespace Metalama.Patterns.Observability.UnitTests;

public sealed class CoreTests : InpcTestsBase
{
    [Fact]
    public void SimpleIsCorrect()
    {
        var v = new Simple();

        v.S1.Should().Be( 0 );
        v.S2.Should().Be( 0 );
        v.S3.Should().Be( 0 );

        this.SubscribeTo( v );

        this.EventsFrom( () => v.S1 = 0 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.S2 = 0 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.S3 = 0 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.S1 = 1 )
            .Should()
            .Equal( "S1" );

        this.EventsFrom( () => v.S2 = 1 )
            .Should()
            .Equal( "S2" );

        this.EventsFrom( () => v.S3 = 1 )
            .Should()
            .Equal( "S3" );
    }

    [Fact]
    public void SimpleWithInpcPropertiesIsCorrect()
    {
        var v = new SimpleWithInpcProperties();

        v.A1.Should().Be( 0 );
        v.R1.Should().BeNull();
        v.R2.Should().BeNull();

        v.Invoking( v2 => v2.A2 ).Should().Throw<NullReferenceException>();

        var sv = this.SubscribeTo( v );

        this.EventsFrom( () => v.A1 = 1 )
            .Should()
            .Equal( "A1" );

        var a = new Simple();

        var sa = this.SubscribeTo( a );

        this.EventsFrom( () => v.R1 = a )
            .Should()
            .Equal( (sv, "A2"), (sv, "R1") );

        this.EventsFrom( () => v.R1!.S1 = 1 )
            .Should()
            .Equal( (sa, "S1"), (sv, "A2") );

        this.EventsFrom( () => v.R1!.S2 = 1 )
            .Should()
            .Equal( (sa, "S2") );

        this.EventsFrom( () => v.R1!.S3 = 1 )
            .Should()
            .Equal( (sa, "S3") );
    }

    // ReSharper disable once ClassCanBeSealed.Local
    private class DerivedFromExistingInpcImplWithValidOpcMethod : ExistingInpcImplWithValidOpcMethod
    {
        private readonly Action<string> _onPropertyChanged;

        public DerivedFromExistingInpcImplWithValidOpcMethod( Action<string> onPropertyChanged )
        {
            this._onPropertyChanged = onPropertyChanged;
        }

        protected override void OnPropertyChanged( string propertyName )
        {
            this._onPropertyChanged( propertyName );
            base.OnPropertyChanged( propertyName );
        }
    }

    [Fact]
    public void HandCodedExistingInpcImplWithValidOPCMethodIsCorrect()
    {
        List<string> opc = new();

        // Sanity check that hand-coded class behaves as expected.
        var v = new DerivedFromExistingInpcImplWithValidOpcMethod( opc.Add );

        v.EX1.Should().Be( 0 );
        v.EX2.Should().NotBeNull();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.EX1 = v.EX1 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.EX2 = v.EX2 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.EX1 = 1 )
            .Should()
            .Equal( "EX1" );

        this.EventsFrom( () => v.EX2 = null )
            .Should()
            .Equal( "EX2" );

        this.EventsFrom( () => v.EX2 = new Simple() )
            .Should()
            .Equal( "EX2" );

        opc.Should().Equal( "EX1", "EX2", "EX2" );
    }

    // ReSharper disable once ClassCanBeSealed.Local
    private class DerivedFromExistingAbstractInpcImplWithValidOpcMethod : ExistingAbstractInpcImplWithValidOPCMethod
    {
        private readonly Action<string> _onPropertyChanged;

        public DerivedFromExistingAbstractInpcImplWithValidOpcMethod( Action<string> onPropertyChanged )
        {
            this._onPropertyChanged = onPropertyChanged;
        }

        protected override void OnPropertyChanged( string propertyName )
        {
            this._onPropertyChanged( propertyName );
            base.OnPropertyChanged( propertyName );
        }
    }

    [Fact]
    public void HandCodedExistingAbstractInpcImplWithValidOPCMethodIsCorrect()
    {
        List<string> opc = new();

        // Sanity check that hand-coded class behaves as expected.
        var v = new DerivedFromExistingAbstractInpcImplWithValidOpcMethod( opc.Add );

        v.EX1.Should().Be( 0 );
        v.EX2.Should().NotBeNull();

        this.SubscribeTo( v );

        this.EventsFrom( () => v.EX1 = v.EX1 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.EX2 = v.EX2 )
            .Should()
            .BeEmpty();

        this.EventsFrom( () => v.EX1 = 1 )
            .Should()
            .Equal( "EX1" );

        this.EventsFrom( () => v.EX2 = null )
            .Should()
            .Equal( "EX2" );

        this.EventsFrom( () => v.EX2 = new Simple() )
            .Should()
            .Equal( "EX2" );

        opc.Should().Equal( "EX1", "EX2", "EX2" );
    }
}