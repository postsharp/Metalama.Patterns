// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandCanExecutePropertyIsNotValid
{
    [Command]
    public ICommand NotBoolCommand { get; }

    private void ExecuteNotBool() { }

    private int CanExecuteNotBool => 42;

    [Command]
    public ICommand NoGetterCommand { get; }

    private void ExecuteNoGetter() { }

    private bool CanExecuteNoGetter
    {
        set { }
    }
}