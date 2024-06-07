﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Xaml.UnitTests.Assets.DependencyPropertyNS;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Metalama.Patterns.Xaml.UnitTests.DependencyPropertyNS;

public sealed class CallbackTests
{
    private CallbackTestClass Instance { get; }

    private readonly CallbackTestClass.ThreadContext _threadContext;

    public CallbackTests()
    {
        this._threadContext = CallbackTestClass.ThreadContext.Current;
        this._threadContext.Reset();
        this.Instance = new CallbackTestClass();
    }

    private void ThrowIfThreadContextHasChanged()
    {
        if ( this._threadContext != CallbackTestClass.ThreadContext.Current )
        {
            throw new InvalidOperationException( "Test harness problem: CallbackTestClass.ThreadContext.Current has changed." );
        }
    }

    private List<string> Log
    {
        get
        {
            this.ThrowIfThreadContextHasChanged();

            return this._threadContext.Log;
        }
    }

    private void SetOnValidate( Func<int, bool>? onValidate )
    {
        this.ThrowIfThreadContextHasChanged();
        this._threadContext.OnValidate = onValidate;
    }

    [Fact]
    public void ImplicitInstanceDependencyProperty()
    {
        this.Instance.ImplicitInstanceDependencyProperty = 42;
        this.Log.Should().Equal( "OnImplicitInstanceDependencyPropertyChanging", "OnImplicitInstanceDependencyPropertyChanged" );
    }

    [Fact]
    public void ImplicitInstanceNoParameters()
    {
        this.Instance.ImplicitInstanceNoParameters = 42;
        this.Log.Should().Equal( "OnImplicitInstanceNoParametersChanging", "OnImplicitInstanceNoParametersChanged" );
    }

    [Fact]
    public void ImplicitInstanceValue()
    {
        this.Instance.ImplicitInstanceValue = 42;
        this.Log.Should().Equal( "OnImplicitInstanceValueChanging|42", "OnImplicitInstanceValueChanged|42" );
    }

    [Fact]
    public void ImplicitInstanceOldValueNewValue()
    {
        this.Instance.ImplicitInstanceOldValueNewValue = 42;
        this.Log.Should().Equal( "OnImplicitInstanceOldValueNewValueChanged|0|42" );
    }

    [Fact]
    public void ImplicitStaticDependencyProperty()
    {
        this.Instance.ImplicitStaticDependencyProperty = 42;
        this.Log.Should().Equal( "OnImplicitStaticDependencyPropertyChanging", "OnImplicitStaticDependencyPropertyChanged" );
    }

    [Fact]
    public void ImplicitStaticDependencyPropertyAndInstance()
    {
        this.Instance.ImplicitStaticDependencyPropertyAndInstance = 42;

        this.Log.Should()
            .Equal(
                $"OnImplicitStaticDependencyPropertyAndInstanceChanging|{this.Instance.Id}",
                $"OnImplicitStaticDependencyPropertyAndInstanceChanged|{this.Instance.Id}" );
    }

    [Fact]
    public void ImplicitStaticInstance()
    {
        this.Instance.ImplicitStaticInstance = 42;

        this.Log.Should()
            .Equal(
                $"OnImplicitStaticInstanceChanging|{this.Instance.Id}",
                $"OnImplicitStaticInstanceChanged|{this.Instance.Id}" );
    }

    [Fact]
    public void ImplicitStaticNoParameters()
    {
        this.Instance.ImplicitStaticNoParameters = 42;
        this.Log.Should().Equal( "OnImplicitStaticNoParametersChanging", "OnImplicitStaticNoParametersChanged" );
    }

    [Fact]
    public void ImplicitInstanceValidateDependencyPropertyAndValue()
    {
        this.SetOnValidate( v => v == 42 );
        this.Instance.ImplicitInstanceValidateDependencyPropertyAndValue = 42;

        this.Log.Should()
            .Equal(
                "ValidateImplicitInstanceValidateDependencyPropertyAndValue|42",
                "OnImplicitInstanceValidateDependencyPropertyAndValueChanging",
                "OnImplicitInstanceValidateDependencyPropertyAndValueChanged" );

        this.Instance.Invoking( t => t.ImplicitInstanceValidateDependencyPropertyAndValue = 99 )
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ImplicitInstanceValidateValue()
    {
        this.SetOnValidate( v => v == 42 );
        this.Instance.ImplicitInstanceValidateValue = 42;

        this.Log.Should()
            .Equal(
                "ValidateImplicitInstanceValidateValue|42",
                "OnImplicitInstanceValidateValueChanging",
                "OnImplicitInstanceValidateValueChanged" );

        this.Instance.Invoking( t => t.ImplicitInstanceValidateValue = 99 )
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ImplicitStaticValidateDependencyPropertyAndInstanceAndValue()
    {
        this.SetOnValidate( v => v == 42 );
        this.Instance.ImplicitStaticValidateDependencyPropertyAndInstanceAndValue = 42;

        this.Log.Should()
            .Equal(
                $"ValidateImplicitStaticValidateDependencyPropertyAndInstanceAndValue|{this.Instance.Id}|42",
                "OnImplicitStaticValidateDependencyPropertyAndInstanceAndValueChanging",
                "OnImplicitStaticValidateDependencyPropertyAndInstanceAndValueChanged" );

        this.Instance.Invoking( t => t.ImplicitStaticValidateDependencyPropertyAndInstanceAndValue = 99 )
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ImplicitStaticValidateDependencyPropertyAndValue()
    {
        this.SetOnValidate( v => v == 42 );
        this.Instance.ImplicitStaticValidateDependencyPropertyAndValue = 42;

        this.Log.Should()
            .Equal(
                "ValidateImplicitStaticValidateDependencyPropertyAndValue|42",
                "OnImplicitStaticValidateDependencyPropertyAndValueChanging",
                "OnImplicitStaticValidateDependencyPropertyAndValueChanged" );

        this.Instance.Invoking( t => t.ImplicitStaticValidateDependencyPropertyAndValue = 99 )
            .Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void ImplicitStaticValidateValue()
    {
        this.SetOnValidate( v => v == 42 );
        this.Instance.ImplicitStaticValidateValue = 42;

        this.Log.Should()
            .Equal(
                "ValidateImplicitStaticValidateValue|42",
                "OnImplicitStaticValidateValueChanging",
                "OnImplicitStaticValidateValueChanged" );

        this.Instance.Invoking( t => t.ImplicitStaticValidateValue = 99 )
            .Should()
            .Throw<ArgumentException>();
    }
}