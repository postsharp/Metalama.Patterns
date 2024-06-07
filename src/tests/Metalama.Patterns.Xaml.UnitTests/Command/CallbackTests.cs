﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Xaml.UnitTests.Assets.Command;
using Xunit;

namespace Metalama.Patterns.Xaml.UnitTests.Command;

public sealed class CallbackTests
{
    private CallbackTestClass Instance { get; }

    private readonly CommandTestBase.ThreadContext _threadContext;

    public CallbackTests()
    {
        this._threadContext = CommandTestBase.ThreadContext.Current;
        this._threadContext.Reset();
        this.Instance = new CallbackTestClass();
    }

    private void ThrowIfThreadContextHasChanged()
    {
        if ( this._threadContext != CommandTestBase.ThreadContext.Current )
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

    private void SetCanExecute( Func<int?, bool>? canExecute )
    {
        this.ThrowIfThreadContextHasChanged();
        this._threadContext.CanExecute = canExecute;
    }

    [Fact]
    public void ImplicitInstanceMethodNoParameterCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitInstanceMethodNoParameterCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitInstanceMethodNoParameter" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitInstanceMethodNoParameterCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitInstanceMethodNoParameter", "ExecuteImplicitInstanceMethodNoParameter" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitInstanceMethodNoParameterCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitInstanceMethodNoParameterCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitInstanceMethodWithParameterCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitInstanceMethodWithParameterCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitInstanceMethodWithParameter|42" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitInstanceMethodWithParameterCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitInstanceMethodWithParameter|42", "ExecuteImplicitInstanceMethodWithParameter|42" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitInstanceMethodWithParameterCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitInstanceMethodWithParameterCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitStaticMethodNoParameterCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitStaticMethodNoParameterCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitStaticMethodNoParameter" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitStaticMethodNoParameterCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitStaticMethodNoParameter", "ExecuteImplicitStaticMethodNoParameter" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitStaticMethodNoParameterCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitStaticMethodNoParameterCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitStaticMethodWithParameterCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitStaticMethodWithParameterCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitStaticMethodWithParameter|42" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitStaticMethodWithParameterCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitStaticMethodWithParameter|42", "ExecuteImplicitStaticMethodWithParameter|42" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitStaticMethodWithParameterCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitStaticMethodWithParameterCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitInstancePropertyCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitInstancePropertyCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitInstanceProperty" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitInstancePropertyCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitInstanceProperty", "ExecuteImplicitInstanceProperty" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitInstancePropertyCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitInstancePropertyCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitStaticPropertyCommand()
    {
        var log = this.Log;

        this.Instance.ImplicitStaticPropertyCommand.CanExecute( 42 ).Should().BeTrue();
        log.Should().Equal( "CanExecuteImplicitStaticProperty" );
        log.Clear();

        this.Instance.Invoking( c => c.ImplicitStaticPropertyCommand.Execute( 42 ) ).Should().NotThrow();
        log.Should().Equal( "CanExecuteImplicitStaticProperty", "ExecuteImplicitStaticProperty" );

        this.SetCanExecute( _ => false );
        this.Instance.ImplicitStaticPropertyCommand.CanExecute( 42 ).Should().BeFalse();
        this.Instance.Invoking( c => c.ImplicitStaticPropertyCommand.Execute( 42 ) ).Should().Throw<InvalidOperationException>();
    }
}