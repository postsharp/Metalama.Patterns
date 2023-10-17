// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandExecuteMethodIsNotValid
{
    [Command]
    public ICommand GenericCommand { get; }

    private bool ExecuteGeneric<T>( T value ) => true;

    [Command]
    public ICommand NotBoolCommand { get; }

    private int ExecuteNotBool() => 42;

    [Command]
    public ICommand TwoParametersCommand { get; }

    private bool ExecuteTwoParamters( int a, int b ) => true;

    [Command]
    public ICommand RefParameterCommand { get; }

    private bool ExecuteRefParameter( ref int a ) => true;
}