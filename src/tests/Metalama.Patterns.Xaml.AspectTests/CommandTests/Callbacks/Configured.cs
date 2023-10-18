// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;

public class Configured
{
    [Command( ExecuteMethod = nameof(Exec1) )]
    public ICommand ConfiguredExecuteMethodCommand { get; }

    private void Exec1() { }

    private bool CanExecuteConfiguredExecuteMethod() => true;

    [Command( CanExecuteMethod = nameof(CanExec1) )]
    public ICommand ConfiguredCanExecuteMethodCommand { get; }

    private void ExecuteConfiguredCanExecuteMethod() { }

    private bool CanExec1() => true;

    [Command( CanExecuteProperty = nameof(CanExec2) )]
    public ICommand ConfiguredCanExecutePropertyCommand { get; }

    private void ExecuteConfiguredCanExecuteProperty() { }

    private bool CanExec2 => true;
}