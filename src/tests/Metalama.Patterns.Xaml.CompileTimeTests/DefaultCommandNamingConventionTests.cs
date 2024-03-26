﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;
using Xunit;

namespace Metalama.Patterns.Xaml.CompileTimeTests;

public sealed class DefaultCommandNamingConventionTests
{
    [InlineData( "execute", "execute" )]
    [InlineData( "m_execute", "m_execute" )]
    [InlineData( "m_", "m_" )]
    [InlineData( "_", "_" )]
    [InlineData( "Command", "Command" )]
    [InlineData( "_command", "Command" )]
    [InlineData( "executeFoo", "Foo" )]
    [InlineData( "ExecuteFoo", "Foo" )]
    [InlineData( "m_foo", "Foo" )]
    [InlineData( "m_executeFoo", "Foo" )]
    [InlineData( "Foo", "Foo" )]
    [InlineData( "foo", "Foo" )]
    [InlineData( "FooCommand", "Foo" )]
    [InlineData( "Foo_Command", "Foo" )]
    [InlineData( "foo_command", "Foo" )]
    [InlineData( "m_execute_foo_command", "Foo" )]
    [Theory]
    public void GetCommandNameFromExecuteMethodName( string methodName, string expectedCommandName )
    {
        DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName( methodName ).Should().Be( expectedCommandName );
    }

    [Fact]
    public void GetCommandPropertyNameFromCommandName()
    {
        DefaultCommandNamingConvention.GetCommandPropertyNameFromCommandName( "Foo" ).Should().Be( "FooCommand" );
    }

    [Fact]
    public void GetCanExecuteNameFromCommandName()
    {
        DefaultCommandNamingConvention.GetCanExecuteNameFromCommandName( "Foo" ).Should().Be( "CanExecuteFoo" );
    }
}