// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;

public class AlternativeNames
{
    [Command]
    public ICommand AlternativeExecuteMethodCommand { get; }

    private void AlternativeExecuteMethod() { }

    [Command]
    public ICommand AlternativeCanExecuteMethodCommand { get; }

    private void ExecuteAlternativeCanExecuteMethod() { }

    private bool CanAlternativeCanExecuteMethod() => true;

    [Command]
    public ICommand AlternativeCanExecutePropertyCommand { get; }

    private void ExecuteAlternativeCanExecuteProperty() { }

    private bool CanAlternativeCanExecuteProperty => true;
}