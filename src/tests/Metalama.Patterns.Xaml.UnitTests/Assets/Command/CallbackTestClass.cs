// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.UnitTests.Assets.Command;

public sealed class CallbackTestClass : CommandTestBase
{
    [Command]
    public ICommand ImplicitInstanceMethodNoParameterCommand { get; }

    private void ExecuteImplicitInstanceMethodNoParameter()
    {
        LogCall();
    }

    private bool CanExecuteImplicitInstanceMethodNoParameter()
    {
        LogCall();
        return CanExecute();
    }

    [Command]
    public ICommand ImplicitInstanceMethodWithParameterCommand { get; }

    private void ExecuteImplicitInstanceMethodWithParameter( int v )
    {
        LogCall( $"{v}" );
    }

    private bool CanExecuteImplicitInstanceMethodWithParameter( int v )
    {
        LogCall( $"{v}" );
        return CanExecute( v );
    }

    [Command]
    public ICommand ImplicitStaticMethodNoParameterCommand { get; }

    private static void ExecuteImplicitStaticMethodNoParameter()
    {
        LogCall();
    }

    private static bool CanExecuteImplicitStaticMethodNoParameter()
    {
        LogCall();
        return CanExecute();
    }

    [Command]
    public ICommand ImplicitStaticMethodWithParameterCommand { get; }

    private static void ExecuteImplicitStaticMethodWithParameter( int v )
    {
        LogCall( $"{v}" );
    }

    private static bool CanExecuteImplicitStaticMethodWithParameter( int v )
    {
        LogCall( $"{v}" );
        return CanExecute( v );
    }

    [Command]
    public ICommand ImplicitInstancePropertyCommand { get; }

    private void ExecuteImplicitInstanceProperty()
    {
        LogCall();
    }

    private bool CanExecuteImplicitInstanceProperty
    {
        get
        {
            LogCall();
            return CanExecute();
        }
    }

    [Command]
    public ICommand ImplicitStaticPropertyCommand { get; }

    private static void ExecuteImplicitStaticProperty()
    {
        LogCall();
    }

    private static bool CanExecuteImplicitStaticProperty
    {
        get
        {
            LogCall();
            return CanExecute();
        }
    }
}