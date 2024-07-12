// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Wpf.UnitTests.Assets.Command;
using System.Windows.Input;
using Xunit;

namespace Metalama.Patterns.Wpf.UnitTests.Command;

public sealed class InpcIntegrationTests
{
    private static void TestCanExecuteChanged( ICommand command, Action<bool> setCanExecute )
    {
        var events = new List<string>();

        command.CanExecuteChanged += ( sender, args ) =>
        {
            sender.Should().BeSameAs( command );
            args.Should().BeSameAs( EventArgs.Empty );
            events.Add( $"CanExecute={command.CanExecute( 42 )}" );
        };

        setCanExecute( true );

        events.Should().Equal( "CanExecute=True" );
        events.Clear();

        setCanExecute( false );
        events.Should().Equal( "CanExecute=False" );
    }

    [Fact]
    public void ManualImplementationNotification()
    {
        var c = new ManualInpcIntegrationTestClass();
        TestCanExecuteChanged( c.FooCommand, b => c.CanExecuteFoo = b );
    }

    // TODO: Test disabled due to #34010 - [Observable] overrides the setter, framework generates unsupported `init` keyword in net471.

#if NETCOREAPP
    [Fact]
    public void ObservableAspectImplementationNotification()
    {
        var c = new ObservableAspectIntegrationTestClass();
        TestCanExecuteChanged( c.FooCommand, b => c.CanExecuteFoo = b );
    }
#endif
}