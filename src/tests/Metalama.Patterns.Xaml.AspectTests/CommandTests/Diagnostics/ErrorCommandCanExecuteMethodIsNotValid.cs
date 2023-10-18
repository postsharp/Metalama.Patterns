// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandCanExecuteMethodIsNotValid
{
    [Command]
    public ICommand GenericCommand { get; }

    private void ExecuteGeneric() { }

    private bool CanExecuteGeneric<T>( T value ) => true;

    [Command]
    public ICommand NotBoolCommand { get; }

    private void ExecuteNotBool() { }

    private int CanExecuteNotBool() => 42;

    [Command]
    public ICommand TwoParametersCommand { get; }

    private void ExecuteTwoParameters() { }

    private bool CanExecuteTwoParameters( int a, int b ) => true;

    [Command]
    public ICommand RefParameterCommand { get; }

    private void ExecuteRefParameter() { }

    private bool CanExecuteRefParameter( ref int a ) => true;
}