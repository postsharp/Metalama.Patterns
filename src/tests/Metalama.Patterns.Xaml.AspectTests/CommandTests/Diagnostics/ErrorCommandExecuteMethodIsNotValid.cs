// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandExecuteMethodIsNotValid
{
    [Command]
    public ICommand GenericCommand { get; }

    private void ExecuteGeneric<T>( T value ) { }

    [Command]
    public ICommand TwoParametersCommand { get; }

    private void ExecuteTwoParameters( int a, int b ) { }

    [Command]
    public ICommand RefParameterCommand { get; }

    private void ExecuteRefParameter( ref int a ) { }
}